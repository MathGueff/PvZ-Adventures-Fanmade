using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallNut : Plant
{
    [Header("HealthStates")]
    private float plantMaxHealth;
    private float FirstHealthCondition;
    private float SecondHealthCondition;
    private float ThirdHealthCondition;

    private void Start()
    {
        plantMaxHealth = MaxHealth;
        FirstHealthCondition = plantMaxHealth / 4 * 3;
        SecondHealthCondition = plantMaxHealth / 2;
        ThirdHealthCondition = plantMaxHealth / 4;
    }

    protected override void Update()
    {
        if(PlantLifeState != PlantLifeState.Alive)
            return;
        
        DoAction();
    }

    protected override void DoAction()
    {
        if (CurrentHealth <= FirstHealthCondition && CurrentHealth > SecondHealthCondition)
        {
            setAnimation("transition", 1);
        }
        else if (CurrentHealth <= SecondHealthCondition && CurrentHealth > ThirdHealthCondition)
        {
            setAnimation("transition", 2);
        }
        else if (CurrentHealth <= ThirdHealthCondition)
        {
            setAnimation("transition", 3);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // Cor do gizmo para visualizar
        Gizmos.DrawWireCube(transform.position, DettectRange);
    }
}
