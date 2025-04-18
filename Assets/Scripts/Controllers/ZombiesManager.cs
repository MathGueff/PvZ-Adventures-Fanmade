using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesManager : MonoBehaviour
{
    [Header("Singleton")]
    public static ZombiesManager instance;

    [Header("Zombies Details")]
    public List<GameObject> allZombiesAlive; //Lista de Zumbis vivos na cena
    public List<GameObject> zombiesForWaveAlive; //Lista de zumbis da wave atual vivos
    public float totalZombiesInLevel; //Total de Zumbis do level
    public float totalZombiesSpawned; //Total de Zumbis já spawnados
    public float currentWaveZombiesSpawned; //Quantidade de Zumbis da wave atual spawnados

    #region Dictionarys
    //Dicionário de Spawn Regular e Estado de Spawn do Zumbi
    public Dictionary<LevelDataScriptable.ZombieSpawnData, ZombieSpawnState> normalSpawns = 
        new Dictionary<LevelDataScriptable.ZombieSpawnData, ZombieSpawnState>();

    //Dicionário de Wave e Estado da Wave
    public Dictionary<LevelDataScriptable.Wave, WaveState> wavesInLevel = 
        new Dictionary<LevelDataScriptable.Wave, WaveState>();

    //Dicionário de Wave e Zumbi da Wave com Estado do Zumbi
    public Dictionary<LevelDataScriptable.Wave,
        Dictionary<LevelDataScriptable.ZombieSpawnData, ZombieSpawnState>> waveZombiesStates =
        new Dictionary<LevelDataScriptable.Wave, Dictionary<LevelDataScriptable.ZombieSpawnData, ZombieSpawnState>>();

    //Dicionário com quantidade de zumbis em cada horda
    public Dictionary<LevelDataScriptable.Wave, int> waveZombiesCount = new Dictionary<LevelDataScriptable.Wave, int>();

    //Acessos encapsulados dos Dicionários
    public static Dictionary<LevelDataScriptable.Wave, WaveState> WavesInLevel => instance.wavesInLevel;
    public static Dictionary<LevelDataScriptable.ZombieSpawnData, ZombieSpawnState> NormalSpawns => instance.normalSpawns;
    public static Dictionary<LevelDataScriptable.Wave,Dictionary<LevelDataScriptable.ZombieSpawnData, ZombieSpawnState>> WaveZombiesStates => instance.waveZombiesStates;
    public static Dictionary<LevelDataScriptable.Wave, int> WaveZombiesCount => instance.waveZombiesCount;

    #endregion

    private void Awake()
    { 
        if (instance == null)
        {
            instance = this;
            GameController.LevelData.InitializeZombiePrefabs();
            DefineLevel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CalculateTotalZombie()
    {
        totalZombiesInLevel = GameController.LevelData.regularSpawns.Count; // Contagem inicial dos zumbis regulares
        foreach (var wave in GameController.LevelData.waves)
        {
            totalZombiesInLevel += wave.zombiesInHorde.Count; // Adiciona a contagem de zumbis das hordas
        }
    }

    public bool CheckIfAllZombiesSpawned()
    {
        if(totalZombiesSpawned == totalZombiesInLevel)
        {
            return true;
        }
        return false;
    }

    #region Dicts Defines
    private void DefineLevel()
    {
        DefineRegularSpawnZombies();
        DefineWaves();
        DefineWaveZombiesState();
        CalculateTotalZombie();
    }

    private void DefineRegularSpawnZombies()
    {
        foreach (var normalSpawn in GameController.LevelData.regularSpawns)
        {
            normalSpawns.Add(normalSpawn, ZombieSpawnState.NotSpawned);
        }
    }

    private void DefineWaves()
    {
        foreach (var wave in GameController.LevelData.waves)
        {
            wavesInLevel.Add(wave, WaveState.Waiting);
        }
    }

    private void DefineWaveZombiesState()
    {
        int i = 0;
        // Inicializa o dicionário para cada onda
        foreach (var wave in GameController.LevelData.waves)
        {
            int count = 0;
            i++;
            var zombiesInWave = new Dictionary<LevelDataScriptable.ZombieSpawnData, ZombieSpawnState>();

            foreach (var zombieInHorde in wave.zombiesInHorde)
            {
                count++;
                zombiesInWave.Add(zombieInHorde, ZombieSpawnState.NotSpawned);
            }

            waveZombiesStates.Add(wave, zombiesInWave);
            waveZombiesCount.Add(wave, count);
        }
    }
    #endregion
}