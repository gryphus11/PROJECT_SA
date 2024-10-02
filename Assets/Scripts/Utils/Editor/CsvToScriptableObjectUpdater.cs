using DG.DemiEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CsvToScriptableObjectUpdater
{
    private const string CsvFolderPath = "Assets/@Resources/Data/Csv";
    private const string ScriptableObjectFolderPath = "Assets/@Resources/Data/ScriptableObjects";

    [MenuItem("Tools/Update ScriptableObjects from CSV")]
    public static void UpdateScriptableObjectsFromCSV()
    {
        // CSV 폴더 내의 모든 CSV 파일 경로 가져오기
        string[] csvFilePaths = Directory.GetFiles(CsvFolderPath, "*.csv");

        // 각 CSV 파일에 대해 처리
        foreach (string csvFilePath in csvFilePaths)
        {
            // CSV 파일 이름(확장자 제외) 가져오기
            string csvFileName = Path.GetFileNameWithoutExtension(csvFilePath);

            // ScriptableObject 폴더에서 CSV와 동일한 이름의 ScriptableObject 파일 찾기
            string[] scriptableObjectPaths = Directory.GetFiles(ScriptableObjectFolderPath, $"{csvFileName}.asset", SearchOption.AllDirectories);

            foreach (string scriptableObjectPath in scriptableObjectPaths)
            {
                // ScriptableObject 로드
                ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(scriptableObjectPath);

                if (scriptableObject != null)
                {
                    Debug.Log($"Updating ScriptableObject: {csvFileName} from CSV: {csvFilePath}");

                    // ScriptableObject에 CSV 내용을 채워 넣기
                    PopulateScriptableObjectFromCSV(csvFilePath, scriptableObject);

                    // ScriptableObject 변경 사항 저장
                    EditorUtility.SetDirty(scriptableObject);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        Debug.Log("All matching ScriptableObjects have been updated from CSV.");
    }

    /// <summary>
    /// CSV 파일 내용을 ScriptableObject의 필드에 채워 넣는 함수.
    /// </summary>
    /// <param name="csvFilePath">CSV 파일 경로</param>
    /// <param name="scriptableObject">대상 ScriptableObject</param>
    private static void PopulateScriptableObjectFromCSV(string csvFilePath, ScriptableObject scriptableObject)
    {
        // ScriptableObject의 모든 리스트 필드를 가져오기
        var listFields = GetListFields(scriptableObject);

        // CSV 파일 내용을 읽어 ScriptableObject의 리스트로 변환
        foreach (var listField in listFields)
        {
            Type elementType = listField.FieldType.GetGenericArguments()[0];
            IList dataList = ReadCSVIntoList(csvFilePath, elementType);

            if (dataList == null || dataList.Count == 0)
            {
                Debug.LogError("CSV 파일에서 데이터를 읽어오지 못했습니다.");
                continue;
            }

            // 해당 ScriptableObject의 리스트 필드에 데이터를 할당
            listField.SetValue(scriptableObject, dataList);
            Debug.Log($"Populated {listField.Name} with {dataList.Count} entries.");
        }
    }

    /// <summary>
    /// CSV 파일을 읽어 List로 변환하여 반환하는 함수.
    /// </summary>
    /// <param name="filePath">CSV 파일 경로</param>
    /// <param name="elementType">리스트 요소 타입</param>
    /// <returns>CSV 데이터를 채운 리스트</returns>
    private static IList ReadCSVIntoList(string filePath, Type elementType)
    {
        IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
        FieldInfo[] fields = elementType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        using (StreamReader reader = new StreamReader(filePath))
        {
            // 첫 줄은 헤더이므로 헤더를 읽고 열 이름을 필드와 매핑
            string headerLine = reader.ReadLine();
            if (string.IsNullOrEmpty(headerLine))
            {
                Debug.LogError("CSV 파일이 비어 있습니다.");
                return null;
            }

            string[] headers = headerLine.Split(',');
            Dictionary<string, FieldInfo> fieldDict = new Dictionary<string, FieldInfo>();
            foreach (string header in headers)
            {
                foreach (FieldInfo field in fields)
                {
                    if (field.Name.Equals(header, StringComparison.OrdinalIgnoreCase))
                    {
                        fieldDict.Add(header, field);
                        break;
                    }
                }
            }

            // CSV 데이터를 읽어 해당 필드에 값 설정
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;

                string[] values = line.Split(',');
                object instance = Activator.CreateInstance(elementType);

                for (int i = 0; i < values.Length; i++)
                {
                    if (fieldDict.TryGetValue(headers[i], out FieldInfo field))
                    {
                        try
                        {
                            object value = ConvertValue(values[i], field.FieldType);
                            field.SetValue(instance, value);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Failed to set field value for {field.Name}. Error: {e.Message}");
                        }
                    }
                }

                list.Add(instance);
            }
        }

        return list;
    }

    /// <summary>
    /// CSV에서 읽어온 데이터를 필드 타입에 맞게 변환하는 함수.
    /// </summary>
    private static object ConvertValue(string stringValue, Type fieldType)
    {
        if (fieldType == typeof(int))
            return int.Parse(stringValue);
        else if (fieldType == typeof(float))
            return float.Parse(stringValue);
        else if (fieldType == typeof(string))
            return stringValue;
        else if (fieldType == typeof(bool))
            return bool.Parse(stringValue);
        else if (fieldType.IsEnum)
            return Enum.Parse(fieldType, stringValue);
        else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
        {
            // 리스트 타입인 경우, 문자열을 '|' 구분자로 나누어 리스트로 변환
            Type elementType = fieldType.GetGenericArguments()[0];
            IList list = (IList)Activator.CreateInstance(fieldType);

            string[] elements = stringValue.Split('|');
            foreach (string element in elements)
            {
                if (element.IsNullOrEmpty())
                    continue;

                object listElementValue = ConvertValue(element, elementType);
                list.Add(listElementValue);
            }
            return list;
        }
        else
            throw new NotSupportedException($"Field type {fieldType.Name} is not supported.");
    }

    /// <summary>
    /// ScriptableObject의 필드 중 List 타입 필드를 모두 가져오는 함수.
    /// </summary>
    /// <param name="scriptableObject">대상 ScriptableObject</param>
    /// <returns>List 타입의 필드 정보</returns>
    private static List<FieldInfo> GetListFields(ScriptableObject scriptableObject)
    {
        List<FieldInfo> listFields = new List<FieldInfo>();
        FieldInfo[] fields = scriptableObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                listFields.Add(field);
            }
        }

        return listFields;
    }
}
