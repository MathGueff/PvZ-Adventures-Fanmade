using System.Collections.Generic;
using UnityEngine;

public enum ZombieSpawnState
{
    Spawned,
    NotSpawned
}

public enum WaveState
{
    Waiting,
    Started,
    Ended
}

public class WaveController : MonoBehaviour
{
    [Header("Level Info")]
    public LevelDataScriptable.Wave currentWave;

    [Header("Time")]
    public float currentTime;
    public float hordeTime;

    [Header("Components")]
    private ZombieSpawnController zombieSpawnController;
    private LevelHUDController levelHUDController;

    [Header("Sounds")]
    [SerializeField] private AudioClip waveStartSound;

    #region Unity Methods
    private void Start()
    {
        zombieSpawnController = FindObjectOfType<ZombieSpawnController>();
        levelHUDController = FindObjectOfType<LevelHUDController>();
    }

    private void Update()
    {
        if (GameController.instance.gameState == GameState.Started && GameController.LevelData != null)
        {
            levelHUDController.UpdateHUD();
            if (zombieSpawnController.gravestones.Length > 0)
            {
                if (CheckWaves())
                {
                    hordeTime += Time.deltaTime;
                    zombieSpawnController.WavesSpawn(currentWave);
                }
                else
                {
                    currentTime += Time.deltaTime;
                    zombieSpawnController.RegularSpawn();
                }
            }
            else
            {
                Debug.Log("Gravestones nao existem: " + zombieSpawnController.gravestones.Length);
            }
        }
    }
    #endregion

    #region Checks
    public bool CheckWaves()
    {
        foreach (var wave in ZombiesManager.WavesInLevel)
        {
            if (wave.Value == WaveState.Ended)
                continue;

            //Se a hora da horda tiver chegado, a horda estiver em Estado de Waiting ou Começado, retorna true
            if ((currentTime >= wave.Key.hordeTime && wave.Value == WaveState.Waiting) 
                || wave.Value == WaveState.Started)
            {
                StartWave(wave.Key);
                return true;
            }
        }
        return false;
    }

    public int GetNumberOfZombiesOfWave(LevelDataScriptable.Wave wave)
    {
        return ZombiesManager.WaveZombiesCount[wave];
    }

    private void CheckIfAllZombiesInWaveAreDead()
    {
        int totalZombiesOfWave;
        totalZombiesOfWave = GetNumberOfZombiesOfWave(currentWave); 
        // Se todos os zumbis foram spawnados, verifica se todos estão mortos
        if (ZombiesManager.instance.currentWaveZombiesSpawned == totalZombiesOfWave)
        {
            if (ZombiesManager.instance.zombiesForWaveAlive.Count == 0)
            {
                EndWave();
            }
        }
    }

    #endregion

    private void StartWave(LevelDataScriptable.Wave wave)
    {
        if (ZombiesManager.WavesInLevel[wave] == WaveState.Waiting)
        {
            ZombiesManager.WavesInLevel[wave] = WaveState.Started;
            currentWave = wave;
            EventHandler.Instance.WaveStarted(currentWave);
            SoundManager.instance.PlaySound(waveStartSound);
        }
    }

    public void EndWave()
    {
        hordeTime = 0;
        ZombiesManager.WavesInLevel[currentWave] = WaveState.Ended;
        EventHandler.Instance.WaveDefeated(currentWave);
        currentWave = null; // Limpa a onda atual
        ZombiesManager.instance.currentWaveZombiesSpawned = 0;
    }

    public void OnZombieDied(Zombie z)
    {
        ZombiesManager.instance.allZombiesAlive.Remove(z.gameObject);
        if (ZombiesManager.instance.zombiesForWaveAlive.Contains(z.gameObject))
        {
            ZombiesManager.instance.zombiesForWaveAlive.Remove(z.gameObject);
            CheckIfAllZombiesInWaveAreDead();
        }
    }
}
