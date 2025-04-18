using System.Collections.Generic;
using UnityEngine;

public abstract class Plant : MonoBehaviour
{
    #region Attributes
    [Header("Health")]
    private int maxHealth;
    private int currentHealth;
    private float damageReceivedModifier = 1f;
    private bool canBeRevived;
    private float timeToRevive;
    private int sunCustToRevive;

    [Header("Attack")]
    private int plantDamage;
    private float actionInterval;
    private float actionTimer = 0f;
    private Vector2 dettectRange;
    private DamageType damageType;
    private float boostsDuration;
    private int custToBoost;

    [Header("Components")]
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public PlantLifeHandler plantLifeHandler;
    [HideInInspector] public PlantAttackHandler plantAttackHandler;
    [HideInInspector] public PlantEffects plantEffects;
    [HideInInspector] public PlantBoostManager plantBoostManager;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PlantRangeArea plantRangeArea;
    [HideInInspector] public BoxCollider2D plantBoxCollider;

    [Header("Scriptables")]
    public PlantScriptable plantSettings;
    private List<ClickBoostScriptable> boostsScriptables;

    [Header("Enums")]
    private PlantAction plantAction;
    private PlantLifeState plantLifeState;
    private CharacterDirection plantDirection;
    private CharacterHorizontalDirection plantHorizontalDirection = CharacterHorizontalDirection.None;
    private PlantType plantType;

    [Header("Layers")]
    private LayerMask zombieLayer;

    [Header("Bool")]
    private bool canRecover;
    private bool plantBoosted;

    #region Get & Set
    public CharacterDirection PlantDirection 
    { 
        get => plantDirection;
        set
        {
            if (plantDirection == value) return;
            plantDirection = value;
            if(anim != null)
            {
                anim.SetInteger("direction", ((int)plantDirection));
            }
        } 
    }
    public CharacterHorizontalDirection PlantHorizontalDirection 
    { 
        get => plantHorizontalDirection;
        set 
        {
            if (plantHorizontalDirection == value) return;
            plantHorizontalDirection = value;

            switch (plantHorizontalDirection)
            {
                case CharacterHorizontalDirection.Right:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case CharacterHorizontalDirection.Left:
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
            }
        }
    }

    public float DamageReceivedModifier { get => DamageReceivedModifier1; set => DamageReceivedModifier1 = value; }

    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float DamageReceivedModifier1 { get => damageReceivedModifier; set => damageReceivedModifier = value; }
    public bool CanBeRevived { get => canBeRevived; set => canBeRevived = value; }
    public float TimeToRevive { get => timeToRevive; set => timeToRevive = value; }
    public int SunCustToRevive { get => sunCustToRevive; set => sunCustToRevive = value; }
    public int PlantDamage { get => plantDamage; set => plantDamage = value; }
    public float ActionInterval { get => actionInterval; set => actionInterval = value; }
    public float ActionTimer { get => actionTimer; set => actionTimer = value; }
    public Vector2 DettectRange { get => dettectRange; set => dettectRange = value; }
    public DamageType DamageType { get => damageType; set => damageType = value; }
    public float BoostsDuration { get => boostsDuration; set => boostsDuration = value; }
    public int CustToBoost { get => custToBoost; set => custToBoost = value; }
    public List<ClickBoostScriptable> BoostsScriptables { get => boostsScriptables; set => boostsScriptables = value; }
    public PlantAction PlantAction { get => plantAction; set => plantAction = value; }
    public PlantLifeState PlantLifeState { get => plantLifeState; set => plantLifeState = value; }
    public PlantType PlantType { get => plantType; set => plantType = value; }
    public LayerMask ZombieLayer { get => zombieLayer; set => zombieLayer = value; }
    public bool CanRecover { get => canRecover; set => canRecover = value; }
    public bool PlantBoosted { get => plantBoosted; set => plantBoosted = value; }
    #endregion

    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        InitializeComponents();
        DefinePlant();
        ZombieLayer = LayerMask.GetMask("Zombie");
    }

    protected virtual void Update()
    {
        if (PlantLifeState != PlantLifeState.Alive) return;

        DoAction();

        if (PlantAction == PlantAction.Idle) DoIdle();
    }
    #endregion

    #region Creating Plant
    private void DefinePlant()
    {
        MaxHealth = plantSettings.maxHealth;
        CurrentHealth = MaxHealth;

        SunCustToRevive = plantSettings.sunCustToRevive;
        TimeToRevive = plantSettings.timeToRevive;

        PlantAction = PlantAction.Idle;
        PlantLifeState = PlantLifeState.Alive;

        PlantDamage = plantSettings.plantDamage;
        ActionInterval = plantSettings.actionInterval;
        DettectRange = plantSettings.dettectRange;
        DamageType = plantSettings.damageType;

        CanRecover = plantSettings.canRecover;
        PlantType = plantSettings.plantType;

        BoostsScriptables = plantSettings.boostsScriptables;
        BoostsDuration = plantSettings.boostsDuration;
        CustToBoost = plantSettings.custToBoost;
    }

    private void InitializeComponents()
    {
        plantRangeArea = GetComponentInChildren<PlantRangeArea>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        plantLifeHandler = GetComponent<PlantLifeHandler>();
        plantAttackHandler = GetComponent<PlantAttackHandler>();
        plantBoostManager = GetComponent<PlantBoostManager>();
        plantBoxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        if (plantSettings.canRecover)
        {
            plantEffects = GetComponentInChildren<PlantEffects>();
        }

        // Verificação de componentes obrigatórios
        object[] components = new object[] { spriteRenderer, plantLifeHandler, plantAttackHandler, plantBoostManager, plantBoxCollider, anim };
        foreach (var component in components)
        {
            if (component == null)
            {
                Debug.LogWarning($"{component} não encontrado");
            }
        }
    }
    #endregion

    #region Actions
    protected void DoIdle() => setAnimation("transition", 0);

    protected abstract void DoAction();

    public virtual void DoTakeDamage(int amount)
    {
        plantLifeHandler.TakeDamage(amount);
    }

    public virtual void DoDie()
    {
        EndAction();
        plantLifeHandler.Die();
    }

    public void EndAction()
    {
        ActionTimer = 0f;
        PlantAction = PlantAction.Idle;
    }

    public bool IsActionReady()
    {
        ActionTimer += Time.deltaTime;
        return ActionTimer >= ActionInterval;
    }
    #endregion

    #region Boosts
    public virtual void BoostPlant()
    {
        if (PlantBoosted || !SunController.instance.CompareSuns(CustToBoost)) return;

        EventHandler.Instance.CallOnMouseDownPlant(this);
        PlantBoosted = true;
        PlantSpriteManager.PlantBoostedSprite(spriteRenderer, this);
        plantBoostManager.ApplyBoosts(BoostsScriptables);
        SunController.instance.RemoveSun(CustToBoost);
    }

    public virtual void EndBoost()
    {
        PlantBoosted = false;
        PlantSpriteManager.NormalPlantSprite(spriteRenderer, this);
    }

    public virtual void RecoverPlant()
    {
        if (SunController.instance.CompareSuns(SunCustToRevive))
        {
            EventHandler.Instance.CallOnMouseDownPlant(this);
            plantLifeHandler.RecoveringPlant();
            SunController.instance.RemoveSun(SunCustToRevive);
        }
    }
    #endregion

    #region Mouse Interactions
    public virtual void OnMouseDown()
    {
        if (!PlantPositioning.instance.IsPositioningAPlant())
        {
            if (PlantLifeState == PlantLifeState.ReadyToRevive) RecoverPlant();
            if (PlantLifeState == PlantLifeState.Alive) BoostPlant();
        }
    }

    protected virtual void OnMouseEnter()
    {
        if (PlantLifeState == PlantLifeState.ReadyToRevive || PlantLifeState == PlantLifeState.Alive)
        {
            EventHandler.Instance.CallOnMouseEnterPlant(this);
        }
    }

    protected virtual void OnMouseExit()
    {
        if (PlantLifeState == PlantLifeState.ReadyToRevive || PlantLifeState == PlantLifeState.Alive)
        {
            EventHandler.Instance.CallOnMouseExitPlant(this);
        }
    }
    #endregion

    #region Animation
    public void setAnimation(string parameter, int value = 0, string typeParameter = "int")
    {
        if (anim != null)
        {
            switch (typeParameter)
            {
                case "int":
                    anim.SetInteger(parameter, value);
                    break;
                case "trigger":
                    anim.SetTrigger(parameter);
                    break;
            }
        }
    }
    #endregion
}
