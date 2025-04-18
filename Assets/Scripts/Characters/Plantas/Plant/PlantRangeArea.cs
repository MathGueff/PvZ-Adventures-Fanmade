using UnityEngine;

public class PlantRangeArea : MonoBehaviour
{
    private Plant plant;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        plant = GetComponentInParent<Plant>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer!= null && plant != null)
        {
            DrawRangeArea();
        }
        EventHandler.Instance.OnMouseEnterPlant += ShowArea;
        EventHandler.Instance.OnMouseExitPlant += HideArea;
    }

    private void DrawRangeArea()
    {
        spriteRenderer.size = new Vector2(plant.DettectRange.x, plant.DettectRange.y);
        spriteRenderer.color = plant.plantSettings.rangeColor;
    }

    public void RedrawRangeArea()
    {
        if (spriteRenderer != null && plant != null)
        {
            spriteRenderer.size = new Vector2(plant.DettectRange.x, plant.DettectRange.y);
        }
    }

    private void ShowArea(Plant plant)
    {
        if(plant == this.plant)
        {
            spriteRenderer.enabled = true;
        }
    }
    private void HideArea(Plant plant)
    {
        if (plant == this.plant)
        {
            spriteRenderer.enabled = false;
        }
    }
}