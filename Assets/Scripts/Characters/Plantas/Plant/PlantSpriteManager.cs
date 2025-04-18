using UnityEngine;

public static class PlantSpriteManager
{
    public enum ScaleState
    {
        Normal, 
        PlantBuffed
    }

    public static void PlantBoostedSprite(SpriteRenderer plantSprite, Plant plant)
    {
        ChangeScale(plant, ScaleState.PlantBuffed);
        ChangeColor(plantSprite, new Color(0.9329033f, 1f, 0.03301889f));
        plantSprite.sortingOrder += 1;
    }

    public static void NormalPlantSprite(SpriteRenderer plantSprite, Plant plant)
    {
        ChangeScale(plant, ScaleState.Normal);
        if (plant.PlantLifeState == PlantLifeState.Alive)
        {
            ChangeColor(plantSprite, new Color(1f,1f,1f));
        }
        plantSprite.sortingOrder -= 1;
    }

    public static void ChangeColor(SpriteRenderer sr, Color color)
    {
        sr.color = color;
    }

    public static void ChangeScale(Plant plant, ScaleState scaleState)
    {
        if(scaleState == ScaleState.PlantBuffed)
        {
            float scaleValue = 0.1f;

            plant.transform.localScale = new Vector3(1 + scaleValue, 1 + scaleValue, 1f);

            if (plant.plantRangeArea != null)
            {
                plant.plantRangeArea.transform.localScale = new Vector3((1 - scaleValue) + 0.009f, (1 - scaleValue) + 0.009f, 1f);
            }
        }
        else
        {
            plant.transform.localScale = new Vector3(1f, 1f, 1f);

            if (plant.plantRangeArea != null)
            {
                plant.plantRangeArea.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
} 