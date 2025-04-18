using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    protected Animator anim;
    protected Zombie z;

    [Header("Sorting Layer")]
    protected SpriteRenderer spriteRenderer;
    protected int offSetValue = 0;

    public virtual void Awake()
    {
        anim = GetComponent<Animator>();
        z = GetComponentInParent<Zombie>();
        if (anim != null) z.AddAnimator(anim);
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(spriteRenderer != null)
        {
            ZombieSpriteManager.AddSpriteRenderer(z, spriteRenderer, spriteRenderer.sortingOrder);
        }

        if (anim == null)
            Debug.LogError("Animator não encontrado no VariantBody.");
        if (z == null)
            Debug.LogError("DJZombie não encontrado no objeto pai.");
    }

    public virtual Animator GetAnimator()
    {
        return anim;
    }

    public virtual void DestroyZombie()
    {
        ZombieLifeHandler zombie = GetComponentInParent<ZombieLifeHandler>();  // Acha o script do pai
        if (zombie != null)
        {
            zombie.DestroyZombie();
        }
    }
    
    public virtual void AttackPlant()
    {
        z.zombieAttackHandler.AttackPlant();
    }
}