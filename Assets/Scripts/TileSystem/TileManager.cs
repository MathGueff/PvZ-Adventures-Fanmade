using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public enum PlantType 
{
    Lawn,
    Path
}

public class TileManager : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap tilemapGramado; // Tilemap que contém os locais para plantio no gramado
    public Tilemap[] tilemapCaminhos; // Array de Tilemaps que contêm os locais para plantio nos caminhos

    [Header("Prefabs")]
    public GameObject plantSpotPrefab; // Prefab dos spots

    [Header("Parent")]
    public Transform plantSpotsParent; // Referência para o GameObject pai dos spots

    private List<GameObject> plantSpots = new List<GameObject>(); // Lista de spots gerados
    private List<GameObject> plantSpotsLawn = new List<GameObject>(); // Lista de spots gerados
    private List<GameObject> plantSpotsPath = new List<GameObject>(); // Lista de spots gerados

    void Start()
    {
        //currentTilemapType = TilemapType.Gramado; // Define o tipo de tilemap inicial
        GeneratePlantSpots();
    }

    void GeneratePlantSpots()
    {
        foreach (Tilemap tilemapCaminho in tilemapCaminhos)
        {
            GeneratePlantSpotsForTilemap(tilemapCaminho);
        }
        GeneratePlantSpotsForTilemap(tilemapGramado);
    }

    void GeneratePlantSpotsForTilemap(Tilemap tilemap)
    {
        // Itera sobre todas as células dentro dos limites do Tilemap
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            // Obtém a posição no mundo correspondente
            Vector3 worldPos = tilemap.GetCellCenterWorld(position);

            // Verifica se há um tile na posição atual
            TileBase tile = tilemap.GetTile(position);

            // Se um tile estiver presente, cria um PlantSpot
            if (tile != null)
            {
                GameObject spot = Instantiate(plantSpotPrefab, worldPos, Quaternion.identity);

                // Define o pai como plantSpotsParent para melhor organização
                if (plantSpotsParent != null)
                {
                    spot.transform.SetParent(plantSpotsParent);
                }

                spot.SetActive(false); // Inicialmente inativo
                if(tilemap == tilemapGramado)
                {
                    spot.GetComponent<PlantSpot>().positionType = PlantType.Lawn;
                    plantSpotsLawn.Add(spot);
                    plantSpots.Add(spot);
                }
                else
                {
                    spot.GetComponent<PlantSpot>().positionType = PlantType.Path;
                    plantSpotsPath.Add(spot);
                    plantSpots.Add(spot);
                }
            }
        }
    }

    public void ShowPlantSpots(PlantType type)
    {
        HidePlantSpots();
        if (type == PlantType.Lawn)
        {
            foreach (GameObject spot in plantSpotsLawn)
            {
                if(spot.GetComponent<PlantSpot>().DetectPlant() == null)
                    spot.SetActive(true);   
            }
        }
        else
        {
            foreach (GameObject spot in plantSpotsPath)
            {
                if (spot.GetComponent<PlantSpot>().DetectPlant() == null)
                    spot.SetActive(true);
            }
        }
    }

    public void HidePlantSpots()
    {
        foreach (GameObject spot in plantSpots)
        {
            spot.SetActive(false);
        }
    }
}
