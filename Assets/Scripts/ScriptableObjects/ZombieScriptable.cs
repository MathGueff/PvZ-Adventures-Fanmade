using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Zombie", menuName = "Zombies/Zombie")]
public class ZombieScriptable : ScriptableObject
{
    [Header("Attack")]
    public int zombieDamage;
    public TargetTypePlant targetTypePlant;
   
    [Header("Speed")]
    public float zombieSpeed;

    [Header("Life")]
    public int zombieHealth;
    public List<int> armorsHealth;
    public Vector2 zombieRange;
    public DamageType armorType;

    [Header("Prefab")]
    public GameObject zombiePrefab;

    [Header("Sprite")]
    public Sprite zombieSprite;

    [Header("Info")]
    public string zombieName;
}
