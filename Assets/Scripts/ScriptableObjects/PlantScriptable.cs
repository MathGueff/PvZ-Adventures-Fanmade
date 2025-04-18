using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plant", menuName = "Plants/Plant")]
public class PlantScriptable : ScriptableObject
{
    #region Mechanics

    [Header("Life")]
    public int maxHealth;
    public float timeToRevive = 15f;
    public int sunCustToRevive = 25;
    public bool canRecover;

    [Header("Action")]
    public float actionInterval; // Intervalo entre ataques

    [Header("Attack")]
    public int plantDamage; // Dano causado pela planta
    public DetectAttackType typeOfAttack;
    public Vector2 dettectRange;
    public DamageType damageType;

    [Header("Boosts")]
    public float boostsDuration;
    public List<ClickBoostScriptable> boostsScriptables = new List<ClickBoostScriptable>();
    public int custToBoost;

    #endregion
    #region UI and Pots
    [Header("Prefab")]
    public GameObject plantPrefab;
    public GameObject plantPreviewPrefab;

    [Header("Sprite")]
    public Sprite plantSprite;

    [Header("Info")]
    public string plantName;
    public PlantType plantType;

    [Header("Cooldown")]
    public float plantSeedCooldown;

    [Header("Seeds Quantity")]
    public int plantSeedQuantity;

    [Header("Sun")]
    public int plantSunCust;

    [Header("Preview")]
    public Color rangeColor;
    #endregion
}
