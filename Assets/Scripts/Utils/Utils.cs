using UnityEngine;

public class Utils
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static Vector2 GetRandomPoint(Vector2 targetPoint, float minDistance = 10.0f, float maxDistance = 20.0f)
    {
        // 랜덤한 각도를 생성 (0에서 2π 사이)
        float randomAngle = Random.Range(0.0f, Mathf.PI * 2.0f);

        // 거리 범위 내에서 랜덤한 거리 생성
        float distance = Random.Range(minDistance, maxDistance);

        // 랜덤 방향으로 이동한 x, y 좌표 계산
        float xDistance = Mathf.Cos(randomAngle) * distance;
        float yDistance = Mathf.Sin(randomAngle) * distance;

        // 계산된 랜덤 포인트
        Vector2 randomPoint = targetPoint + new Vector2(xDistance, yDistance);

        return randomPoint;
    }
}
