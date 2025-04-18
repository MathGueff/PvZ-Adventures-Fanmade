using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DetectAttackType
{
    None,
    Further, //Mais longe
    Closer, //Mais perto
    DistanceToEnd //Distância até o fim
}

public enum CharacterDirection
{
    Top = 0,
    Front = 1,
    Down = 2
}

public enum CharacterHorizontalDirection
{
    None,
    Right = 1,
    Left = -1
}

public static class DirectionManager
{
    public static void ChangeDirection(GameObject character,Transform target, Animator anim)
    {
        if(target != null)
        {
            (CharacterDirection characterDirection, CharacterHorizontalDirection horizontalDirection) directions = GetDirection(target.position, character);
            
            Plant plant = character.GetComponent<Plant>();
            if(plant != null)
            {
                plant.PlantDirection = directions.characterDirection;
                plant.PlantHorizontalDirection = directions.horizontalDirection;
                return;
            }
           
            Zombie zombie = character.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.ZombieDirection = directions.characterDirection;
                zombie.ZombieHorizontalDirection = directions.horizontalDirection;
            }
        }
    }

    /// <summary>
    /// Recebe o transform do alvo a ser comparado e do próprio objeto 
    /// </summary>
    /// <param name="target">Alvo para comparação</param>
    /// <param name="character">Próprio objeto</param>
    /// <returns>Retorna a direção entre eles</returns>
    public static (CharacterDirection characterDirection, CharacterHorizontalDirection horizontalDirection) GetDirection(Vector3 target, GameObject character)
    {
        CharacterDirection animDirection = CharacterDirection.Top;
        CharacterHorizontalDirection horizontalDirection = CharacterHorizontalDirection.None;

        Vector2 direction = target - character.transform.position;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Movendo-se para os lados (horizontal)
            if (direction.x > 0) //Direita
            {
                horizontalDirection = CharacterHorizontalDirection.Right;
            }
            else if (direction.x < 0) //Esquerda
            {
                horizontalDirection = CharacterHorizontalDirection.Left;
            }
            animDirection = CharacterDirection.Front;
        }
        else
        {
            // Movendo-se verticalmente (cima ou baixo)
            if (direction.y > 0) //Para cima (DOWN)
            {
                animDirection = CharacterDirection.Down;
            }
            else if (direction.y < 0) //Para baixo (TOP)
            {
                animDirection = CharacterDirection.Top;
            }
        }
        return (animDirection, horizontalDirection);
    }
}

