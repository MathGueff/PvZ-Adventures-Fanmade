using System.Collections;
using UnityEngine;

public class ZombieSoundManager : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip[] zombieGrunts; // Sons de grunhido

    [Header("Attributes")]
    public float minInterval = 15f;  // Tempo mínimo entre grunhidos
    public float maxInterval = 30f; // Tempo máximo entre grunhidos
    public float currentTime = 0f;
    public float waitTime = 0f;

    [Header("Components")]
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        waitTime = Random.Range(minInterval, maxInterval);
    }

    private void Update()
    {
        if (GameController.instance.gameState != GameState.Started)
            return;

        if (ZombiesManager.instance.allZombiesAlive.Count > 0)
        {
            if (WaitTimeToPlay())
            {
                PlayRandomZombieGrunt();
                ResetTime();
            }
        }
        else
        {
            currentTime = 0;
        }

    }

    private bool WaitTimeToPlay()
    {
        if (currentTime >= waitTime)
        {
            return true;
        }
        currentTime += Time.deltaTime;
        return false;
    }

    private void ResetTime()
    {
        waitTime = Random.Range(minInterval, maxInterval);
        currentTime = 0;
    }

    private void PlayRandomZombieGrunt()
    {
        AudioClip grunt = zombieGrunts[Random.Range(0, zombieGrunts.Length)];
        audioSource.PlayOneShot(grunt);
    }
}
