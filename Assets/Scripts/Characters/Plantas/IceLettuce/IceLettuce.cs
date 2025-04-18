using System.Collections;
using UnityEngine;

public class IceLettuce : Plant
{
    [Header("Freeze")]
    [SerializeField] private Zombie freezedZombie;
    public float freezeDuration;
    public float freezeTimer;
    public AudioClip freezeSound;

    [Header("Freezed Zombie Slow")]
    public float freezedZombieSlowSpeed;
    public float freezedZombieSlowDuration;

    [Header("Slow")]
    public float slowZombieSpeed;
    public float slowDuration;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") && freezedZombie == null && Vector2.Distance(collision.transform.position, transform.position) <= 0.3f)
        {
            Zombie zombie = collision.GetComponent<Zombie>();
            freezedZombie = zombie;
            PlantAction = PlantAction.Acting;
            DoAction();
        }
    }

    protected override void Update()
    {
        if(PlantAction == PlantAction.Acting)
        {
            freezeTimer += Time.deltaTime;
            if(freezeTimer >= freezeDuration)
            {
                DoDie();
            }
        }
        else
        {
            base.Update();
        }
    }

    protected override void DoAction()
    {
        if(freezedZombie != null)
            setAnimation("transition", 1); //Animação de congelamento do zumbi
    }

    //Chamado após a animãção "Acting"
    private void FreezeZombie()
    {
        if (freezedZombie != null && PlantAction == PlantAction.Acting)
        {
            SoundManager.instance.PlaySound(freezeSound);
            freezedZombie.SetEffect(Effects.Freeze, freezeDuration);
            SlowFreezedZombie();
            plantBoxCollider.enabled = false;
        }
    }

    private void SlowFreezedZombie()
    {
        if(freezedZombie != null)
            freezedZombie.zombieEffects.StartPendingEffect
                (Effects.Slow, freezeDuration, freezedZombieSlowDuration, freezedZombieSlowSpeed);
    }

    //Chamado após a animãção "Acting"
    private void ApplySlowInArea()
    {
        Collider2D[] hitZombies = plantAttackHandler.CheckZombieNear(DettectRange) ?? new Collider2D[0];
        if(hitZombies.Length > 0)
        {
            foreach (Collider2D zombieCollider in hitZombies)
            {
                Zombie zombie = zombieCollider.GetComponent<Zombie>();

                if(freezedZombie != null && zombie != null && zombie == freezedZombie) continue;

                if (zombie != null)
                {
                    zombie.SetEffect(Effects.Slow, slowDuration, slowZombieSpeed);
                }
            }
        }
    }

    public override void BoostPlant()
    {
        if (SunController.instance.CompareSuns(CustToBoost)) freezeDuration += 4;
        base.BoostPlant();
    }

    public override void EndBoost()
    {
        if(freezedZombie == null) freezeDuration -= 4;
        base.EndBoost();
    }
}