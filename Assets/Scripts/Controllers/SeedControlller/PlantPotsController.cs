using UnityEngine;
using UnityEngine.EventSystems; // Importar este namespace para UI events
using TMPro;
using UnityEngine.UI;

public enum potState
{
    PotInitialized,
    PotNotInitialized,
    PotSelected,
    PotNotSelected
}

public class PlantPotsController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Variables

    [Header("Plant Settings")]
    public PlantScriptable plantSettings;

    [Header("State")]
    [HideInInspector] public potState potState = potState.PotNotInitialized;
    [HideInInspector] public potState potSelection = potState.PotNotSelected;

    [Header("Cooldown")]
    private float seedCooldown;
    private float currentCooldown = 0;

    [Header("Quantity")]
    private int maxSeedQuantity = 5;
    private int currentSeedQuantity;

    [Header("Sun")]
    private int sunCust;
    private int currentSuns;

    [Header("Info")]
    private string plantName;
    [HideInInspector] public int potIndex;

    [Header("Components")]
    private PlantPositioning plantPositioning;
    private SunController sunController;
    private Animator anim;

    [Header("Text")]    
    public TextMeshProUGUI seedQuantityText;
    public TextMeshProUGUI sunCustText;
    public TextMeshProUGUI cooldownText;

    [Header("Image")]
    public Image UIplantImage;
    public Image UIplantPreviewImage;

    [Header("Prefabs")]
    private GameObject plantPrefab;
    private GameObject plantPreviewPrefab;

    [Header("Type")]
    private PlantType typePlant;

    [Header("Sound")]
    [SerializeField] private AudioClip invalidToSelectSound;

    #endregion

    #region Unity Methods
    private void Awake()
    {
        plantPositioning = FindObjectOfType<PlantPositioning>();
        sunController = FindObjectOfType<SunController>();
    }

    private void Start()
    {
        EventHandler.Instance.OnPotClicked += ResetState;
        EventHandler.Instance.OnPlantPlanted += ResetAllPotsState;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(GameController.instance.gameState == GameState.Started)
        {
            if (potState == potState.PotInitialized)
            {
                if(Input.GetKeyDown(KeyCode.Alpha0 + potIndex))
                {
                    SelectPot();
                }
                currentSuns = sunController.totalSun;
                UpdateUI();
                if (currentCooldown < seedCooldown)
                {
                    anim.SetBool("seedNotReady", true);
                    currentCooldown += Time.deltaTime;
                }
                else
                {
                    anim.SetBool("seedNotReady", false);
                }
            }
        }
    }
    #endregion

    #region UpdateUI
    private void UpdateUI()
    {
        if (currentSeedQuantity <= 0)
        {
            anim.SetTrigger("seedsEnded");
        }
        UIplantImage.fillAmount = currentCooldown / seedCooldown;
        if (sunCust > currentSuns)
        {
            sunCustText.color = Color.red;
        }
        else
        {
            sunCustText.color = Color.white;
        }
        seedQuantityText.text = "x" + currentSeedQuantity.ToString();
        cooldownText.text = Mathf.Round(seedCooldown - currentCooldown).ToString();
    }
    #endregion

    #region Pointer
    public void OnPointerClick(PointerEventData eventData)
    {
        SelectPot();
    }

    public void SelectPot()
    {
        if (GameController.instance.gameState == GameState.Started)
        {
            if (currentSeedQuantity > 0)
            {
                bool cooldownIsOver = currentCooldown >= seedCooldown;
                bool hasSeeds = currentSeedQuantity > 0;
                bool hasSun = currentSuns >= sunCust;

                if (!cooldownIsOver)
                    InvalidToSelect("A semente ainda está carregando!");
                if (!hasSeeds)
                    InvalidToSelect("As sementes acabaram!");
                if (!hasSun)
                    InvalidToSelect("Você não tem sóis suficientes!");

                if (cooldownIsOver && hasSun && hasSeeds)
                {
                    if (potSelection == potState.PotNotSelected)
                    {
                        EventHandler.Instance.PotClicked(potIndex);
                        potSelection = potState.PotSelected;
                        anim.SetInteger("transition", 1);
                        plantPositioning.SelectPlant(potIndex, this);
                    }
                    else
                    {
                        potSelection = potState.PotNotSelected;
                        anim.SetInteger("transition", 0);
                        plantPositioning.DesselectPlant();
                    }
                }
            }
            else
            {
                InvalidToSelect("As sementes acabaram!");
            }
        }
    }

    public void InvalidToSelect(string message)
    {
        Debug.Log(message);
        SoundManager.instance.PlaySound(invalidToSelectSound);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(cooldownText.text != "0")
        {
            cooldownText.gameObject.SetActive(true);
        }
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        cooldownText.gameObject.SetActive(false);
    }

    private void ResetState(int id)
    {
        if(id != potIndex) 
        {
            potSelection = potState.PotNotSelected;
            anim.SetInteger("transition", 0);
        }
    }

    private void ResetAllPotsState()
    {
        potSelection = potState.PotNotSelected;
        anim.SetInteger("transition", 0);
    }

    #endregion

    #region Getters And Setters
    public void UpdateSeedStatus()
    {
        currentCooldown = 0;
        currentSeedQuantity -= 1;
    }

    public PlantType TypeOfPlant()
    {
        return typePlant;
    }

    public int ReturnSunCust()
    {
        return sunCust;
    }

    public GameObject GetPlantPrefab()
    {
        return plantPrefab;
    }
    
    public GameObject GetPlantPreviewPrefab()
    {
        return plantPreviewPrefab;
    }
    #endregion


    #region Define

    public void InitializePot()
    {
        potState = potState.PotInitialized;
        PlantConfiguration();
        DefineTexts();
        DefineImages();

        currentSeedQuantity = maxSeedQuantity;
        currentSuns = sunController.totalSun;
        UIplantImage.fillAmount = 0;
        cooldownText.gameObject.SetActive(false);
    }

    public void RedefinePot()
    {
        potState = potState.PotNotInitialized;
        plantSettings = null;
        plantPrefab = null;
        plantPreviewPrefab = null;
        plantName = null;
        seedCooldown = 0;
        maxSeedQuantity = 0;
        sunCust = 0;
        RemoveTexts();
        RemoveImages();
    }

    public void PlantConfiguration()
    {
        plantPrefab = plantSettings.plantPrefab;
        plantPreviewPrefab = plantSettings.plantPreviewPrefab;
        plantName = plantSettings.plantName;
        typePlant = plantSettings.plantType;
        seedCooldown = plantSettings.plantSeedCooldown;
        maxSeedQuantity = plantSettings.plantSeedQuantity;
        sunCust = plantSettings.plantSunCust;
    }

    public void DefineTexts()
    {
        sunCustText.text = sunCust.ToString();
        seedQuantityText.text = maxSeedQuantity.ToString();
        cooldownText.text = (seedCooldown - currentCooldown).ToString();
    }

    public void RemoveTexts()
    {
        sunCustText.text = null;
        seedQuantityText.text = null;
    }

    public void DefineImages()
    {
        UIplantImage.enabled = true;
        UIplantPreviewImage.enabled = true;
        UIplantImage.sprite = plantSettings.plantSprite;
        UIplantPreviewImage.sprite = plantSettings.plantSprite;
    }

    public void RemoveImages()
    {
        UIplantImage.enabled = false;
        UIplantPreviewImage.enabled = false;
        UIplantImage.sprite = null;
        UIplantPreviewImage.sprite = null;
    }


    #endregion


}
