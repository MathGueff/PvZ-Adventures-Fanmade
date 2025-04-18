using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAttackHandler : MonoBehaviour
{
    private Plant p;
    private Zombie targetZombie;
    private void Awake()
    {
        p = GetComponent<Plant>();
        targetZombie = null;
    }

    #region Attack

    public void DoAttack(Vector2 dettectRange)
    {
        Zombie z = FindTarget(dettectRange, p.plantSettings.typeOfAttack, p.ZombieLayer)?.GetComponent<Zombie>();

        if (z != null && z.ZombieState != ZombieState.Dying)
        {
            DirectionManager.ChangeDirection(gameObject, z.transform, p.anim);
            if (p.IsActionReady() && p.PlantAction != PlantAction.Acting)
            {
                p.PlantAction = PlantAction.Acting;
                targetZombie = z;
                //Animação de Ataque
                p.setAnimation("transition", 1);
            }
        }
    }

    public Zombie GetTarget()
    {
        return targetZombie;
    }

    public void AttackZombie(Zombie zombie)
    {
        zombie.DoTakeDamage(p.PlantDamage, p.DamageType);
    }

    #endregion

    #region Target Dettection
    public Zombie FindTarget(Vector2 dettectRange, DetectAttackType typeOfDetecting, LayerMask enemyLayer)
    {
        // Inicializa arrays para colliders detectados
        Collider2D[] enemiesCollider;

        Vector2 plantPosition = transform.position;

        Collider2D target = null;
        float bestDistance = 0;

        //Definindo o bestDistance para diferentes tipos de detecção
        switch (typeOfDetecting)
        {
            case DetectAttackType.Closer:
                bestDistance = float.MaxValue; //Valor máximo para que o primeiro valor de distância seja o menor
                break;
            case DetectAttackType.DistanceToEnd:
                bestDistance = float.MaxValue;
                break;
        }

        enemiesCollider = Physics2D.OverlapBoxAll(plantPosition, dettectRange, 0f, enemyLayer);

        if (enemiesCollider.Length > 0)
        {
            target = GetEnemy(plantPosition, enemiesCollider, bestDistance, typeOfDetecting);
        }

        return target?.GetComponent<Zombie>();
    }

    public Collider2D GetEnemy(Vector2 plantPosition, Collider2D[] targetColliders, float bestDistance, DetectAttackType typeOfDetecting)
    {
        Collider2D target = null;
        foreach (var targetCollider in targetColliders)
        {
            Zombie zombie = targetCollider.GetComponent<Zombie>();
            if (zombie != null)
            {
                ZombieMovementHandler zombieMovement = zombie.GetComponent<ZombieMovementHandler>();
                if (zombieMovement != null)
                {
                    if(typeOfDetecting == DetectAttackType.Closer)
                    {
                        float distance = Vector2.Distance(plantPosition, targetCollider.transform.position);
                        if (distance < bestDistance)
                        {
                            target = targetCollider;
                            bestDistance = distance;
                        }
                    }
                    else if(typeOfDetecting == DetectAttackType.DistanceToEnd)
                    {
                        float distanceToEnd = zombieMovement.CalculateDistanceToEnd();
                        // Se este alvo está mais distante do que o anterior, atualiza o alvo
                        if (distanceToEnd < bestDistance)
                        {
                            bestDistance = distanceToEnd;
                            target = targetCollider;
                        }
                    }
                }

            }
        }
        return target;
    }

    public Collider2D[] CheckZombieNear(Vector2 size)
    {
        Collider2D[] hitZombies = Physics2D.OverlapBoxAll(transform.position, size, 0f, p.ZombieLayer);
        if (hitZombies.Length > 0)
            return hitZombies;
        return null;
    }
    #endregion
}
