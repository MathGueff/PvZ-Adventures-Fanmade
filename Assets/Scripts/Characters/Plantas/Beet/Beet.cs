using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beet : Plant
{
    [Header("Zumbis")]
    private Zombie targetZombie;

    [Header("Sound")]
    [SerializeField] private AudioClip attackSound;


    protected override void DoAction()
    {
        plantAttackHandler.DoAttack(DettectRange);
        targetZombie = plantAttackHandler.GetTarget();
    }

    public void DealDamage()
    {
        if (targetZombie == null || targetZombie.ZombieState == ZombieState.Dying)
        {
            EndAction();
            return;
        }
        SoundManager.instance.PlaySound(attackSound);
        plantAttackHandler.AttackZombie(targetZombie);
        EndAction();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta; // Cor do gizmo para visualizar
        Gizmos.DrawWireCube(transform.position, DettectRange);
    }
}
