using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public enum PotAvailable
{
    NotAvailable,
    Available
}

public class PickAPlantController : MonoBehaviour
{
    [Header("Plant Pots")]
    private Dictionary<int, PlantPotsController> plantPots = new Dictionary<int, PlantPotsController>();
    private Dictionary<PlantScriptable, int> PotsOrganization = new Dictionary<PlantScriptable, int>();
    private Dictionary<int, PotAvailable> potsAvailable = new Dictionary<int, PotAvailable>();

    [Header("UI")]
    [SerializeField] private GameObject gameHUDCanvas;
    [SerializeField] private GameObject pickAPlantCanvas;
    [SerializeField] private GameObject confirmButton;

    [Header("Sound")]
    [SerializeField] AudioClip pickingPlantSound;

    #region Unity Methods
    private void Start()
    {
        gameHUDCanvas.SetActive(false);
        pickAPlantCanvas.SetActive(true);
    }

    private void Update()
    {
        if(plantPots != null)
        {
            if (GetNextPotAvailable() != null) //Se houver potes faltando
            {
                confirmButton.SetActive(false);
            }
            else
            {
                confirmButton.SetActive(true);
            }
        }
    }

    #endregion

    #region Plant Pots
    public void DefinePlantPots(int potIndex, PlantPotsController plantPotController)
    {
        potsAvailable.Add(potIndex, PotAvailable.Available);
        plantPots.Add(potIndex, plantPotController);
    }

    public void setPotPlant(PlantScriptable plantScriptable, int potIndex)
    {
        if (potsAvailable[potIndex] == PotAvailable.Available)
        {
            plantPots[potIndex].plantSettings = plantScriptable;
            PotsOrganization.Add(plantScriptable, potIndex);
            potsAvailable[potIndex] = PotAvailable.NotAvailable;
            plantPots[potIndex].InitializePot();
            SoundManager.instance.PlaySound(pickingPlantSound);
        }
    }

    public void RedistributePlantInPots(PlantScriptable plantScriptable)
    {
        foreach (var plantPot in plantPots)
        {
            int plantSelectedIndex = PotsOrganization[plantScriptable];
            if (plantSelectedIndex == plantPot.Key)
            {
                potsAvailable[plantPot.Key] = PotAvailable.Available;
                plantPots[plantPot.Key].RedefinePot();
            }
        }
        PotsOrganization.Remove(plantScriptable);
    }

    public int? GetNextPotAvailable()
    {
        foreach (var pot in potsAvailable)
        {
            if (pot.Value == PotAvailable.Available)
            {
                return (int) pot.Key;
            }
        }
        return null;
    }
    #endregion

    #region Start Game
    public void StartButton()
    {
        GameController.instance.StartGame();
        UpdateCanvas();
    }

    public void UpdateCanvas()
    {
        gameHUDCanvas.SetActive(true);
        pickAPlantCanvas.SetActive(false);
    }

    #endregion

}
