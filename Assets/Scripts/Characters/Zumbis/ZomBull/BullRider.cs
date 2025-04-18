using System.Collections;
using UnityEngine;

public class BullRider : Zombie
{
    public enum BullRiderState
    {
        Riding,
        Flying,
        Walking,
    }

    private ZomBull zomBull;
    private BullRiderHat armor;
    private BullRiderState riderState;

    public ZomBull ZomBull { get => zomBull; set => zomBull = value; }

    public BullRiderState RiderState { get => riderState; set => riderState = value; }

    public override void Start()
    {
        base.Start();
        ZomBull = GetComponentInParent<ZomBull>();
        armor = GetComponentInChildren<BullRiderHat>();
        if(spriteRenderer != null && ZomBull != null)
        ZombieSpriteManager.AddSpriteRenderer(ZomBull, spriteRenderer, spriteRenderer.sortingOrder);
        if (anim != null && ZomBull != null)
            ZomBull.AddAnimator(anim);
    }

    public override void Update()
    {
        if (RiderState != BullRiderState.Walking)
        {
            if (ZomBull != null)
            {
                ZombieDirection = ZomBull.ZombieDirection;
                ZombieState = ZomBull.ZombieState;
            }
            return;
        }
        base.Update();
    }

    public override void DoTakeDamage(int amount, DamageType damageType)
    {
        if(armor.currentArmorHealth > 0)
            armor.TakeDamage(amount, damageType);
        else
            base.DoTakeDamage(amount, damageType);
    }

    public void InstantiateBullRider(Transform[] waypoints, int waypointIndex)
    {
        if (ZomBull != null)
        {
            ZombieSpriteManager.RemoveSpriteRenderer(ZomBull, spriteRenderer);
            ZomBull.RemoveAnimator(anim);
            armor.RemoveAnimAndSprite();
        }
        transform.SetParent(null);
        boxCollider.enabled = true;
        zombieMovHand.waypoints = waypoints;
        zombieMovHand.waypointIndex = waypointIndex;
    }

    public void StartMoveToTarget(Vector3 waypointTarget, Vector3 launchTarget, float launchingSpeed) => 
        StartCoroutine(MoveToTarget(waypointTarget, launchTarget, launchingSpeed)); 

    private IEnumerator MoveToTarget(Vector3 waypointTarget, Vector3 launchTarget, float launchingSpeed)
    {
        // Enquanto a distância for maior que 0.2f e a distância do waypoint for maior do que 0.2
        while (Vector3.Distance(transform.position, launchTarget) >= 0.2f 
            && Vector3.Distance(transform.position, waypointTarget) >= 0.2f)
        {
            // Move o objeto lentamente em direção ao alvo
            transform.position = Vector3.MoveTowards(transform.position, launchTarget, launchingSpeed * Time.deltaTime);
            yield return null; // Espera o próximo frame para continuar o movimento
        }
        Falled();
    }

    public void Running()
    {
        anim.SetTrigger("run");
        armor.SetAnim("run");
    }

    public void Launched()
    {
        RiderState = BullRiderState.Flying;
        anim.SetTrigger("launch");
        armor.SetAnim("launch");
    }

    private void Falled()
    {
        RiderState = BullRiderState.Walking;
        anim.SetTrigger("fall");
        armor.SetAnim("fall");
    }
}