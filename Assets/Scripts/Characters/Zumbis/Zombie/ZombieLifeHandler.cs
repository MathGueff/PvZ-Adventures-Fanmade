using System.Collections;
using UnityEngine;

public class ZombieLifeHandler : MonoBehaviour
{
    private Zombie z;
    private WaveController waveController;

    public delegate void ZombieDiedHandler(Zombie zombie);
    public event ZombieDiedHandler ZombieDied; // Evento de morte do zumbi

    private void Start()
    {
        waveController = FindObjectOfType<WaveController>();
        ZombieDied += waveController.OnZombieDied;
        ZombieDied += GameController.instance.OnZombieDied;
        z = GetComponent<Zombie>();
    }

    public void TakeDamage(int damage, DamageType damageType)
    {
        float damageTypeModifier = DamageTypeRelations.GetDamageModifier(damageType, z.ArmorType);
        damage = Mathf.RoundToInt(damage * damageTypeModifier * z.GetDamageModifier());

        z.CurrentHealth -= damage;
        if(z.CurrentHealth <= 0 && z.ZombieState != ZombieState.Dying)
        {
            z.DoDie();
        }
    }

    public void Dye()
    {
        if(z.anim != null && z.ZombieState != ZombieState.Dying)
        {
            z.ZombieState = ZombieState.Dying;
            z.anim.SetTrigger("die");
            z.boxCollider.enabled = false;
        }
        else
        {
            DestroyZombie();
        }
    }

    public void DestroyZombie()
    {
        SoundManager.instance.PlaySound(z.deathSound);
        ZombieDied(z); //Evento para identificar morte do zumbi
        Destroy(gameObject);
    }
}
