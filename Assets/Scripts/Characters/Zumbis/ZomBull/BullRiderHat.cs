using System.Collections;
using UnityEngine;

public class BullRiderHat : Armor
{
    private BullRider bullRider;

    public override void Start()
    {
        base.Start();
        bullRider = GetComponentInParent<BullRider>();
        if (spriteRenderer != null && bullRider.ZomBull != null)
            ZombieSpriteManager.AddSpriteRenderer(bullRider.ZomBull, spriteRenderer, spriteRenderer.sortingOrder);
        if (armorAnim != null && bullRider.ZomBull != null)
            bullRider.ZomBull.AddAnimator(armorAnim);
    }

    public void SetAnim(string action)
    {
        armorAnim.SetTrigger(action);
        if(bullRider.ZomBull != null && action == "launch")
        {
            ZombieSpriteManager.RemoveSpriteRenderer(bullRider.ZomBull, spriteRenderer);
        }
    }

    public void RemoveAnimAndSprite()
    {
        ZombieSpriteManager.RemoveSpriteRenderer(bullRider.ZomBull, spriteRenderer);
        bullRider.ZomBull.RemoveAnimator(armorAnim);
    }
}