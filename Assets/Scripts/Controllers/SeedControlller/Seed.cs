using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Necessário para o uso de IPointerClickHandler
using UnityEngine.UI;

public enum seedState
{
    NotSelected,
    Selected
}

public class Seed : MonoBehaviour, IPointerClickHandler // Implementa a interface
{
    [Header("Components")]
    public PlantScriptable plantScriptable;
    private PickAPlantController pickAPlantController;
    private Animator anim;

    [Header("State")]
    private seedState currentState;

    private Image plantImage;

    private void Start()
    {
        currentState = seedState.NotSelected;
        pickAPlantController = FindObjectOfType<PickAPlantController>();
        anim = GetComponentInChildren<Animator>();
        plantImage = transform.Find("PlantImage").GetComponent<Image>();
        plantImage.enabled = true;
        plantImage.sprite = plantScriptable.plantSprite;
    }

    // Implementação do método da interface IPointerClickHandler
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentState == seedState.NotSelected)
        {
            int? nextPot = pickAPlantController.GetNextPotAvailable();
            if(nextPot != null) //Se houver potes disponíveis
            {
                anim.SetInteger("transition", 1);
                currentState = seedState.Selected;
                pickAPlantController.setPotPlant(plantScriptable, (int) nextPot);
            }
            else
            {
                Debug.Log("Todos os potes de plantas estão cheios!");
            }
        }
        else
        {
            anim.SetInteger("transition", 0);
            currentState = seedState.NotSelected;
            pickAPlantController.RedistributePlantInPots(plantScriptable);
        }
    }
}
