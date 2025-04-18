using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventHandler : MonoBehaviour
{
    #region  Singleton

    public static EventHandler Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Evita múltiplas instâncias
        }
    }

    #endregion

    #region PlantMouse

    public event Action<Plant> OnMouseEnterPlant;
    public event Action<Plant> OnMouseExitPlant;
    public event Action<Plant> OnMouseDownPlant;

    public void CallOnMouseEnterPlant(Plant plant) => OnMouseEnterPlant?.Invoke(plant);
    public void CallOnMouseExitPlant(Plant plant) => OnMouseExitPlant?.Invoke(plant);
    public void CallOnMouseDownPlant(Plant plant) => OnMouseDownPlant?.Invoke(plant);


    #endregion

    #region ZombieMouse

    public event Action<Zombie> OnMouseDownZombie;
    public event Action<Zombie> OnMouseEnterZombie;
    public event Action<Zombie> OnMouseExitZombie;
    public void CallOnMouseDownZombie(Zombie zombie) => OnMouseDownZombie?.Invoke(zombie);
    public void CallOnMouseEnterZombie(Zombie zombie) => OnMouseEnterZombie?.Invoke(zombie);
    public void CallOnMouseExitZombie(Zombie zombie) => OnMouseExitZombie?.Invoke(zombie);
    #endregion

    #region Flags And Waves
    public event Action<LevelDataScriptable.Wave> OnWaveStarted;
    public event Action<LevelDataScriptable.Wave> OnWaveDefeated;

    public void WaveStarted(LevelDataScriptable.Wave wave)
    {
        OnWaveStarted?.Invoke(wave);
    }

    public void WaveDefeated(LevelDataScriptable.Wave wave)
    {
        OnWaveDefeated?.Invoke(wave);
    }
    #endregion

    #region Plant Position
    public event Action<int> OnPotClicked;
    public event Action OnPlantPlanted;
    public event Action OnPlantPreviewChanged;

    public void PotClicked(int potId)
    {
        OnPotClicked?.Invoke(potId);
    }

    public void PlantPlanted()
    {
        OnPlantPlanted?.Invoke();
    }

    public void PlantPreviewChanged()
    {
        OnPlantPreviewChanged?.Invoke();
    }

    #endregion

    #region Plant Pots
    public event Action<PlantSpot> OnPlantSpotSelected;

    public void SpotSelected(PlantSpot plantSpot)
    {
        OnPlantSpotSelected?.Invoke(plantSpot);
    }
    #endregion

    #region CongaMusic

    public event Action<CongaLeader> OnCongaLeaderSpawn;
    public event Action<CongaLeader> OnCongaLeaderDied;

    public void LeaderCongaSpawned(CongaLeader congaLeader)
    {
        OnCongaLeaderSpawn?.Invoke(congaLeader);
    }

    public void LeaderCongaDied(CongaLeader congaLeader)
    {
        OnCongaLeaderDied?.Invoke(congaLeader);
    }

    #endregion
}
