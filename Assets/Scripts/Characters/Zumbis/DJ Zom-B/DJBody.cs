using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJBody : ZombieBody
{
    private void CallGoldenBite()
    {
        DJZombie dj = z as DJZombie;
        dj.GoldenBite();
    }
}
