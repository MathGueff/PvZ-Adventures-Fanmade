using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEffects : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ChangeEffectState(EffectState state)
    {
        int transitionValue = (int)state;
        anim.SetInteger("transition", transitionValue);
    }
}
