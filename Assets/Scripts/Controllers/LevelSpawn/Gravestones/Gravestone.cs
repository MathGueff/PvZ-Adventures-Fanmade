using UnityEngine;

public class Gravestone : MonoBehaviour
{
    [Header("Zombies")]
    public Transform[] waypoints; // Waypoints que os zumbis devem seguir

    [Header("Components")]
    private Animator anim;
    private WaveController waveController;

    private void Start()
    {
        anim = GetComponent<Animator>();
        waveController = FindObjectOfType<WaveController>();
    }

    public void SpawnZombie(GameObject zombiePrefab, bool waveZombie = false)
    {
        if (zombiePrefab != null)
        {
            anim.SetTrigger("isSpawning");

            // Instancia o zumbi
            GameObject spawnedZombie = Instantiate(zombiePrefab, transform.position, Quaternion.identity);
            if (waveZombie)
            {
                ZombiesManager.instance.zombiesForWaveAlive.Add(spawnedZombie);
            }
            if (spawnedZombie != null)
            {
                // Tenta acessar o script Zombie ou sua subclasse
                Zombie zombieScript = spawnedZombie.GetComponent<Zombie>();

                if (zombieScript != null)
                {
                    // Define os waypoints
                    if (zombieScript?.zombieMovHand != null)
                    {
                        zombieScript.zombieMovHand.SetWaypoints(waypoints);
                    }
                    else
                    {
                        // Tratar o caso onde o zombieMovHand é nulo
                        Debug.LogWarning("zombieMovHand é nulo!");
                    }
                }
                else
                {
                    Debug.LogError("O prefab instanciado não possui um script derivado de Zombie!");
                }
            }
            else
            {
                Debug.LogError("Falha ao instanciar o prefab do zumbi.");
            }
        }
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
