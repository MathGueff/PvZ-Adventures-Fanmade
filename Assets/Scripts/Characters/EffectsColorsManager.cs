using System.Collections.Generic;
using UnityEngine;

public static class EffectsColorsManager
{
    public static Dictionary<Effects, Color> effectsColors = new Dictionary<Effects, Color>
    {
        {Effects.None, new Color(1f,1f,1f)},
        {Effects.Stun,  new Color(0.5647058f, 0.6392156f, 1f, 1f)},
        {Effects.Freeze,  new Color(0.3915094f, 0.9465144f, 1f, 1f)},
    };

    public static Color GetEffectColor(Effects effect) 
    {
        return effectsColors.TryGetValue(effect, out Color color) ? color : new Color(1f, 1f, 1f, 1f);
    }
}