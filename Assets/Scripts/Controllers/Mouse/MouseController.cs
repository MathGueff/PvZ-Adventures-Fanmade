using UnityEngine;

public class MouseController : MonoBehaviour
{
    public enum MouseType
    {
        Default,
        SprayZombie,
        SprayPlant,
    }

    public AudioClip sprayPlantSound;
    public AudioClip sprayZombieSound;

    public Texture2D sprayPlantCursor;
    public Texture2D sprayZombieCursor;
    public Vector2 hotspot = Vector2.zero;

    public MouseType currentMouse;
    public Plant currentPlant;
    public Zombie currentZombie;

    private void Start()
    {
        EventHandler.Instance.OnMouseEnterPlant += SetCursorSprayPlant;
        EventHandler.Instance.OnMouseExitPlant += ResetCursor;
        EventHandler.Instance.OnMouseDownPlant += CallPlantSpraySound;
        EventHandler.Instance.OnMouseEnterZombie += SetCursorSprayZombie;
        EventHandler.Instance.OnMouseExitZombie += ResetCursor;
        EventHandler.Instance.OnMouseDownZombie += CallZombieSpraySound;
    }

    private void Update()
    {
        bool isPlantInactive = currentPlant == null ||
                          currentPlant.PlantLifeState == PlantLifeState.Dying ||
                          currentPlant.PlantLifeState == PlantLifeState.Reviving;

        bool isMouseInSprayMode = currentMouse == MouseType.SprayPlant || currentMouse == MouseType.SprayZombie;

        if (isPlantInactive && currentZombie == null && isMouseInSprayMode)
        {
            ResetCursor();
        }
    }

    private void CallZombieSpraySound(Zombie z)
    {
        SoundManager.instance.PlaySound(sprayZombieSound);
    }

    private void CallPlantSpraySound(Plant p)
    {
        SoundManager.instance.PlaySound(sprayPlantSound);
    }

    private void ResetCursor()
    {
        switch (currentMouse)
        {
            case MouseType.SprayZombie:
                currentZombie = null;
                break;
            case MouseType.SprayPlant:
                currentPlant = null;
                break;
        }
        currentMouse = MouseType.Default;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void ResetCursor(Plant p) => ResetCursor();

    private void ResetCursor(Zombie z) => ResetCursor();

    private void SetCursorSprayPlant(Plant plant)
    {
        if (sprayPlantCursor != null)
        {
            currentPlant = plant;
            currentMouse = MouseType.SprayPlant;
            Cursor.SetCursor(sprayPlantCursor, hotspot, CursorMode.Auto);
        }
        else
        {
            Debug.LogWarning("Cursor Texture não foi atribuída!");
        }
    }

    private void SetCursorSprayZombie(Zombie zombie)
    {
        if (sprayZombieCursor != null)
        {
            currentZombie = zombie;
            currentMouse = MouseType.SprayZombie;
            Cursor.SetCursor(sprayZombieCursor, hotspot, CursorMode.Auto);
        }
        else
        {
            Debug.LogWarning("Cursor Texture não foi atribuída!");
        }
    }
}