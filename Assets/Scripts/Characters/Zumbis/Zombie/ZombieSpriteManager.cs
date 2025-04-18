using System.Collections.Generic;
using UnityEngine;

public static class ZombieSpriteManager
{
    public static void ChangeSortingLayer(Zombie zombie)
    {
        foreach (var sprite in zombie.SpriteRendererList)
        {
            sprite.Key.sortingOrder = Mathf.RoundToInt((-zombie.transform.position.y * 100) + sprite.Value);
        }
    }

    public static void AddSpriteRenderer(Zombie zombie, SpriteRenderer spriteRenderer, int value = 0)
    {
        if (spriteRenderer != null && zombie.SpriteRendererList != null)
        {
            Debug.Log($"Alterando cor do SpriteRenderer {spriteRenderer.name} do objeto {zombie.name}");
            zombie.SpriteRendererList.Add(spriteRenderer, value);
        }
        else
        {
            Debug.LogWarning("Tentando adicionar um SpriteRenderer nulo.");
        }
    }

    public static void RemoveSpriteRenderer(Zombie zombie, SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer != null && zombie.SpriteRendererList.ContainsKey(spriteRenderer))
        {
            zombie.SpriteRendererList.Remove(spriteRenderer);
        }
        else
        {
            Debug.LogWarning("Tentando adicionar um SpriteRenderer nulo.");
        }
    }
}