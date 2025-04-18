using UnityEngine;

public class PlantSpot : MonoBehaviour
{
    public PlantType positionType;

    [Header("Prefabs")]
    [SerializeField] private GameObject plantPreview; // Pré-visualização da planta

    [Header("Components")]
    private SunController sunController;
    [SerializeField] public Plant plant;

    [Header("Sprite")]
    private SpriteRenderer spriteRenderer;
    private Sprite spriteTileGreen;
    private Sprite spriteTileEmpty;

    [Header("Layer")]
    private LayerMask plantLayer;

    [Header("Sound")]
    [SerializeField] private AudioClip plantingSound;

    public bool isMouseOver = false;

    private void Start()
    {
        sunController = FindObjectOfType<SunController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteTileEmpty = Resources.Load<Sprite>("Sprites/tileVazio");
        spriteTileGreen = Resources.Load<Sprite>("Sprites/tileSelecionado");
        plantLayer = LayerMask.GetMask("LawnPlant", "RecoveringPlant", "PathPlant", "NoCollisionPlant");
        EventHandler.Instance.OnPlantPreviewChanged += UpdatePlantPreview;
        EventHandler.Instance.OnPlantSpotSelected += DesselectOtherSpots;
    }

    private void Update()
    {
        Plant detectedPlant = DetectPlant();
        if (detectedPlant != null)
        {
            plant = detectedPlant;
        }
        else if(plantPreview == null)
        {
            plant = null;
            spriteRenderer.sprite = spriteTileEmpty;
        }
    }

    #region Mouse Detecction
    void OnMouseEnter()
    {
        isMouseOver = true;
        // Verifica se o spot está ocupado e se uma planta foi selecionada
        if (DetectPlant() == null && PlantPositioning.instance.selectedPlantPreview != null)
        {
            InstantiatePlantPreviewInSpot();
            EventHandler.Instance.SpotSelected(this);
        }
    }
    void OnMouseDown()
    {
        isMouseOver = false;
        // Verifica se o spot está ocupado e se uma planta foi selecionada
        if (DetectPlant() == null && PlantPositioning.instance.selectedPlant != null)
        {
            InstantiatePlant();
        }
    }

    void OnMouseExit()
    {
        isMouseOver = false;
        DestroyPlantPreview();
    }
    #endregion

    #region Plant
    private void InstantiatePlantPreviewInSpot()
    {
        // Instancia a pré-visualização da planta
        if (plantPreview == null)
        {
            plantPreview = Instantiate(
                PlantPositioning.instance.selectedPlantPreview,
                transform.position,
                Quaternion.identity
            );

            plantPreview.transform.SetParent(transform); // Define o pai como o spot para facilitar a movimentação
            plantPreview.name = "PlantPreview"; // Nome para fácil identificação
            spriteRenderer.sprite = spriteTileGreen;
        }
    }

    private void UpdatePlantPreview()
    {
        if(plantPreview != null && isMouseOver)
        {
            DestroyPlantPreview();
            plantPreview = null;
            InstantiatePlantPreviewInSpot();
        }
    }

    private void DesselectOtherSpots(PlantSpot plantSpot)
    {
        if (this != plantSpot && plantPreview != null)
        {
            DestroyPlantPreview();
            plantPreview = null;
        }
    }

    private void DestroyPlantPreview()
    {
        // Remove a pré-visualização quando o mouse sai
        if (plantPreview != null)
        {
            Destroy(plantPreview);
            spriteRenderer.sprite = spriteTileEmpty;
        }
    }

    private void InstantiatePlant()
    {
        SoundManager.instance.PlaySound(plantingSound);
        GameObject newPlant = Instantiate(
            PlantPositioning.instance.selectedPlant,
            transform.position,
            Quaternion.identity
        );

        plant = newPlant.GetComponent<Plant>();

        sunController.RemoveSun(PlantPositioning.instance.currentSeedController.ReturnSunCust());
        PlantPositioning.instance.currentSeedController.UpdateSeedStatus();

        if (plantPreview != null)
        {
            Destroy(plantPreview);
        }

        // Checa se a planta foi plantada com sucesso
        if (DetectPlant())
        {
            gameObject.SetActive(false);
            PlantPositioning.instance.DesselectPlant();
            PlantPositioning.instance.HidePlantSpots();
            EventHandler.Instance.PlantPlanted();
        }
    }

    public Plant DetectPlant()
    {
        Vector2 boxSize = new Vector2(.5f, .5f);
        Collider2D isOccupied = Physics2D.OverlapBox(transform.position, boxSize, 0f, plantLayer);
        if (isOccupied != null)
        {
            Plant plantInSpot = isOccupied.GetComponent<Plant>();
            if (plantInSpot != null)
            {
                return plantInSpot;
            }
        }
        return null;
    }
    #endregion
}
