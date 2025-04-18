using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovementHandler : MonoBehaviour
{
    public delegate void ZombieFinishPathHandler();
    public event ZombieFinishPathHandler ZombieFinishPath; // Zumbi chega ao fim do caminho

    [Header("Components")]
    private Zombie z;

    [Header("Boolean")]
    private bool endMovement = false;

    [Header("Waypoints")]
    public Transform[] waypoints; // Waypoints que o zumbi deve seguir
    public int waypointIndex = 0; // índice atual do waypoint que o zumbi está seguindo
    public Transform currentWaypoint;
    public Transform oldWaypoint;
    public Vector3 spawnPoint;

    private void Start()
    {
        z = GetComponent<Zombie>();
        spawnPoint = transform.position;
        ZombieFinishPath += GameController.instance.OnGameOver;
    }

    private void Update()
    {
        if ((waypoints == null || waypoints.Length == 0) || endMovement)
            return;

        if (waypointIndex < waypoints.Length)
        {
            currentWaypoint = waypoints[waypointIndex];
            if (waypointIndex > 0)
                oldWaypoint = waypoints[waypointIndex - 1];
        }
    }

    public void Walk()
    {
        z.ZombieState = ZombieState.Moving;
        ZombieSpriteManager.ChangeSortingLayer(z);
        MoveTowardsWaypoint();
    }

    public void MoveTowardsWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0 || currentWaypoint == null)
            return;
        if (waypointIndex < waypoints.Length)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.position,  z.currentSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, currentWaypoint.position) < 0.1f)
            {
                // Ajusta a posição do zumbi para coincidir exatamente com o waypoint
                transform.position = currentWaypoint.position;
                waypointIndex++;
            }
        }
        else
        {
            if (!endMovement)
            {
                ZombieFinishPath();
                endMovement = true;
            }
        }
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        if (newWaypoints != null && newWaypoints.Length > 0)
        {
            waypoints = newWaypoints;
        }
        else
        {
            Debug.LogError("O array de waypoints está nulo ou vazio no zombieMovHand.");
        }
    }

    public float CalculateDistanceToEnd()
    {
        float distanceToEnd = 0;
        int indexWaypoint = 0;
        foreach (Transform waypoint in waypoints)
        {
            if (waypointIndex <= indexWaypoint)
            {
                if (waypointIndex == indexWaypoint)
                {
                    // Calcula a distância direta do zumbi até o próximo waypoint
                    distanceToEnd += Vector2.Distance(transform.position, waypoint.transform.position);
                }
                else
                {
                    // Acumula a distância entre waypoints subsequentes
                    distanceToEnd += Vector2.Distance(waypoints[indexWaypoint - 1].position, waypoint.position);
                }
            }
            indexWaypoint++;
        }
        return distanceToEnd;
    }

    public Vector2 CalculateDistanceToWaypoint(Vector3 waypoint)
    {
        Vector2 direction = transform.position - (waypoint != null ? waypoint : spawnPoint);
        return direction;
    }
}
