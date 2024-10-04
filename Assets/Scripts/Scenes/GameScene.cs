using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

public class GameScene : BaseScene
{
    SpawningPool _spawningPool;
    UniTaskCompletionSource _waveStartSource = null;

    #region Action
    public System.Action<int> onWaveStart;
    public System.Action<int> onSecondChange;
    public System.Action onWaveEnd;
    #endregion

    GameManager _game;
    bool isGameEnd = false;
    private int _lastSecond = 0;
    UI_GameScene _ui;

    protected override bool Init()
    {
        if(base.Init() == false)
            return false;

        sceneType = Define.SceneType.Game;
        _game = Managers.Game;

        InitAsync().Forget();
        return true;
    }

    private async UniTask InitAsync()
    {
        await Managers.Resource.LoadAsyncLabelTask<Object>("Preload",
                (key, count, total) =>
                {
                    Debug.Log($"{key} : {count} / {total}");
                });

        StartLoad();

        foreach (var data in Managers.Data.CreatureDic)
        {
            Debug.Log($"{data.Key} / {data.Value.maxHp}");
        }
    }

    private void StartLoad()
    {
        Managers.Data.Init();

        Managers.UI.ShowSceneUI<UI_Joystick>();
        
        Managers.Game.Character = new CharacterStatus();
        Managers.Game.Character.SetInfo(Managers.Game.SelectedPlayerID);

        var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, Managers.Game.SelectedPlayerID);
        var cameraController = FindObjectOfType<CameraController>();
        cameraController.Target = player.transform;

        var mapObj = Managers.Resource.Instantiate("Map01");
        mapObj.transform.position = Vector3.zero;
        Managers.Game.CurrentMap = mapObj.GetOrAddComponent<MapController>();

        // 범위 디버그용
        Managers.Game.CurrentMap.GetComponent<GridController>().player = player;

        _ui = Managers.UI.ShowSceneUI<UI_GameScene>();

        Managers.Game.CurrentWaveIndex = 0;
        Managers.Game.WaveArray = Managers.Data.WaveDic[1];

        if (_spawningPool == null)
            _spawningPool = gameObject.AddComponent<SpawningPool>();

        onWaveStart = _ui.OnWaveStart;
        onSecondChange = _ui.OnSecondChange;
        onWaveEnd = _ui.OnWaveEnd;

        StartWave(Managers.Game.WaveArray[Managers.Game.CurrentWaveIndex]);
    }

    private void StartWave(WaveData wave)
    {
        if (_waveStartSource != null)
        {
            _waveStartSource.TrySetCanceled();
            _waveStartSource = null;
        }

        StartWaveTask(wave).Forget();
    }

    private async UniTask StartWaveTask(WaveData wave)
    {
        _waveStartSource = new UniTaskCompletionSource();
        Debug.Log($"Wave Start : {wave.WaveIndex}");
        await UniTask.NextFrame();

        onWaveStart?.Invoke(wave.WaveIndex);

        // 스폰시 보상을 준다거나 할 수 있음

        _game.TimeRemaining = _game.WaveArray[_game.CurrentWaveIndex].RemainsTime;

        Vector2 spawnPos = Utils.GetRandomPoint(_game.Player.PlayerCenterPos);

        _spawningPool.StartSpawn();

        //엘리트 몬스터 소환
        MonsterController elite;
        for (int i = 0; i < _game.CurrentWaveData.EleteId.Count; i++)
        {
            elite = Managers.Object.Spawn<MonsterController>(spawnPos, _game.CurrentWaveData.EleteId[i]);
            
            // 추후 UI업데이트
        }
    }

    void WaveEnd()
    {
        Debug.Log($"Wave End : {_game.CurrentWaveIndex}");
        onWaveEnd?.Invoke();

        if (_game.CurrentWaveIndex < _game.WaveArray.Count - 1)
        {
            _game.CurrentWaveIndex++;

            StartWave(_game.WaveArray[_game.CurrentWaveIndex]);
        }
    }

    private void Update()
    {
        if (isGameEnd == true || _game.WaveArray == null || _game.CurrentWaveData == null)
            return;

        _game.TimeRemaining -= Time.deltaTime;

        int currentMinute = Mathf.FloorToInt(_game.TimeRemaining / _game.CurrentWaveData.RemainsTime);
        int currentSecond = (int)_game.TimeRemaining;

        if (currentSecond != _lastSecond)
        {
            onSecondChange?.Invoke(currentSecond);
        }

        if (_game.TimeRemaining < 0)
        {
            //wave 종료
            WaveEnd();
        }
    }
}
