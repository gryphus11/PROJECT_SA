using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

public class ResourceManager
{
    private Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, Object>();

    private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();

    public int HandleCount { get; private set; }

    /// <summary>
    /// 메모리에 있는 리소스를 불러온다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T Load<T>(string key) where T : Object
    {
        if (_resources.TryGetValue(key, out Object resource))
        {
            return resource as T;
        }
        
        return null;
    }

    public async UniTask<T> LoadAssetAsync<T>(string key, System.Action<T> callback = null) where T : Object
    {
        Debug.Log($"Load Async Asset : {key}");

        // 이미 로드되어 있는 경우
        if (_resources.TryGetValue(key, out Object resource))
        {
            callback?.Invoke(resource as T);
            return resource as T;
        }

        // 로딩이 진행중인 경우 콜백만 추가
        if (_handles.ContainsKey(key))
        {
            _handles[key].Completed += (op) => { callback?.Invoke(op.Result as T); };
            return null;
        }

        // Sprite 로드 처리
        string loadKey = key;
        if (key.Contains(".sprite"))
            loadKey = $"{key}[{key.Replace(".sprite", "")}]";

        var handle = Addressables.LoadAssetAsync<T>(loadKey);
        _handles.Add(key, handle);
        HandleCount++;

        handle.Completed += (op) =>
        {
            T result = op.Result;

            if (key.Contains(".sprite") && result is Texture2D texture)
            { 
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                result = sprite as T;
            }

            _resources.Add(key, result);
            callback?.Invoke(result);
            HandleCount--;
        };

        await handle.ToUniTask();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Succeeded To Load Asset Async : {key}");
            return handle.Result;
        }
        else
        {
            Debug.LogError($"Failed To Load Asset Async : {key} / {handle.OperationException}");
            return null;
        }
    }

    public void LoadAllAsync<T>(string key, System.Action<List<T>> callback = null) where T : Object
    {
        LoadAssetsAsync(key, callback).Forget();
    }

    #region 선로딩을 위한 함수
    /// <summary>
    /// 라벨을 통해 선 로딩을 수행
    /// </summary>
    /// <typeparam name="T">제네릭 타입</typeparam>
    /// <param name="key">라벨</param>
    /// <param name="loadCallback">개별 로드 콜백</param>
    /// <param name="onComplete">모두 완료 콜백</param>
    public void LoadAsyncLabel<T>(string key, System.Action<string, int, int> loadCallback = null, System.Action onComplete = null) where T : Object
    {
        LoadAsyncLabelTask<T>(key, loadCallback, onComplete).Forget();
    }

    public async UniTask LoadAsyncLabelTask<T>(string key, System.Action<string, int, int> loadCallback = null, System.Action onComplete = null) where T : Object
    {
        var locations = await LoadResourceLocationsAsync(key);

        if (locations == null)
            return;

        int loadCount = 0;
        int totalCount = locations.Count;

        List<UniTask> loadTasks = new List<UniTask>();
        
        foreach (var location in locations)
        {
            string loadKey = location.PrimaryKey;

            var loadTask = LoadAssetAsync<T>(loadKey, (asset) =>
            {
                ++loadCount;
                loadCallback?.Invoke(loadKey, loadCount, totalCount);
            });
            
            loadTasks.Add(loadTask);
        }

        await UniTask.WhenAll(loadTasks);

        onComplete?.Invoke();
    }
    #endregion

    public async UniTask<List<T>> LoadAssetsAsync<T>(object key, System.Action<List<T>> callback = null) where T : Object
    {
        var locations = await LoadResourceLocationsAsync(key);

        if (locations == null)
            return null;

        var results = new List<T>();

        await LoadAndUpdateCollection(locations, results);

        callback?.Invoke(results);
        return results;
    }


    async UniTask LoadAndUpdateCollection<T>(IList<IResourceLocation> locations, List<T> loadedAssetList) where T : Object
    {
        List<UniTask> tasks = new List<UniTask>();

        foreach (var location in locations)
        {
            string key = location.PrimaryKey;
            var task = LoadAssetAsync<T>(key, 
            (asset) => 
            {
                if (asset != null && !loadedAssetList.Contains(asset))
                    loadedAssetList.Add(asset);
            });

            tasks.Add(task);
        }

        await UniTask.WhenAll(tasks);
    }

    public void InstantiateAsync(string key, Transform parent = null, System.Action<GameObject> callback = null)
    {
        InstantiateAsyncTask(key, parent, callback).Forget();
    }

    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        // 선로드 한 것으로 동기식 인스턴스화
        var prefab = Load<GameObject>(key);

        if (prefab == null)
        {
            Debug.Log($"{key} is Null");
            return null;
        }

        if (pooling)
            return Managers.Pool.Pop(prefab);

        GameObject instance = Object.Instantiate(prefab, parent);
        instance.name = prefab.name;

        if (parent != null)
            instance.transform.localPosition = parent.transform.position;

        return instance;
    }

    public async UniTask<GameObject> InstantiateAsyncTask(string key, Transform parent = null, System.Action<GameObject> callback = null)
    {
        var prefab = await LoadAssetAsync<GameObject>(key);

        if (prefab == null)
        {
            Debug.Log($"{key} is Null");
            return null;
        }

        GameObject instance = GameObject.Instantiate(prefab, parent);
        instance.name = prefab.name;

        if (parent != null)
            instance.transform.localPosition = parent.transform.position;

        callback?.Invoke(instance);

        return instance;
    }



    public void Release(string key)
    {
        if (_resources.TryGetValue(key, out Object resource) == false)
            return;

        _resources.Remove(key);

        if (_handles.TryGetValue(key, out AsyncOperationHandle handle))
            Addressables.Release(handle);

        _handles.Remove(key);
    }

    public void Clear()
    {
        _resources.Clear();

        foreach (var handle in _handles.Values)
            Addressables.Release(handle);

        _handles.Clear();
    }

    public void Destroy(GameObject go, float seconds = 0.0f)
    {
        if (go == null)
            return;

        //Debug.Log($"####Destroy : {go.GetInstanceID()}");

        // 풀에 넣기 성공했다면 리턴
        if (Managers.Pool.Push(go))
            return;

        Object.Destroy(go, seconds);
    }

    // 라벨에 해당하는 모든 게임 오브젝트를 인스턴스화하는 함수
    public async UniTask<List<GameObject>> InstantiateAllWithLabelAsync(string label, Transform parent = null)
    {
        var locations = await LoadResourceLocationsAsync(label);

        if (locations == null)
        {
            Debug.LogError($"Failed to load resource locations for label: {label}");
            return null;
        }

        var instances = new List<GameObject>();

        foreach (var location in locations)
        {
            var instance = await InstantiateAsyncTask(location.PrimaryKey, parent, null);
            if (instance != null)
            {
                Debug.Log($"#### Instantiated : {location.PrimaryKey} ");
                instances.Add(instance);
            }
            else
            {
                Debug.LogError($"Failed to instantiate object at {location}");
            }
        }

        return instances;
    }


    // 리소스 경로를 불러 온다
    private async UniTask<IList<IResourceLocation>> LoadResourceLocationsAsync(object key)
    {
        var locationsHandle = Addressables.LoadResourceLocationsAsync(key);
        await locationsHandle.ToUniTask();

        if (locationsHandle.Status == AsyncOperationStatus.Succeeded && locationsHandle.Result.Count > 0)
        {
            return locationsHandle.Result;
        }
        else
        {
            return null;
        }
    }

    public void ReleaseAssets<T>(IList<T> objects) where T : Object
    {
        try
        {
            if (objects == null)
                return;

            foreach (var obj in objects)
            {
                if (obj == null)
                    continue;

                if (obj is GameObject)
                {
                    Addressables.ReleaseInstance(obj as GameObject);
                }
                else
                {
                    Addressables.Release(obj);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log($"{e}");
        }
    }
}
