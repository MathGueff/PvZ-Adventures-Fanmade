using UnityEngine;

public class ZombieSpawnController : MonoBehaviour
{
    [Header("Gravestones")]
    [HideInInspector] public Gravestone[] gravestones;
    [HideInInspector] public Gravestone currentGravestone;

    [Header("Components")]
    private WaveController waveController;

    [Header("Controllers")]
    public float zombiesSpawned;

    [Header("Sounds")]
    [SerializeField] private AudioClip zombiesComingSound;
    [SerializeField] private AudioClip zombieSpawning;

    private void Awake()
    {
        gravestones = FindObjectsOfType<Gravestone>();
        waveController = FindObjectOfType<WaveController>();
    }

    #region SpawnController
    public void RegularSpawn()
    {
        foreach (var zombieSpawned in GameController.LevelData.regularSpawns)
        {
            if (waveController.currentTime >= zombieSpawned.spawnTime && ZombiesManager.NormalSpawns[zombieSpawned] == ZombieSpawnState.NotSpawned)
            {
                SpawnZombieAtGravestone(zombieSpawned, false);
                ZombiesManager.NormalSpawns[zombieSpawned] = ZombieSpawnState.Spawned;
            }
        }
    }

    public void WavesSpawn(LevelDataScriptable.Wave wave)
    {
        foreach (var zombieInHorde in wave.zombiesInHorde)
        {
            if (ZombiesManager.WaveZombiesStates[wave][zombieInHorde] == ZombieSpawnState.Spawned)
                continue;

            //Se estiver no tempo do zumbi spawnar e seu estado for NãoSpawnado
            if (waveController.hordeTime >= zombieInHorde.spawnTime)
            {
                //Spawna zumbi da horda
                SpawnZombieAtGravestone(zombieInHorde, true);
                ZombiesManager.WaveZombiesStates[wave][zombieInHorde] = ZombieSpawnState.Spawned;
                ZombiesManager.instance.currentWaveZombiesSpawned++;
            }
        }
    }

    private void SpawnZombieAtGravestone(LevelDataScriptable.ZombieSpawnData zombieData, bool isWaveZombie = false)
    {
        Gravestone gravestone = null;
        gravestone = DefineGravestone(zombieData);
        
        gravestone.SpawnZombie(zombieData.zombieScriptable.zombiePrefab, isWaveZombie);
        if(zombiesSpawned == 0)
        {
            SoundManager.instance.PlaySound(zombiesComingSound);
        }
        SoundManager.instance.PlaySound(zombieSpawning);
        zombiesSpawned++;
        ZombiesManager.instance.totalZombiesSpawned++;
    }

    public Gravestone DefineGravestone(LevelDataScriptable.ZombieSpawnData zombieData)
    {
        Gravestone gravestone = null;

        if (currentGravestone == null)
        {
            currentGravestone = getRandomGravestone();
        }

        if (zombieData.spawnAtSameGravestone)
        {
            gravestone = currentGravestone;
        }
        else
        {
            gravestone = getRandomGravestone();
            if (zombieData.spawnAtDiffGravestone && gravestone == currentGravestone && gravestones.Length > 1)
            {
                do
                {
                    gravestone = getRandomGravestone();
                } while (gravestone == currentGravestone);
            }
            currentGravestone = gravestone;
        }
        return gravestone;
    }

    public Gravestone getRandomGravestone()
    {
        return gravestones[Random.Range(0, gravestones.Length)];
    }

    #endregion
}