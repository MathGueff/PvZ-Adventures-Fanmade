using System.Collections;
using UnityEngine;

public class ZomBull : Zombie
{
    public float timeToRun;
    public float speedInRun;
    private bool isRunning;
    private BullRider bullRider;

    [Header("Launch")]
    public float launchSpeed;
    public float maxLaunchDistance;

    public override void Start()
    {
        base.Start();
        bullRider = GetComponentInChildren<BullRider>();
        StartCoroutine(WaitToRun());
    }

    public override void Update()
    {
        if (isRunning)
        {
            Plant plant = zombieAttackHandler.FindTarget(ZombieRange, DetectAttackType.Closer, plantLayer, targetType);
            if(plant != null)
            {
                DirectionManager.ChangeDirection(gameObject, plant.transform, anim);
                EndRun();
            }
            else
            {
                if(zombieMovHand.currentWaypoint != null)
                    DirectionManager.ChangeDirection(gameObject, zombieMovHand?.currentWaypoint.transform, anim);
            }
        }
        base.Update();
    }

    public override void DoDie()
    {
        if (bullRider != null)
            bullRider.zombieLifeHandler.DestroyZombie();
        base.DoDie();
    }

    private IEnumerator WaitToRun()
    {
        float elapsedTime = 0f;

        while (ZombieState == ZombieState.Attacking || zombieEffects.IsStunnedOrFrozen())
        {
            yield return null; 
        }

        while (elapsedTime < timeToRun)
        {
            if (ZombieState == ZombieState.Attacking || zombieEffects.IsStunnedOrFrozen())
            {
                yield return new WaitUntil(() => ZombieState != ZombieState.Attacking && !zombieEffects.IsStunnedOrFrozen());
            }
            else
            {
                elapsedTime += Time.deltaTime;
            }

            yield return null; 
        }

        StartRun();
    }


    private void StartRun()
    {
        isRunning = true;
        AddSpeedModifier(speedInRun, "Run");
        targetType = TargetTypePlant.PathPlant;
        anim.SetTrigger("run");
        bullRider.Running();
    }

    private void EndRun()
    {
        isRunning = false;
        RemoveSpeedModifier("Run");
        targetType = TargetTypePlant.All;
        anim.SetTrigger("launch");
        bullRider.Launched();
    }


    //Configurações do Zumbinho para ser lançado
    private void LaunchZombie()
    {
        Vector3 launchPosition = GetLaunchPosition();
        bullRider.InstantiateBullRider(zombieMovHand.waypoints, zombieMovHand.waypointIndex);
        bullRider.StartMoveToTarget(zombieMovHand.currentWaypoint.position, launchPosition, launchSpeed);
        bullRider = null;
    }

    //Pegar a posição para que o zumbi deve ser arremessado
    private Vector3 GetLaunchPosition()
    {
        Vector3 launchPosition = Vector3.zero;
        
        switch (ZombieDirection)
        {
            case CharacterDirection.Top:
                launchPosition = transform.position - new Vector3(0f, maxLaunchDistance);
                break;
            case CharacterDirection.Down:
                launchPosition = transform.position + new Vector3(0f, maxLaunchDistance);
                break;
            case CharacterDirection.Front:
                if (zombieMovHand.CalculateDistanceToWaypoint(zombieMovHand.currentWaypoint.position).x >= 0)
                {
                    launchPosition = transform.position - new Vector3(maxLaunchDistance, 0f);
                }
                else
                {
                    launchPosition = transform.position + new Vector3(maxLaunchDistance, 0f);
                }
                break;
        }

        return launchPosition;
    }
}