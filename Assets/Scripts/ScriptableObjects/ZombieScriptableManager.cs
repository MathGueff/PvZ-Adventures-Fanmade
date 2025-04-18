using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieScriptableManager", menuName = "Game/ZombieScriptableManager")]
public class ZombieScriptableManager : ScriptableObject
{
    public enum ZombieName
    {
        BrownCoat,
        ConeHead,
        BucketHead,
        DJ,
        Imp,
        ImposterZombie,
        CongaLeader,
        CongaDancer,
        ZomBull
    }

    // Lista de KeyValuePair para serializar no Inspector
    [SerializeField]
    private List<ZombieScriptableMapping> zombieScriptables;

    // Estrutura para mapear ZombieName para GameObject
    [System.Serializable]
    public struct ZombieScriptableMapping
    {
        public ZombieName zombieName;
        public ZombieScriptable scriptable;
    }

    // Função para obter o prefab correspondente ao ZombieName
    public ZombieScriptable GetZombieScriptable(ZombieName zombieName)
    {
        foreach (var mapping in zombieScriptables)
        {
            if (mapping.zombieName == zombieName)
            {
                return mapping.scriptable;
            }
        }
        Debug.LogWarning($"Prefab não encontrado para o zumbi: {zombieName}");
        return null;
    }
}
