using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TargetTypePlant
{
    All,
    PathPlant,
    LawnPlant
}

public class ZombieAttackHandler : MonoBehaviour
{

    private Zombie z;
    private Plant targetPlant;
    [SerializeField] private AudioClip zombieEatingSound;

    private void Start()
    {
        z = GetComponent<Zombie>();    
    }

    public void StartAttack(Plant plant)
    {
        if (plant == null && plant.PlantLifeState != PlantLifeState.Dying)
        {
            Debug.LogWarning("Tentativa de atacar uma planta nula.");
            z.CurrentEatingPlant = null;
            return;
        }

        z.ZombieState = ZombieState.Attacking;
        targetPlant = plant;
    }

    public void AttackPlant()
    {
        if(targetPlant != null && targetPlant.PlantLifeState != PlantLifeState.Dying)
        {
            SoundManager.instance.PlaySound(zombieEatingSound);
            targetPlant.DoTakeDamage(z.ZombieDamage);
        }
        else
        {
            z.CurrentEatingPlant = null;
        }
    }

    public Plant FindTarget(Vector2 dettectRange, DetectAttackType typeOfDetecting, LayerMask enemyLayer, TargetTypePlant typeToAttack = TargetTypePlant.All)
    {
        List<Collider2D> enemiesColliders = new List<Collider2D>();
        Vector2 zombiePosition = transform.position;

        Collider2D target = null;
        float bestDistance = 0;

        //Definindo o bestDistance para diferentes tipos de detecção
        switch (typeOfDetecting)
        {
            case DetectAttackType.Further:
                bestDistance = float.MinValue; //Valor mínimo para que o primeiro valor de distância seja o maior
                break;
            case DetectAttackType.Closer:
                bestDistance = float.MaxValue; //Valor máximo para que o primeiro valor de distância seja o menor
                break;
        }

        enemiesColliders.AddRange(Physics2D.OverlapBoxAll(zombiePosition, dettectRange, 0f, enemyLayer));

        List<Collider2D> pathPlants = new List<Collider2D>();
        List<Collider2D> lawnPlants = new List<Collider2D>();
        foreach (var collider in enemiesColliders)
        {
            Plant plant = collider.GetComponent<Plant>();
            if(plant.PlantType == PlantType.Path)
            {
                //Adiciona plantas de caminho
                pathPlants.Add(collider);
            }
            else
            {
                //Adiciona plantas de gramado
                lawnPlants.Add(collider);
            }
        }

        //Faz o código para atacar planta de caminho
        if (pathPlants.Count > 0 && (typeToAttack == TargetTypePlant.All || typeToAttack == TargetTypePlant.PathPlant))
        {
            target = GetEnemy(zombiePosition, pathPlants, typeOfDetecting, bestDistance);
            if (target != null)
            {
                Plant plantTarget = target.GetComponent<Plant>();
                if (typeOfDetecting == DetectAttackType.Closer)
                {
                    float distanceX = Mathf.Abs(zombiePosition.x - target.transform.position.x);
                    float distanceY = Mathf.Abs(zombiePosition.y - target.transform.position.y);
                    if (CheckPathPlantDistance(target, zombiePosition))
                    {
                        float tolerance = 1.1f;
                        if ((Mathf.Approximately(distanceX, 0) && distanceY <= tolerance) ||
                                (Mathf.Approximately(distanceY, 0) && distanceX <= tolerance))
                        {
                            return target?.GetComponent<Plant>();
                        }
                    }
                }
            }
        }

        //Faz o código para atacar planta de gramado
        if (lawnPlants.Count > 0 && (typeToAttack == TargetTypePlant.All || typeToAttack == TargetTypePlant.LawnPlant))
        {
            if (z.CurrentEatingPlant != null && z.CurrentEatingPlant.PlantLifeState == PlantLifeState.Alive)
                return z.CurrentEatingPlant;
            target = GetEnemy(zombiePosition, lawnPlants, typeOfDetecting, bestDistance);
            if (target != null)
            {
                if (typeOfDetecting == DetectAttackType.Closer)
                {
                    // Calcula a diferença nas coordenadas X e Y entre alvo e atacante
                    float distanceX = Mathf.Abs(zombiePosition.x - target.transform.position.x);
                    float distanceY = Mathf.Abs(zombiePosition.y - target.transform.position.y);
                    float tolerance = 0.01f;
                    //Verifica se pode atacar na horizontal ou vertical
                    if ((distanceX == 1 && distanceY <= tolerance) || (distanceY == 1 && distanceX <= tolerance))
                    {
                        //Ataque na vertical ou horizontal
                        return target?.GetComponent<Plant>();
                    }
                }
            }
        }
        return null;
    }

    public bool CheckPathPlantDistance(Collider2D target, Vector2 zombiePosition)
    {
        if(z.zombieMovHand.currentWaypoint != null)
        {
            // Calcula a diferença nas coordenadas X e Y entre alvo e atacante
            Vector2 targetPosition = target.transform.position;
            Vector2 waypointPosition = z.zombieMovHand.currentWaypoint.transform.position;

            float distanceZombie = (zombiePosition - waypointPosition).sqrMagnitude;
            float distancePlant = (targetPosition - waypointPosition).sqrMagnitude;

            return distanceZombie >= distancePlant;
        }
        return false;
    }

    public Collider2D GetEnemy(Vector2 zombiePosition, List<Collider2D> targetColliders, DetectAttackType typeOfDetecting, float bestDistance)
    {
        Collider2D target = null;
        foreach (var targetCollider in targetColliders)
        {
            float distance = Vector2.Distance(zombiePosition, targetCollider.transform.position);
            if ((typeOfDetecting == DetectAttackType.Closer && distance < bestDistance) ||
                (typeOfDetecting == DetectAttackType.Further && distance > bestDistance))
            {
                target = targetCollider;
                bestDistance = distance;
            }
        }
        return target;
    }
}
