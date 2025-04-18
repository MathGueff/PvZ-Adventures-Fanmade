using System.Collections.Generic;

public enum DamageType
{
    Normal,
    Fire,
    Ice
}


public static class DamageTypeRelations
{
    //Dicionário com tipo de dano de ataque e de armadura, e respectivo modificador contra
    public static readonly Dictionary<(DamageType attacker, DamageType defender), float> damageRelations = 
        new Dictionary<(DamageType, DamageType), float>
        {
            {(DamageType.Fire, DamageType.Ice), 2},
            {(DamageType.Ice, DamageType.Fire), 2},
        };  

    public static float GetDamageModifier(DamageType attacker, DamageType defender)
    {
        return damageRelations.TryGetValue((attacker, defender), out float modifier) ? modifier : 1f;
    }
}