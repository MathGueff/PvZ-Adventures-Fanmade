using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Flag : MonoBehaviour
{
    public event Action OnPassed;
    private Animator anim;
    public LevelDataScriptable.Wave waveOfFlag;

    private void Start()
    {
        anim = GetComponent<Animator>();
        EventHandler.Instance.OnWaveStarted += Started; 
        EventHandler.Instance.OnWaveDefeated += Defeated; 
    }

    public void SetWaveOfFlag(LevelDataScriptable.Wave wave)
    {
        waveOfFlag = wave;
    }

    private void Started(LevelDataScriptable.Wave wave)
    {
        if(wave == this.waveOfFlag && ZombiesManager.WavesInLevel[wave] == WaveState.Started)
        {
            anim.SetInteger("transition", 1);
        }
    }


    private void Defeated(LevelDataScriptable.Wave wave)
    {
        if(wave == this.waveOfFlag && ZombiesManager.WavesInLevel[wave] == WaveState.Ended)
        {
            anim.SetInteger("transition", 2);
        }
    }

    private void DestroyFlag()
    {
        gameObject.SetActive(false);
    }
}
