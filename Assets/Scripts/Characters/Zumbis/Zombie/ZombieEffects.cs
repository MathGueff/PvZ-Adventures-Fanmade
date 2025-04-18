using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effects
{
    None = 0,
    Stun = 1,
    Freeze = 2,
    Slow = 3,
    Burning = 4
}

public class ZombieEffects : MonoBehaviour
{
    [Header("Current Effect")]
    [HideInInspector] public Effects currentEffect;
    private Coroutine currentCoroutine;
    public Effects pendingEffect;

    [Header("Components")]
    private Zombie zombie;
    private Animator effectAnimator;

    [Header("Attributes")]

    [Header("Colors")]
    private Color stunColor;
    private Color freezeColor;
    private Color originalColor;


    private void Start()
    {
        originalColor = EffectsColorsManager.GetEffectColor(Effects.None);
        stunColor = EffectsColorsManager.GetEffectColor(Effects.Stun);
        freezeColor = EffectsColorsManager.GetEffectColor(Effects.Freeze);
        zombie = GetComponent<Zombie>();
        currentEffect = Effects.None;

        effectAnimator = transform.Find("ZombieEffect").GetComponent<Animator>();
    }

    /// <summary>
    /// Applies an effect to the zombie, updating the current Effect enum to the specified effect.
    /// </summary>
    /// <param name="effect">The effect to be applied to the zombie.</param>
    /// <param name="duration">The duration of the effect.</param>
    /// <param name="amount">The intensity of the effect (e.g., burning → damage, freezing → speed reduction).</param>

    #region Effects 
    public void DefineEffect(Effects effect, float duration = 0, float amount = 0)
    {
        if (IsStunnedOrFrozen() && effect == Effects.Slow) return;

        ResetCoroutine(effect);
        currentEffect = effect;
        if(effectAnimator != null) effectAnimator.SetInteger("effect", (int)currentEffect);
        switch (currentEffect)
        {
            case Effects.None:
                NoneEffect();
                break;
            case Effects.Stun:
                StartStun(duration);
                break;
            case Effects.Freeze:
                StartFreeze(duration);
                break;
            case Effects.Slow:
                StartSlow(duration, amount);
                break;
        }
    }

    public void StartPendingEffect(Effects effect, float timeToWait ,float duration = 0, float amount = 0)
    {
        pendingEffect = effect;
        StartCoroutine(WaitToStartPendingEffect(timeToWait, duration, amount));
    }

    private IEnumerator WaitToStartPendingEffect(float timeToWait, float duration = 0, float amount = 0)
    {
        yield return new WaitForSeconds(timeToWait);
        DefineEffect(pendingEffect, duration, amount);
        pendingEffect = Effects.None;
    }

    public void ResetEffects()
    {
        switch (currentEffect)
        {
            case Effects.Stun:
                EndStun();
                break;
            case Effects.Freeze:
                EndFreeze();
                break;
            case Effects.Slow:
                EndSlow();
                break;
        }
    }

    public void ResetCoroutine(Effects nextEffect)
    {
        if (currentCoroutine != null)
        {
            if (currentEffect == Effects.Stun && nextEffect == Effects.Slow) return;

            if (currentEffect != Effects.None)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
                ResetEffects();
            }
        }
    }
    #endregion

    #region None
    private void NoneEffect()
    {
        foreach (var sprite in zombie.SpriteRendererList)
        {
            sprite.Key.color = originalColor;
        }

        foreach (var animator in zombie.animList)
        {
            animator.enabled = true;
            animator.speed = 1;
        }
    }
    #endregion

    #region Stun
    public void StartStun(float duration)
    {
        if (duration == 0)
        {
            Debug.LogError("Duração deve ser passada para o efeito de stun");
        }
        StunEffect();
        currentCoroutine = StartCoroutine(WaitStunToEnd(duration));
    }

    private IEnumerator WaitStunToEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndStun();
    }

    private void EndStun()
    {
        DefineEffect(Effects.None);
    }

    private void StunEffect()
    {
        foreach (var sprite in zombie.SpriteRendererList)
        {
            sprite.Key.color = stunColor;
        }
        if (zombie.ZombieState != ZombieState.Dying)
        {
            foreach (var animator in zombie.animList)
            {
                Debug.Log($"Animator desligado: {animator.name}");
                animator.enabled = false;
            }
        }
    }

    #endregion

    #region Slow

    private void StartSlow(float duration, float speedDecrease)
    {
        if (duration == 0 || speedDecrease == 0)
        {
            Debug.LogError("Duração e redução de velocidade devem ser passados para o efeito de slow");
        }
        SlowEffect(speedDecrease);
        currentCoroutine = StartCoroutine(WaitSlowToEnd(duration));
    }

    private void EndSlow()
    {
        zombie.RemoveSpeedModifier("SlowEffect");
        DefineEffect(Effects.None);
    }

    private void SlowEffect(float speedDecrease)
    {
        Zombie.SpeedModifier currentSlow = zombie.GetSpeedModifier("SlowEffect");
        if (currentSlow == null || speedDecrease > currentSlow.value)
        {
            zombie.AddSpeedModifier(speedDecrease, "SlowEffect");
        }
        //Mudando a cor do sprite para a cor de congelamento
        foreach (var sprite in zombie.SpriteRendererList)
        {
            sprite.Key.color = freezeColor;
        }
        //Alterando a velocidade do Animator
        if (zombie.ZombieState != ZombieState.Dying)
        {
            foreach (var animator in zombie.animList)
            {
                animator.speed = 1 - zombie.GetAllSpeedModifiers();
            }
        }
    }

    private IEnumerator WaitSlowToEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndSlow();
    }

    #endregion

    #region Freeze

    private void StartFreeze(float duration)
    {
        if (duration == 0)
        {
            Debug.LogError("Duração deve ser passada para o efeito de freeze");
        }
        FreezeEffect();
        currentCoroutine = StartCoroutine(WaitFreezeToEnd(duration));
    }

    private IEnumerator WaitFreezeToEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndFreeze();
    }

    private void FreezeEffect()
    {
        foreach (var sprite in zombie.SpriteRendererList)
        {
            sprite.Key.color = freezeColor;
        }
        if (zombie.ZombieState != ZombieState.Dying)
        {
            foreach (var animator in zombie.animList)
            {
                animator.enabled = false;
            }
        }
    }

    private void EndFreeze() => DefineEffect(Effects.None);

    #endregion

    #region Checks

    public bool IsStunnedOrFrozen() => currentEffect == Effects.Freeze || currentEffect == Effects.Stun;

    #endregion
}
