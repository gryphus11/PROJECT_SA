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
        // CSV ���� ���� ��� CSV ���� ��� ��������
        string[] csvFilePaths = Directory.GetFiles(CsvFolderPath, "*.csv");

        // �� CSV ���Ͽ� ���� ó��
        foreach (string csvFilePath in csvFilePaths)
        {
            // CSV ���� �̸�(Ȯ���� ����) ��������
            string csvFileName = Path.GetFileNameWithoutExtension(csvFilePath);

            // ScriptableObject �������� CSV�� ������ �̸��� ScriptableObject ���� ã��
            string[] scriptableObjectPaths = Directory.GetFiles(ScriptableObjectFolderPath, $"{csvFileName}.asset", SearchOption.AllDirectories);

            foreach (string scriptableObjectPath in scriptableObjectPaths)
            {
                // ScriptableObject �ε�
                ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(scriptableObjectPath);

                if (scriptableObject != null)
                {
                    Debug.Log($"Updating ScriptableObject: {csvFileName} from CSV: {csvFilePath}");

                    // ScriptableObject�� CSV ������ ä�� �ֱ�
                    PopulateScriptableObjectFromCSV(csvFilePath, scriptableObject);

                    // ScriptableObject ���� ���� ����
                    EditorUtility.SetDirty(scriptableObject);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        Debug.Log("All matching ScriptableObjects have been updated from CSV.");
    }

    /// <summary>
    /// CSV ���� ������ ScriptableObject�� �ʵ忡 ä�� �ִ� �Լ�.
    /// </summary>
    /// <param name="csvFilePath">CSV ���� ���</param>
    /// <param name="scriptableObject">��� ScriptableObject</param>
    private static void PopulateScriptableObjectFromCSV(string csvFilePath, ScriptableObject scriptableObject)
    {
        // ScriptableObject�� ��� ����Ʈ �ʵ带 ��������
        var listFields = GetListFields(scriptableObject);

        // CSV ���� ������ �о� ScriptableObject�� ����Ʈ�� ��ȯ
        foreach (var listField in listFields)
        {
            Type elementType = listField.FieldType.GetGenericArguments()[0];
            IList dataList = ReadCSVIntoList(csvFilePath, elementType);

            if (dataList == null || dataList.Count == 0)
            {
                Debug.LogError("CSV ���Ͽ��� �����͸� �о���� ���߽��ϴ�.");
                continue;
            }

            // �ش� ScriptableObject�� ����Ʈ �ʵ忡 �����͸� �Ҵ�
            listField.SetValue(scriptableObject, dataList);
            Debug.Log($"Populated {listField.Name} with {dataList.Count} entries.");
        }
    }

    /// <summary>
    /// CSV ������ �о� List�� ��ȯ�Ͽ� ��ȯ�ϴ� �Լ�.
    /// </summary>
    /// <param name="filePath">CSV ���� ���</param>
    /// <param name="elementType">����Ʈ ��� Ÿ��</param>
    /// <returns>CSV �����͸� ä�� ����Ʈ</returns>
    private static IList ReadCSVIntoList(string filePath, Type elementType)
    {
        IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
        FieldInfo[] fields = elementType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        using (StreamReader reader = new StreamReader(filePath))
        {
            // ù ���� ����̹Ƿ� ����� �а� �� �̸��� �ʵ�� ����
            string headerLine = reader.ReadLine();
            if (string.IsNullOrEmpty(headerLine))
            {
                Debug.LogError("CSV ������ ��� �ֽ��ϴ�.");
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

            // CSV �����͸� �о� �ش� �ʵ忡 �� ����
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
    /// CSV���� �о�� �����͸� �ʵ� Ÿ�Կ� �°� ��ȯ�ϴ� �Լ�.
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
            // ����Ʈ Ÿ���� ���, ���ڿ��� '|' �����ڷ� ������ ����Ʈ�� ��ȯ
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
    /// ScriptableObject�� �ʵ� �� List Ÿ�� �ʵ带 ��� �������� �Լ�.
    /// </summary>
    /// <param name="scriptableObject">��� ScriptableObject</param>
    /// <returns>List Ÿ���� �ʵ� ����</returns>
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
