using System.Collections;
using UnityEngine;

public class PlantLifeHandler : MonoBehaviour
{

    private Plant p;

    private void Start()
    {
        p = GetComponent<Plant>();
    }

    #region Life
    public void TakeDamage(int damage)
    {
        if(p != null)
        {
            int finalDamage = Mathf.RoundToInt(Mathf.Max(1, damage * p.DamageReceivedModifier));
            p.CurrentHealth -= finalDamage;
            if (p.CurrentHealth <= 0 && p.PlantLifeState != PlantLifeState.Dying)
            {
                p.PlantLifeState = PlantLifeState.Dying;
                p.DoDie();
            }
        }
    }

    public void Die()
    {
        if (p.CanRecover)
        {
            if (p.PlantLifeState == PlantLifeState.Dying)
            {
                p.PlantLifeState = PlantLifeState.Reviving;
                p.spriteRenderer.color = new Color(0.8745098f, 0.5960785f, 1f, 1f);
                StartCoroutine(PlantDeath(p.TimeToRevive));
            }
        }
        else
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }

    #endregion

    #region Death Mechanics

    // Método para mudar a cor da planta temporariamente
    private IEnumerator PlantDeath(float duration)
    {
        p.plantEffects.ChangeEffectState(EffectState.Dying);
        gameObject.layer = LayerMask.NameToLayer("RecoveringPlant"); //Muda a layer para que o zumbi nao ataque mais
        p.setAnimation("dying", typeParameter: "trigger"); //Animação Dying

        yield return new WaitForSeconds(duration); //Tempo para disponibilizar o click na planta

        p.PlantLifeState = PlantLifeState.ReadyToRevive;
        p.plantEffects.ChangeEffectState(EffectState.ReadyToRevive);
    }

    public void RecoveringPlant()
    {
        p.PlantLifeState = PlantLifeState.Reviving;
        p.setAnimation("recovering", typeParameter: "trigger"); //Animação recovering
    }

    //Chamado durante a animação de revivendo
    private void RevivePlant()
    {
        if (p.PlantLifeState == PlantLifeState.Reviving)
        {
            p.plantEffects.ChangeEffectState(0);
            p.ActionTimer = 0f;
            p.PlantLifeState = PlantLifeState.Alive;
            p.CurrentHealth = p.MaxHealth;
            gameObject.layer = LayerMask.NameToLayer("LawnPlant");
            p.setAnimation("transition", 0);
            p.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    #endregion
}
