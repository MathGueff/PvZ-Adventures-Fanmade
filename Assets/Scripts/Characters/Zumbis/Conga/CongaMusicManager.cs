using System.Collections.Generic;
using UnityEngine;

public class CongaMusicManager : MonoBehaviour
{
    private List<CongaLeader> congaLeadersAlive = new List<CongaLeader>();
    public bool isPlayingConga;
    public AudioClip congaMusic;

    private void Start()
    {
        EventHandler.Instance.OnCongaLeaderSpawn += PlayCongaMusic;
        EventHandler.Instance.OnCongaLeaderDied += EndCongaMusic;
    }

    private void PlayCongaMusic(CongaLeader congaLeader)
    {
        congaLeadersAlive.Add(congaLeader);
        if(congaLeadersAlive.Count > 0 && !isPlayingConga)
        {
            isPlayingConga = true;
            MusicController.instance.PlayMusic(congaMusic);
        }
    }

    private void EndCongaMusic(CongaLeader congaLeader)
    {
        congaLeadersAlive.Remove(congaLeader);
        if (congaLeadersAlive.Count == 0 && isPlayingConga)
        {
            isPlayingConga = false;
            MusicController.instance.ResumeMusic();
        }
    }
}