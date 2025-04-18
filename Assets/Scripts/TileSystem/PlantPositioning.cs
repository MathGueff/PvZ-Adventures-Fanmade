using UnityEngine;
using System.Collections.Generic;

public class PlantPositioning : MonoBehaviour
{
    [Header("Instance")]
    public static PlantPositioning instance; // Instância singleton do PlantManager

    [Header("Prefabs")]
    public GameObject selectedPlant; // A planta atualmente selecionada pelo jogador
    public GameObject selectedPlantPreview; // A pré-visualização da planta atualmente selecionada

    [Header("SeedController")]
    public PlantPotsController currentSeedController;
    private List<PlantPotsController> totalPots = new List<PlantPotsController>();

    public TileManager tileManager;

    void Awake()
    {
        // Implementa o padrão Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
    }

    public void SelectPlant(int potIndex, PlantPotsController seedController)
    {
        if (potIndex >= 1 && potIndex <= totalPots.Count)
        {
            selectedPlant = seedController.GetPlantPrefab();
            selectedPlantPreview = seedController.GetPlantPreviewPrefab();
            EventHandler.Instance.PlantPreviewChanged();
            currentSeedController = seedController;
            PlantType typePlant = seedController.TypeOfPlant();
            if (tileManager != null)
            {
                tileManager.ShowPlantSpots(typePlant);
            }
        }
        else
        {
            Debug.LogError("Índice de planta inválido!");
        }
    }
    public void DesselectPlant()
    {
        tileManager.HidePlantSpots();
        selectedPlant = null;
        selectedPlantPreview = null;
        currentSeedController = null;
    }

    public void HidePlantSpots()
    {
        if (tileManager != null)
        {
            tileManager.HidePlantSpots();
        }
    }

    public void AddPlantPot(PlantPotsController plantPotController)
    {
        totalPots.Add(plantPotController);
    }

    public bool IsPositioningAPlant() => selectedPlant != null ? true : false;
}
