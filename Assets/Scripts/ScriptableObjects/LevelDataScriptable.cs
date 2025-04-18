using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Levels/Level")]
public class LevelDataScriptable : ScriptableObject
{
    [Header("Plants")]
    public int sunAmountStart;
    public int numberOfPots;
    public int insolationChance;

    [Header("Zombies")]
    // Dicionário para mapear ZombieName para o GameObject correspondente
    [SerializeField]
    private ZombieScriptableManager zombieScriptables;
    public ZombieScriptableManager ZombieScriptables { get => zombieScriptables; set => zombieScriptables = value; }

    // Estrutura para armazenar um zumbi e o tempo de spawn
    [System.Serializable] 
    public class ZombieSpawnData
    {
        public ZombieScriptableManager.ZombieName zombie;
        [HideInInspector] public ZombieScriptable zombieScriptable;
        public float spawnTime;  // Tempo em que o zumbi deve ser spawnado
        public bool spawnAtSameGravestone;
        public bool spawnAtDiffGravestone;

        public void DefineZombiePrefab(ZombieScriptableManager manager)
        {
            if(manager != null)
                zombieScriptable = manager.GetZombieScriptable(zombie);
            else
            {
                Debug.Log("ZombieScriptableManager não atribuído");
            }
        }
    }

    // Estrutura para armazenar uma horda de zumbis
    [System.Serializable]
    public class Wave
    {
        public List<ZombieSpawnData> zombiesInHorde;  // Lista de zumbis que compõem a horda
        public float hordeTime;  // Tempo em que a horda começa
    }

    public List<ZombieSpawnData> regularSpawns;  // Lista de zumbis para spawnar regularmente
    public List<Wave> waves;  // Lista de hordas

    // Método para inicializar os prefabs dos zumbis
    public void InitializeZombiePrefabs()
    {
        foreach (var spawnData in regularSpawns)
        {
            spawnData.DefineZombiePrefab(ZombieScriptables);
        }

        foreach (var wave in waves)
        {
            foreach (var spawnData in wave.zombiesInHorde)
            {
                spawnData.DefineZombiePrefab(ZombieScriptables);
            }
        }
    }
}
