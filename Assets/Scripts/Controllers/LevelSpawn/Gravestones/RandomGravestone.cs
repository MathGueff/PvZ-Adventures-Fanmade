using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGravestone : MonoBehaviour
{
    [Header("Zombies")]
    public List<GameObject> zombiesToSpawn; // Lista de prefabs de zumbis dispon�veis para gerar
    private int zombiesSpawned = 0; // Contador de zumbis gerados
    public Transform[] waypoints; // Waypoints que os zumbis devem seguir
    private bool isSpawning = false;

    [Header("Time")]
    public float minSpawnInterval = 2f; // Intervalo m�nimo entre a gera��o de zumbis
    public float maxSpawnInterval = 5f; // Intervalo m�ximo entre a gera��o de zumbis
    private float currentSpawnInterval; // Intervalo de tempo atual entre a gera��o de zumbis
    private float spawnTimer = 0f; // Temporizador para controlar o intervalo de gera��o

    [Header("Spawn Settings")]
    public bool isSpawnLimited = true; // Define se o spawn � limitado ou n�o
    public int maxZombiesToSpawn = 5; // Quantidade m�xima de zumbis a serem gerados (0 para ilimitado)

    [Header("Components")]
    private Animator anim;
    private GameController gameController;


    private void Start()
    {
        anim = GetComponent<Animator>();
        SetRandomSpawnInterval(); // Define um intervalo de spawn aleat�rio no in�cio
        gameController = FindObjectOfType<GameController>();
    }

    private void Update()
    {
        if(gameController.gameState == GameState.Started)
        {
            anim.SetBool("isSpawning", isSpawning);
            if (!isSpawning)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= currentSpawnInterval - 1)
                {
                    if (!isSpawnLimited || zombiesSpawned < maxZombiesToSpawn || maxZombiesToSpawn <= 0)
                    {
                        isSpawning = true;
                    }
                }
            }
        }
    }

    private void SpawnZombie()
    {
        if (zombiesToSpawn.Count > 0)
        {
            // Escolhe um zumbi aleat�rio da lista
            int randomIndex = Random.Range(0, zombiesToSpawn.Count);
            GameObject newZombie = Instantiate(zombiesToSpawn[randomIndex], transform.position, Quaternion.identity);
            Zombie zombieScript = newZombie.GetComponent<Zombie>();

            if (zombieScript != null)
            {
                zombieScript.zombieMovHand.SetWaypoints(waypoints); // Define os waypoints para o zumbi seguir
            }

            zombiesSpawned++; // Incrementa o contador de zumbis gerados
        }
        isSpawning = false;
        spawnTimer = 0f; // Reseta o temporizador
        SetRandomSpawnInterval(); // Define um novo intervalo de spawn aleat�rio
    }

    private void SetRandomSpawnInterval()
    {
        // Define um intervalo de spawn aleat�rio entre o m�nimo e o m�ximo
        currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);

        if (waypoints != null)
        {
            foreach (Transform waypoint in waypoints)
            {
                Gizmos.DrawSphere(waypoint.position, 0.5f);
            }
        }
    }
}
