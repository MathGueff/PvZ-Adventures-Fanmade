using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownCoatVariant : Zombie
{
    [Header("Components")]
    private VariantBody body;
    private VariantHead head;

    public override void Awake()
    {
        base.Awake();
        body = GetComponentInChildren<VariantBody>();
        head = GetComponentInChildren<VariantHead>();
    }

    public override void Start()
    {
        base.Start();
        Animator bodyAnim = body.GetAnimator();

        if (bodyAnim != null)
        {
            anim = bodyAnim;
        }
        else
        {
            Debug.LogWarning("Animator do body não encontrado");
        }
    }

    public override void DoTakeDamage(int amount, DamageType damageType)
    {
        if(head.currentArmorHealth > 0)
        {
            head.TakeDamage(amount, damageType);
        }
        else
        {
            base.DoTakeDamage(amount, damageType);
        }
    }
}
