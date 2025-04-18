using System.Collections;
using UnityEngine;

public class IceBlock : Plant
{
    public Zombie freezedZombie;

    [Header("Freeze")]
    private int freezeDamage; //Dano do congelamento
    private float freezeInterval; //Tempo entre dano de congelamento

    [Header("Slow")]
    private float slowEffectDuration; //Duração do slow
    private float animatorSlow; //Duração do slow
    private float speedSlow; //Diminuição de velocidade

    [Header("AutoDeath")]
    private float autoDeathTimer; //Timer para quando apenas o zumbi congelado restar vivo
    private float autoDeathInterval;

    protected override void Awake()
    {
        PlantType = PlantType.Path;
        autoDeathInterval = 3;
    }

    protected override void Update()
    {
        if (ZombiesManager.instance.allZombiesAlive.Count <= 0 && ZombiesManager.instance.CheckIfAllZombiesSpawned())
        {
            CheckIfIsLastZombie();
        }
    }

    #region Overrides inúteis


    protected override void DoAction()
    {

    }

    protected override void OnMouseEnter()
    {
        
    }

    protected override void OnMouseExit()
    {
        
    }

    public override void OnMouseDown()
    {
        
    }

    #endregion

    public void InitializeIceBlock(Zombie zombie, int damage, float interval, float slowDuration, float animatorSlow, float speedSlow)
    {
        freezedZombie = zombie;
        MaxHealth = zombie.MaxHealth;
        CurrentHealth = MaxHealth;
        freezeDamage = damage;
        freezeInterval = interval;
        slowEffectDuration = slowDuration;
        this.animatorSlow = animatorSlow;
        this.speedSlow = speedSlow;
        StartCoroutine(FreezingZombie());
    }

    public override void DoTakeDamage(int amount)
    {
        CurrentHealth -= amount;
        ChangeState();
    }

    private void ChangeState()
    {
        if (CurrentHealth >= MaxHealth * (2f / 3f))
        {
            //Normal
        }
        else if (CurrentHealth >= MaxHealth * (1f / 3f))
        {
            //Danificado
        }
        else if (CurrentHealth > 0)
        {
            //Quebrando
        }
        else
        {
            BreakIceBlock();
        }
    }

    private IEnumerator FreezingZombie()
    {
        while(freezedZombie.ZombieState != ZombieState.Dying && CurrentHealth > 0 && freezedZombie != null)
        {
            freezedZombie.DoTakeDamage(freezeDamage, DamageType.Ice);
            yield return new WaitForSeconds(freezeInterval);
        }
    }

    private void CheckIfIsLastZombie()
    {
        autoDeathTimer += Time.deltaTime;
        if (autoDeathTimer >= autoDeathInterval)
        {
            if (freezedZombie != null)
            {
                freezedZombie.DoTakeDamage(freezedZombie.CurrentHealth, DamageType.Normal); 
            }
            BreakIceBlock();
        }
    }

    private void BreakIceBlock()
    {
        if(freezedZombie != null)
        {
            freezedZombie.boxCollider.enabled = true;
            freezedZombie.ResetEffect();
            //if (freezedZombie.zombieState != ZombieState.Dying)
                //freezedZombie.SetEffect(Effects.Slow, time: slowEffectDuration, animatorSpeed: animatorSlow, amount: speedSlow);
        }
        Destroy(gameObject);
    }
}