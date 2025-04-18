using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SunController : MonoBehaviour
{
    public static SunController instance;

    [Header("SunManagement")]
    public int totalSun = 100; // Exemplo de quantidade inicial de sol
    public int sunGeneration = 25;
    public float sunCooldown = 15;
    private float sunCurrentCooldown;
    public GameObject prefabSun;

    [Header("UI")]
    public TextMeshProUGUI sunText;
    public Transform sunDropPanel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        totalSun = GameController.LevelData.sunAmountStart;
        UpdateSunText();
    }

    private void Update()
    {
        if(GameController.instance.gameState == GameState.Started)
        {
            sunCurrentCooldown += Time.deltaTime;
            if (sunCurrentCooldown >= sunCooldown)
            {
                int randomNumber = Random.Range(1, 101);
                if (randomNumber <= GameController.LevelData.insolationChance)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        CreateSun();
                    }
                }
                else
                {
                    CreateSun();
                }
                //AddSun(sunGeneration);
                sunCurrentCooldown = 0;
            }
        }
    }

    // Método para atualizar o texto do sol
    public void UpdateSunText()
    {
        if (sunText != null)
        {
            sunText.text = totalSun.ToString();
        }
    }

    public void CreateSun()
    {
        Collider2D collider = sunDropPanel.GetComponent<Collider2D>();

        // Calcula os limites no espaço de mundo
        Vector3 minWorld = sunDropPanel.TransformPoint(collider.bounds.min);
        Vector3 maxWorld = sunDropPanel.TransformPoint(collider.bounds.max);

        // Calcula a distância entre o ponto inicial e final
        float distance = maxWorld.x - minWorld.x;

        // Gera um ponto aleatório dentro dos limites convertidos
        float randomX = Random.Range(minWorld.x, maxWorld.x);
        Vector2 spawnPosition = new Vector2(randomX, sunDropPanel.position.y);

        // Instancia o sol na posição calculada
        GameObject sunCreated = Instantiate(prefabSun, spawnPosition, Quaternion.identity);

        // Configura o objeto sol
        Sun sunConfig = sunCreated.GetComponent<Sun>();
        sunConfig.setSunAmount(sunGeneration);
        float randomY = Random.Range(0, -10);

        sunConfig.moveSun(new Vector2(sunConfig.transform.position.x,randomY), 1.1f);
    }


    // Método exemplo para adicionar sóis
    public void AddSun(int amount)
    {
        totalSun += amount;
        UpdateSunText(); // Atualiza o texto sempre que o total de sol muda
    }

    // Método exemplo para remover sóis
    public void RemoveSun(int amount)
    {
        totalSun -= amount;
        UpdateSunText(); // Atualiza o texto sempre que o total de sol muda
    }

    public bool CompareSuns(int sunCust)
    {
        if(totalSun >= sunCust)
        {
            return true;
        }
        return false;
    }
}
