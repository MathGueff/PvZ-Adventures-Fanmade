using UnityEngine;
public enum ArmorState
{
    None,
    isFalling,
    isBreaking
}

public class Armor : MonoBehaviour
{
    [Header("Components")]
    protected Animator armorAnim;
    protected Zombie z;

    [Header("Attributes")]
    protected int armorHealth;
    public int currentArmorHealth;
    public DamageType armorType;
    public int armorIndex = 0;

    [Header("State")]
    public ArmorState armorState = ArmorState.None;

    [Header("Sorting Layer")]
    protected SpriteRenderer spriteRenderer;

    public virtual void Start()
    {
        armorAnim = GetComponent<Animator>();
        z = GetComponentInParent<Zombie>();
        if (armorAnim != null) z.AddAnimator(armorAnim);

        DefineArmor();

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            ZombieSpriteManager.AddSpriteRenderer(z, spriteRenderer, spriteRenderer.sortingOrder);
        }
    }

    public virtual void Update()
    {
        if (armorState != ArmorState.isFalling && armorAnim != null)
        {
            ChangeAnimations();
        }
    }

    public void DefineArmor()
    {
        armorHealth = z.zombieScriptable.armorsHealth[armorIndex];
        currentArmorHealth = armorHealth;
    }

    public virtual void ChangeAnimations()
    {
        if (armorAnim == null)
        {
            Debug.LogWarning("Armor anim não encontrado");
            return;
        }

        if (z.ZombieState == ZombieState.Dying)
        {
            HeadFall();
            return;
        }
        armorAnim.SetInteger("direction", ((int)z.ZombieDirection));
        armorAnim.SetInteger("transition", ((int)z.ZombieState));
    }

    public virtual void TakeDamage(int damage, DamageType damageType)
    {
        float damageTypeModifier = DamageTypeRelations.GetDamageModifier(damageType, armorType);
        damage = Mathf.RoundToInt(damage * damageTypeModifier * z.GetDamageModifier());

        currentArmorHealth -= damage;

        if (currentArmorHealth <= 0)
        {
            int overflowDamage = Mathf.Abs(currentArmorHealth);
            if (armorType != z.ArmorType)
            {
                overflowDamage = Mathf.RoundToInt(overflowDamage / damageTypeModifier);
            }
            z.DoTakeDamage(overflowDamage, damageType);
            HeadFall();
        }
    }

    public virtual void HeadFall()
    {
        armorState = ArmorState.isFalling;
        SetAnimationTrigger(armorAnim, "falling");
    }

    public virtual void DestroyArmor()
    {
        gameObject.SetActive(false);
    }

    public virtual void SetAnimationTrigger(Animator animator, string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }
}