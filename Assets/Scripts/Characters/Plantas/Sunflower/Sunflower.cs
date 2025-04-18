using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunflower : Plant
{
    [Header("Components")]
    public GameObject prefabSun;

    [Header("Sun Generation")]
    [SerializeField] private int sunGeneration = 25;
    private Vector2 sunPosition;

    protected override void DoAction()
    {
        if (IsActionReady() && PlantAction != PlantAction.Acting)
        {
            ProduceSols();
        }
    }

    void ProduceSols()
    {
        setAnimation("transition", 1);
        PlantAction = PlantAction.Acting;
    }

    void CreateSun()
    {
        // Ajuste para gerar números de ponto flutuante
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);

        sunPosition = new Vector2(transform.position.x + randomX, transform.position.y + randomY);
        GameObject sunCreated = Instantiate(prefabSun, transform.position, Quaternion.identity);
        Sun sunConfig = sunCreated.GetComponent<Sun>();
        sunConfig.setSunAmount(sunGeneration);
        sunConfig.moveSun(sunPosition);
    }

    void endCreateAnimation()
    {
        EndAction();
    }

    public override void BoostPlant()
    {
        if (SunController.instance.CompareSuns(CustToBoost) && !PlantBoosted)
        {
            StunZombies();
        }
        base.BoostPlant();
    }

    private void StunZombies()
    {
        Collider2D[] zombies = plantAttackHandler.CheckZombieNear(DettectRange);
        if(zombies != null && zombies.Length > 0)
        {
            foreach (var zombie in zombies)
            {
                Zombie zScript = zombie.GetComponent<Zombie>();
                if (zScript != null)
                {
                    zScript.SetEffect(Effects.Stun, 3f);
                }
            }
        }    
    }
}
