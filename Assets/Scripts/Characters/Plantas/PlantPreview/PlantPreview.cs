using UnityEngine;

public class PlantPreview : MonoBehaviour
{
    public PlantScriptable plantScriptable;
    private SpriteRenderer rangeArea;

    private void Start()
    {
        Transform rangeAreaTransform = transform.Find("RangeArea");

        if (rangeAreaTransform != null)
        {
            rangeArea = rangeAreaTransform.GetComponent<SpriteRenderer>();
        }

        if (rangeArea != null && plantScriptable != null)
        {
            DrawRangeArea();
        }
    }

    private void DrawRangeArea()
    {
        rangeArea.size = new Vector2(plantScriptable.dettectRange.x, plantScriptable.dettectRange.y);
        rangeArea.color = plantScriptable.rangeColor;
    }
}