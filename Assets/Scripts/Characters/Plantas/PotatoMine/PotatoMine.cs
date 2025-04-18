using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoMine : Plant
{
    [Header("Attack")]
    public Vector2 explosionSize;
    private bool isExploded = false;

    [Header("Sound")]
    [SerializeField] AudioClip explosionSound;

    protected override void Update()
    {
        DoAction();
    }

    protected override void DoAction()
    {
        if (IsActionReady())
        {
            //Se o tempo de armar tiver sido alcançado

            gameObject.layer = LayerMask.NameToLayer("NoCollisionPlant");
            if (plantAttackHandler.CheckZombieNear(DettectRange + new Vector2(1.5f, 1.5f)) != null && !isExploded)
            {
                //Planta detecta zumbi se aproximando mas distante da sua área de ataque
                setAnimation("transition", 4);
            }
            else
            {
                if (!isExploded)
                {
                    //Retorna ao idle caso perca de vista o zumbi ou não tenha visualizado um zumbi ainda
                    setAnimation("transition", 2);
                }
            }
            if (plantAttackHandler.CheckZombieNear(DettectRange) != null && !isExploded)
            {
                //Planta explode
                isExploded = true;
                setAnimation("transition", 3);
            }
        }
        //Muda para segunda versão
        else if (ActionTimer >= ActionInterval / 2)
        {
            setAnimation("transition", 1);
        }
    }

    private void ExplodeZombies()
    {
        SoundManager.instance.PlaySound(explosionSound);
        Collider2D[] hitZombies = plantAttackHandler.CheckZombieNear(explosionSize);
        foreach (Collider2D zombieCollider in hitZombies)
        {
            Zombie zombie = zombieCollider.GetComponent<Zombie>();
            if(zombie != null)
            {
                plantAttackHandler.AttackZombie(zombie);
            }
        }
    }

    private void DestroyPotato()
    {
        Destroy(gameObject);
    }

    public override void BoostPlant()
    {
        if (SunController.instance.CompareSuns(CustToBoost) && !PlantBoosted)
        {
            ActionTimer += 5;
        }
        base.BoostPlant();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, DettectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, explosionSize);
    }
}
