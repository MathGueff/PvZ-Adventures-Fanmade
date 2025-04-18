using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotsPositionController : MonoBehaviour
{
    public GameObject potPrefab;
    public GameObject powerPrefab;
    public Transform plantPotPanel; // O Transform do Painel onde as imagens serão instanciadas
    public Transform powerPanel;

    [Header("Configurações de Imagem")]
    public int numberOfPots;// Quantidade de imagens a serem instanciadas
    public float potSpacing = 10f; // Espaçamento entre as imagens
    private float yPosition = 0f;
    private float potWidth = 140f;

    [Header("Components")]
    private PickAPlantController pickController;
    private PlantPositioning plantPositioning;

    void Start()
    {
        pickController = GetComponent<PickAPlantController>();
        plantPositioning = GetComponent<PlantPositioning>();
        numberOfPots = GameController.LevelData.numberOfPots;
        InstantiatePots();
    }

    //Inicia cada pote, dependendo de quantos potes a fase disponibilizar
    private void InstantiatePots()
    {
        int centralPosition = (int)Math.Ceiling(numberOfPots / 2.0);
        float higherPositionPot = float.MinValue;
        for (int i = 1; i <= numberOfPots; i++)
        {
            //Criando o pote
            GameObject newPot = Instantiate(potPrefab, plantPotPanel);
            PlantPotsController newPotController = newPot.GetComponent<PlantPotsController>();
            newPotController.potIndex = i;

            //Configurando os potes no Dicionário em PickAPlantController
            pickController.DefinePlantPots(newPotController.potIndex, newPotController);

            //Adicionando a lista de PlantPotControllers de PlantPositioning
            plantPositioning.AddPlantPot(newPotController);

            //Adquirindo o RectTransform para reposicionamento
            RectTransform imageRectTransform = newPot.GetComponent<RectTransform>();

            float xPosition = (potWidth + potSpacing) * (i - centralPosition);
            imageRectTransform.localPosition = new Vector3(xPosition,yPosition,0);
            if(xPosition > higherPositionPot)
            {
                higherPositionPot = xPosition;
            }
        }
        
        GameObject newPower = Instantiate(powerPrefab, powerPanel);
        RectTransform powerRectTransform = newPower.GetComponent<RectTransform>();
        float powerXPosition = higherPositionPot + (potSpacing * potWidth);
        powerRectTransform.localPosition = new Vector3(powerXPosition, yPosition, 0);
    }
}
