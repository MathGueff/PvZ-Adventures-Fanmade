using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RandomWaveController : MonoBehaviour
{
    [Header("Components")]
    private GameController gameController;
    public Flag[] flags;

    [Header("HUD")]
    public Image progressOfLevel;
    public RectTransform zombieHead;
    public GameObject wavePrefab; // Prefab do GameObject que representa a wave
    private GameObject flagObject;

    [Header("Level Info")]
    private float currentTime;
    private int? currentWave;
    private Gravestone[] gravestones;
    private bool inWave;
    private float hordeTime;
    private float zombiesSpawned;
    private float totalZombies;

    [Header("Zombies")]
    public List<GameObject> zombiesPrefabs;
    public int minOfZombies;
    public int maxOfZombies;
    public int minIntervalOfZombies;
    public int maxIntervalOfZombies;
    public List<GameObject> activeZombies = new List<GameObject>();

    [Header("Waves")]
    public int minWaves;
    public int maxWaves;
    public int minIntervalOfWaves;
    public int maxIntervalOfWaves;
    public int minZombiesInWave;
    public int maxZombiesInWave;
    public int minIntervalOfZombiesInWave;
    public int maxIntervalOfZombiesInWave;

    [Header("Definitions")]
    private Dictionary<int, int> zombies = new Dictionary<int, int>(); //ID e intervalo de spawn
    private Dictionary<int, GameObject> zombiePrefab = new Dictionary<int, GameObject>(); //ID e prefab
    private Dictionary<int, ZombieSpawnState> zombiesStates = new Dictionary<int, ZombieSpawnState>(); //ID e prefab

    private Dictionary<int, int> waves = new Dictionary<int, int>(); //ID e tempo de início
    private Dictionary<int, WaveState> wavesStates= new Dictionary<int, WaveState>(); //ID e tempo de início

    private Dictionary<int, List<int>> zombiesInWaves = new Dictionary<int, List<int>>(); // waveID e lista de zombieIDs
    private Dictionary<int, int> zombieSpawnTimesInWave = new Dictionary<int, int>(); // zombieID e intervalo de spawn
    private Dictionary<int, ZombieSpawnState> waveZombiesStates = new Dictionary<int, ZombieSpawnState>(); // zombieID e intervalo de spawn
    private Dictionary<int, GameObject> zombieInWavePrefab = new Dictionary<int, GameObject>(); // zombieID e prefab

    private int zombieCumulativeInterval = 0;
    private int waveCumulativeInterval = 0;
    private int zombieInWaveCumulativeInterval = 0;

    #region Unity Methods

    private void Start()
    {
        gravestones = FindObjectsOfType<Gravestone>();
        gameController = FindObjectOfType<GameController>();
        DefineZombies();
        DefineWaves();
        CalculateTotalZombieCount();
        SetWaveMarkers();
        flags = FindObjectsOfType<Flag>();
        //EventHandler.Instance.OnWaveStarted += StartedWave;
    }

    private void Update()
    {
        if (gameController.gameState == GameState.Started)
        {
            UpdateHUD();
            if (CheckWaves())
            {
                inWave = true;
                hordeTime += Time.deltaTime;
                WavesSpawn();
            }
            else
            {
                inWave = false;
                hordeTime = 0;
                currentTime += Time.deltaTime;
                RegularSpawn();
            }
        }
    }

    #endregion

    #region Checks

    private bool CheckWaves()
    {
        foreach (var wave in waves)
        {
            if (currentTime >= wave.Value && wavesStates[wave.Key] == WaveState.Waiting || wavesStates[wave.Key] == WaveState.Started)
            {
                return true;
            }
        }
        return false;
    }

    private void CheckIfAllZombiesInWaveAreDead()
    {
        if (currentWave == null) return;

        bool allSpawned = true;

        foreach (var zombieInHorde in zombiesInWaves[currentWave.Value])
        {
            // Verifica se o zumbi foi spawnado
            if (waveZombiesStates[zombieInHorde] == ZombieSpawnState.NotSpawned)
            {
                allSpawned = false;
                break;
            }
        }

        Zombie[] zombiesAlive = FindObjectsOfType<Zombie>();

        // Se todos os zumbis foram spawnados, verifica se todos estão mortos
        if (allSpawned)
        {
            if (zombiesAlive.Length == 0)
            {
                wavesStates[currentWave.Value] = WaveState.Ended;
                currentWave = null; // Limpa a onda atual
            }
        }
    }
    public void OnZombieDied(Zombie z)
    {
        CheckIfAllZombiesInWaveAreDead();
    }

    #endregion

    #region SpawnController

    private void RegularSpawn()
    {
        foreach (var zombieID in zombies.Keys)
        {
            if (currentTime >= zombies[zombieID] && zombiesStates[zombieID] == ZombieSpawnState.NotSpawned)
            {
                SpawnZombieAtGravestone(zombiePrefab[zombieID]);
                zombiesStates[zombieID] = ZombieSpawnState.Spawned;
            }
        }
    }

    private void WavesSpawn()
    {
        foreach (var wave in waves.Keys)
        {
            if (currentTime >= waves[wave] && (wavesStates[wave] == WaveState.Waiting || wavesStates[wave] == WaveState.Started))
            {
                if (wavesStates[wave] == WaveState.Waiting)
                {
                    wavesStates[wave] = WaveState.Started;
                    currentWave = wave;
                }
                foreach (var zombieIDInWave in zombiesInWaves[wave])
                {
                    if(hordeTime >= zombieSpawnTimesInWave[zombieIDInWave] && waveZombiesStates[zombieIDInWave] == ZombieSpawnState.NotSpawned)
                    {
                        SpawnZombieAtGravestone(zombieInWavePrefab[zombieIDInWave]);
                        waveZombiesStates[zombieIDInWave] = ZombieSpawnState.Spawned;
                        activeZombies.Add(zombieInWavePrefab[zombieIDInWave]);
                    }
                }
            }
        }
    }

    private void SpawnZombieAtGravestone(GameObject zombiePrefab)
    {
        Gravestone currentGravestone = gravestones[Random.Range(0, gravestones.Length)];

        currentGravestone.SpawnZombie(zombiePrefab);
        zombiesSpawned++;
    }

    #endregion

    #region Wave Events

    private void StartedWave(Collider2D collider)
    {
        //flagObject = collider.gameObject;
    }

    private void EndedWave(Collider2D collider)
    {
        //EventHandler.Instance.WaveDefeated(collider);
    }

    #endregion

    #region Random Generation
    private void DefineZombies()
    {
        int zombieAmount = Random.Range(minOfZombies, maxOfZombies);
        for (int i = 0; i < zombieAmount; i++)
        {
            int interval = Random.Range(minIntervalOfZombies, maxIntervalOfZombies);
            zombieCumulativeInterval += interval;
            zombies.Add(i, zombieCumulativeInterval);
            int randomPrefab = Random.Range(0, zombiesPrefabs.Count);
            zombiePrefab.Add(i, zombiesPrefabs[randomPrefab]);
            zombiesStates.Add(i, ZombieSpawnState.NotSpawned);
            Debug.Log($"Zumbi {zombiesPrefabs[randomPrefab]} no intervalo {zombieCumulativeInterval}");
        }
    }

    private void DefineWaves()
    {
        int waveAmount = Random.Range(minWaves, maxWaves);
        for (int i = 0; i < waveAmount; i++)
        {
            int interval = Random.Range(minIntervalOfWaves, maxIntervalOfWaves);
            waveCumulativeInterval += interval;
            waves.Add(i, waveCumulativeInterval);
            wavesStates.Add(i, WaveState.Waiting);
            Debug.Log($"Wave {i} no intervalo {waveCumulativeInterval}");
            DefineZombiesInWave(i);
            zombieInWaveCumulativeInterval = 0; //Para que não seja aplicado em outras waves!
        }
    }

    private void DefineZombiesInWave(int waveID)
    {
        int zombieAmount = Random.Range(minZombiesInWave, maxZombiesInWave);
        List<int> zombieIDsInWave = new List<int>();

        for (int i = 0; i < zombieAmount; i++)
        {
            int interval = Random.Range(minIntervalOfZombiesInWave, maxIntervalOfZombiesInWave);
            zombieInWaveCumulativeInterval += interval;

            int zombieID = zombieSpawnTimesInWave.Count; // usa a contagem atual como ID
            zombieSpawnTimesInWave[zombieID] = zombieInWaveCumulativeInterval;

            int randomPrefabIndex = Random.Range(0, zombiesPrefabs.Count);
            zombieInWavePrefab[zombieID] = zombiesPrefabs[randomPrefabIndex];

            zombieIDsInWave.Add(zombieID); 
            waveZombiesStates.Add(zombieID, ZombieSpawnState.NotSpawned);
            Debug.Log($"Zumbi da wave {zombiesPrefabs[randomPrefabIndex].gameObject.name} no intervalo {zombieInWaveCumulativeInterval}");
        }

        zombiesInWaves[waveID] = zombieIDsInWave;
        zombieInWaveCumulativeInterval = 0; // Reseta para a próxima onda
    }
    #endregion

    #region HUD

    private void UpdateHUD()
    {
        float progress = zombiesSpawned / totalZombies;
        progressOfLevel.fillAmount = progress;

        UpdateObjectPosition(progress);
    }

    void UpdateObjectPosition(float progress)
    {
        // Posição inicial e final da barra em coordenadas mundiais
        Vector3[] corners = new Vector3[4];
        progressOfLevel.rectTransform.GetWorldCorners(corners);

        Vector3 startPos = corners[0]; // canto inferior esquerdo
        Vector3 endPos = corners[3]; // canto superior direito

        // Calcula a altura (diferença no eixo Y) da barra
        float heightOffset = (corners[1].y - corners[0].y) / 2f;

        // Ajuste a posição no eixo Y para centralizar o objeto
        startPos.y += heightOffset;
        endPos.y += heightOffset;

        // Calcule a nova posição do objeto ao longo da barra
        zombieHead.position = Vector3.Lerp(startPos, endPos, progress);
    }

    private void SetWaveMarkers()
    {
        float cumulativeZombies = 0;

        foreach (var wave in waves.Keys)
        {
            GameObject waveObject = Instantiate(wavePrefab, progressOfLevel.transform);
            RectTransform waveObjectRect = waveObject.GetComponent<RectTransform>();

            float zombiesBeforeWave = 0;

            foreach (var regularSpawn in zombies.Keys)
            {
                if (regularSpawn < waves[wave])
                {
                    zombiesBeforeWave++;
                }
            }

            foreach (var previousWave in waves.Keys)
            {
                if (waves[previousWave] < waves[wave])
                {
                    zombiesBeforeWave += zombiesInWaves[previousWave].Count;
                }
            }

            float wavePosition = (cumulativeZombies + zombiesBeforeWave) / totalZombies;
            float markerX = wavePosition * progressOfLevel.rectTransform.rect.width;

            waveObjectRect.anchoredPosition = new Vector2(markerX, waveObjectRect.anchoredPosition.y);


            cumulativeZombies += zombiesInWaves[wave].Count;
        }
    }

    #endregion

    #region Defines

    private void CalculateTotalZombieCount()
    {
        totalZombies = zombies.Count + zombieSpawnTimesInWave.Count;
    }

    #endregion

}
