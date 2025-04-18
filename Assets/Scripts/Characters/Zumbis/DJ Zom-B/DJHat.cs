using UnityEngine;

public class DJHat : Armor
{
    public override void TakeDamage(int amount, DamageType damageType)
    {
        DJZombie dj = z as DJZombie;
        float modifier = DamageTypeRelations.GetDamageModifier(damageType, armorType);
        amount = Mathf.RoundToInt(amount * modifier);
        currentArmorHealth -= amount;
        if (currentArmorHealth <= 0)
        {
            int overflowDamage = Mathf.Abs(currentArmorHealth);
            if (armorType != z.ArmorType && dj.currentHatIndex < 0)
            {
                overflowDamage = Mathf.RoundToInt(overflowDamage / modifier);
            }
            dj.currentHatIndex--;
            z.DoTakeDamage(overflowDamage, damageType);
            HeadFall();
            if (dj.currentHatIndex < 0)
            {
                z.ZombieState = ZombieState.EspecialAction;
            }
        }
    }
}
