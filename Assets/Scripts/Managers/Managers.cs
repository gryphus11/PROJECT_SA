using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _instance;
    static bool _isInitialized = false;

    public static Managers Instance
    {
        get
        {
            Init();
            return _instance;
        }
    }

    public static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("Managers");
            if (go == null)
                go = new GameObject { name = "Managers" };

            _instance = Utils.GetOrAddComponent<Managers>(go);
            DontDestroyOnLoad(go);

            // Init Others
            Sound.Init();
        }
    }



    #region Contents
    GameManager _game = new GameManager();
    ObjectManager _object = new ObjectManager();

    public static GameManager Game { get { return Instance?._game; } }
    public static ObjectManager Object { get { return Instance?._object; } }
    #endregion

    #region Core
    DataManager _data = new DataManager();
    ResourceManager _resource = new ResourceManager();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();
    SceneManagerEx _scene = new SceneManagerEx();
    PoolManager _pool = new PoolManager();

    public static DataManager Data { get { return Instance?._data; } }
    public static ResourceManager Resource { get { return Instance?._resource; } }
    public static SoundManager Sound { get { return Instance?._sound; } }
    public static UIManager UI { get { return Instance?._ui; } }
    public static SceneManagerEx Scene { get { return Instance?._scene; } }
    public static PoolManager Pool { get { return Instance?._pool; } }
    #endregion


    public static void Clear()
    {
        _instance._sound.Clear();
        _instance._ui.Clear();
        _instance._object.Clear();
        _instance._scene.Clear();
        _instance._pool.Clear();
    }

}
