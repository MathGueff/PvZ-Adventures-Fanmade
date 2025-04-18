using UnityEngine;

public class VariantHead : Armor
{
    [Header("Attributes")]
    private int halfArmorThreshold;

    public override void Start()
    {
        base.Start();
        halfArmorThreshold = armorHealth / 2;
    }

    public override void Update()
    {
        if (armorState != ArmorState.isFalling && armorAnim != null)
        {
            ChangeAnimations();
            CheckState();
        }
    }

    public override void HeadFall()
    {
        if (armorState != ArmorState.isBreaking)
        {
            SetAnimationTrigger(armorAnim, "isBreaking");
        }
        base.HeadFall();
    }

    public void CheckState()
    {
        if (currentArmorHealth <= halfArmorThreshold && armorState == ArmorState.None)
        {
            armorState = ArmorState.isBreaking;
            SetAnimationTrigger(armorAnim, "isBreaking");

            AnimatorStateInfo stateInfo = z.anim.GetCurrentAnimatorStateInfo(0);
            float currentNormalizedTime = stateInfo.normalizedTime % 1;
            switch (z.ZombieState)
            {
                //Quando estiver em walk e direction down
                case ZombieState.Moving when (int)z.ZombieDirection == 2:
                    armorAnim.Play("WalkBreakingDown", 0, currentNormalizedTime);
                    break;
                //Quando estiver em walk e direction front
                case ZombieState.Moving when (int)z.ZombieDirection == 1:
                    armorAnim.Play("WalkBreakingFront", 0, currentNormalizedTime);
                    break;
                //Quando estiver em walk e direction top
                case ZombieState.Moving when (int)z.ZombieDirection == 0:
                    armorAnim.Play("WalkBreakingTop", 0, currentNormalizedTime);
                    break;
                case ZombieState.Attacking when (int)z.ZombieDirection == 2:
                    armorAnim.Play("EatingBreakingDown", 0, currentNormalizedTime);
                    break;
                case ZombieState.Attacking when (int)z.ZombieDirection == 1:
                    armorAnim.Play("EatingBreakingFront", 0, currentNormalizedTime);
                    break;
                case ZombieState.Attacking when (int)z.ZombieDirection == 0:
                    armorAnim.Play("EatingBreakingTop", 0, currentNormalizedTime);
                    break;
            }
        }
    }
}
