using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ZombieState
{
    None,
    Moving,
    Attacking,
    EspecialAction,
    Dying,
}

public class Zombie : MonoBehaviour
{
    #region Parameters

    private ZombieState zombieState;

    [Header("Attack")]
    private Plant currentEatingPlant;
    protected TargetTypePlant targetType;

    [Header("Life")]
    private int maxHealth;
    private int currentHealth;
    private float damageModifier = 1f;
    private DamageType armorType;

    [Header("Speed")]
    public float defaultSpeed;
    private List<SpeedModifier> speedModifiers = new List<SpeedModifier>();

    [Header("Components")]
    public ZombieScriptable zombieScriptable;
    [HideInInspector] public ZombieAttackHandler zombieAttackHandler;
    [HideInInspector] public ZombieMovementHandler zombieMovHand;
    [HideInInspector] public ZombieEffects zombieEffects;
    [HideInInspector] public ZombieLifeHandler zombieLifeHandler;
    [HideInInspector] public Animator anim;
    [HideInInspector] public List<Animator> animList = new List<Animator>();
    [HideInInspector] public BoxCollider2D boxCollider;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    private Dictionary<SpriteRenderer, int> spriteRendererList = new();

    [Header("Color")]
    [HideInInspector] public Color initialColor;
    [HideInInspector] public Color damageColor;

    [Header("Attributes")]
    private int zombieDamage;
    private Vector2 zombieRange;
    private CharacterDirection zombieDirection = CharacterDirection.Top;
    private CharacterHorizontalDirection zombieHorizontalDirection = CharacterHorizontalDirection.None;

    [Header("Layers")]
    protected LayerMask plantLayer;

    [Header("Sounds")]
    public AudioClip deathSound;

    #region Get & Set
    public Plant CurrentEatingPlant { get => currentEatingPlant; set => currentEatingPlant = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public DamageType ArmorType { get => armorType; set => armorType = value; }
    public float currentSpeed => Mathf.Max(defaultSpeed * speedModifiers.Aggregate(1f, (total, mod) => total * (mod.value <= 1 ? (1 - mod.value) : mod.value)), 0f);

    public CharacterDirection ZombieDirection
    {
        get => zombieDirection;
        set
        {
            if (zombieDirection == value) return;

            zombieDirection = value;
            if (anim != null)
                anim.SetInteger("direction", ((int)zombieDirection));
        }
    }

    public ZombieState ZombieState
    {
        get => zombieState;
        set
        {
            if (zombieState == value) return;

            zombieState = value;
            if (anim != null)
                anim.SetInteger("transition", ((int)zombieState));
        }
    }

    public CharacterHorizontalDirection ZombieHorizontalDirection 
    { 
        get => zombieHorizontalDirection;
        set
        {
            if (zombieHorizontalDirection == value) return;
            zombieHorizontalDirection = value;

            switch (zombieHorizontalDirection)
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


    public Dictionary<SpriteRenderer, int> SpriteRendererList { get => spriteRendererList; set => spriteRendererList = value; }
    public int ZombieDamage { get => zombieDamage; set => zombieDamage = value; }
    public Vector2 ZombieRange { get => zombieRange; set => zombieRange = value; }
   
    public class SpeedModifier
    {
        public float value;
        public string source;
        public SpeedModifier(float value, string source)
        {
            this.value = value;
            this.source = source;
        }
    }
    #endregion

    #endregion

    #region Unity Methods
    public virtual void Awake()
    {
        InitializeComponents();
        DefineZombie();
        plantLayer = LayerMask.GetMask("LawnPlant", "PathPlant");
    }

    public virtual void Start()
    {
        ZombieState = ZombieState.None;
        ZombiesManager.instance.allZombiesAlive.Add(gameObject);
    }

    public virtual void Update()
    {
        if (ZombieState != ZombieState.Dying && !zombieEffects.IsStunnedOrFrozen())
            SelectState();
    }
    #endregion

    #region States
    public virtual void SelectState()
    {
        if (ZombieState == ZombieState.Dying) return;

        Plant targetEnemy = FindTarget();

        // Se houver um alvo válido e ele estiver vivo, ataca.
        if (targetEnemy?.PlantLifeState == PlantLifeState.Alive)
        {
            DoAttack(targetEnemy);
        }
        // Se não houver alvo, começa a se mover.
        else if (targetEnemy == null || targetEnemy.PlantLifeState != PlantLifeState.Alive)
        {
            DoMove();
        }


        ChangeDirectionAnimations();
    }

    public virtual Plant FindTarget()
    {
        return zombieAttackHandler.FindTarget(ZombieRange, DetectAttackType.Closer, plantLayer, targetType);
    }
    #endregion

    #region Actions
    public virtual void DoAttack(Plant targetEnemy)
    {
        if (targetEnemy?.PlantLifeState == PlantLifeState.Alive)
        {
            CurrentEatingPlant = targetEnemy;
            zombieAttackHandler.StartAttack(targetEnemy);
        }
        else
        {
            CurrentEatingPlant = null;
        }
    }

    public virtual void DoMove()
    {
        CurrentEatingPlant = null;
        zombieMovHand?.Walk();
    }

    public virtual void DoTakeDamage(int amount, DamageType damageType)
    {
        zombieLifeHandler.TakeDamage(amount, damageType);
    }

    public virtual void DoDie()
    {
        if (zombieEffects.IsStunnedOrFrozen())
            zombieEffects.DefineEffect(Effects.None);
        zombieLifeHandler.Dye();
    }
    #endregion

    #region Animations
    public virtual void ChangeDirectionAnimations()
    {
        switch (ZombieState)
        {
            case ZombieState.Moving:
                if (zombieMovHand.currentWaypoint != null)
                {
                    DirectionManager.ChangeDirection(gameObject, zombieMovHand?.currentWaypoint.transform, anim);
                }
                break;
            case ZombieState.Attacking:
                if (CurrentEatingPlant != null)
                {
                    DirectionManager.ChangeDirection(gameObject, CurrentEatingPlant?.transform, anim);
                }
                break;
        }
    }
    #endregion

    #region Mouse Events
    private void OnMouseDown()
    {
        if (!PlantPositioning.instance.IsPositioningAPlant() 
            && SunController.instance.CompareSuns(25) 
            && !zombieEffects.IsStunnedOrFrozen())
        {
            SunController.instance.RemoveSun(25);
            SetEffect(Effects.Stun, 3f);
            EventHandler.Instance.CallOnMouseDownZombie(this);
        }
    }

    private void OnMouseEnter() => EventHandler.Instance.CallOnMouseEnterZombie(this);
    private void OnMouseExit() => EventHandler.Instance.CallOnMouseExitZombie(this);
    #endregion

    #region Effects
    public void SetEffect(Effects effect, float time = 0, float amount = 0)
    {
        zombieEffects.DefineEffect(effect, time, amount);
    }

    public void ResetEffect() => zombieEffects.ResetEffects();
    #endregion

    #region Initialization
    private void DefineZombie()
    {
        MaxHealth = zombieScriptable.zombieHealth;
        CurrentHealth = MaxHealth;
        ArmorType = zombieScriptable.armorType;
        defaultSpeed = zombieScriptable.zombieSpeed;
        ZombieDamage = zombieScriptable.zombieDamage;
        ZombieRange = zombieScriptable.zombieRange;
        targetType = zombieScriptable.targetTypePlant;
    }

    private void InitializeComponents()
    {
        zombieAttackHandler = GetComponent<ZombieAttackHandler>();
        zombieMovHand = GetComponent<ZombieMovementHandler>();
        zombieLifeHandler = GetComponent<ZombieLifeHandler>();
        zombieEffects = GetComponent<ZombieEffects>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            ZombieSpriteManager.AddSpriteRenderer(this, spriteRenderer, spriteRenderer.sortingOrder);
        anim = GetComponent<Animator>();
        if(anim != null) AddAnimator(anim);
        boxCollider = GetComponent<BoxCollider2D>();
    }
    #endregion

    #region Lists

    public void AddAnimator(Animator a) => animList.Add(a);

    public void RemoveAnimator(Animator a) => animList.Remove(a);

    public List<Animator> GetAnimatorsList() => animList;
    #endregion

    #region Modifiers
    public void AddSpeedModifier(float value, string source)
    {
        var mod = speedModifiers.Find(m => m.source == source);
        if (mod != null) mod.value = value;
        else speedModifiers.Add(new SpeedModifier(value, source));
    }

    public void RemoveSpeedModifier(string source) => speedModifiers.RemoveAll(m => m.source == source);
    public SpeedModifier GetSpeedModifier(string source) => speedModifiers.Find(m => m.source == source);
    public float GetAllSpeedModifiers() => speedModifiers.Aggregate(1f, (total, mod) => total * mod.value);
    public float GetDamageModifier() => damageModifier;
    public void SetDamageModifier(float modifier) => damageModifier += modifier;
    #endregion
}
