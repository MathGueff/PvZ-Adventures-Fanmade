using System.Collections;
using UnityEngine;

public class CongaDancer : Zombie
{
    public CongaLeader congaLeader;
    public bool LeaderDead;

    private CongaDancerBody body;
    private CongaDancerHat armor;

    public override void Awake()
    {
        base.Awake();
        body = GetComponentInChildren<CongaDancerBody>();
        armor = GetComponentInChildren<CongaDancerHat>();
    }

    public override void Start()
    {
        base.Start();
        Animator bodyAnim = body.GetAnimator();

        if (bodyAnim != null)
        {
            anim = bodyAnim;
        }
        else
        {
            Debug.LogWarning("Animator do body não encontrado");
        }
        AddSpeedModifier(0.3f, "LeaderAlive");
    }

    public override void Update()
    {
        if (ZombieState == ZombieState.Dying)
            return;

        if((congaLeader == null || congaLeader.ZombieState == ZombieState.Dying) && !LeaderDead)
        {
            LeaderDied();
        }

        if(congaLeader != null && (congaLeader.ZombieState == ZombieState.EspecialAction || congaLeader.ZombieState == ZombieState.Attacking))
        {
            Dance();
        }
        else
        {
            base.Update();
        }
    }

    public override void DoTakeDamage(int amount, DamageType damageType)
    {
        if(armor.currentArmorHealth > 0)
        {
            armor.TakeDamage(amount, damageType);
        }
        else
        {
            base.DoTakeDamage(amount, damageType);
        }
    }

    private void Dance()
    {
        ZombieState = ZombieState.EspecialAction;
    }

    private void LeaderDied()
    {
        LeaderDead = true;
        RemoveSpeedModifier("LeaderAlive");
    }

    public void SetWaypointDefinitions(Transform[] waypoints)
    {
        zombieMovHand.SetWaypoints(waypoints);
    }

    public void SetLeader(CongaLeader congaLeader)
    {
        this.congaLeader = congaLeader;
    }
}