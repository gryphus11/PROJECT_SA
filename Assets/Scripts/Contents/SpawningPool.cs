using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Cysharp.Threading.Tasks;

public class SpawningPool : MonoBehaviour
{

    public int _maxMonsterCount = 1000;
    UniTaskCompletionSource _spawnTask = null;
    GameManager _game;

    public void StartSpawn()
    {
        _game = Managers.Game;

        if (_spawnTask == null)
            UpdateSpawningPoolTask().Forget();
    }

    private async UniTask UpdateSpawningPoolTask()
    {
        _spawnTask = new UniTaskCompletionSource();

        while (true)
        {
            if (_game.CurrentWaveData.MonsterId.Count == 1)
            {
                for (int i = 0; i < _game.CurrentWaveData.OnceSpawnCount; i++)
                {
                    Vector2 spawnPos = Utils.GetRandomPoint(Managers.Game.Player.PlayerCenterPos);
                    Managers.Object.Spawn<MonsterController>(spawnPos, _game.CurrentWaveData.MonsterId[0]);
                }
            }
            else
            {
                for (int i = 0; i < _game.CurrentWaveData.OnceSpawnCount; i++)
                {
                    Vector2 spawnPos = Utils.GetRandomPoint(Managers.Game.Player.PlayerCenterPos);

                    // ù ���� ���� Ȯ���� ���� ����
                    if (Random.value <= Managers.Game.CurrentWaveData.FirstMonsterSpawnRate)
                    {
                        Managers.Object.Spawn<MonsterController>(spawnPos, _game.CurrentWaveData.MonsterId[0]);
                    }
                    else // �ٸ� ���� ������ ���Ӵ� ���� �� ����
                    {
                        int randomIndex = Random.Range(1, _game.CurrentWaveData.MonsterId.Count);
                        Managers.Object.Spawn<MonsterController>(spawnPos, _game.CurrentWaveData.MonsterId[randomIndex]);
                    }
                }
            }
            
            await UniTask.Delay((int)(_game.CurrentWaveData.SpawnInterval * 1000), cancellationToken: destroyCancellationToken);
        }
    }
}