using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZomBowler : Zombie
{
    [Header("Components")]

    [Header("Bowling")]
    [SerializeField] private float bowlingTimer;
    public float currentTimer;
    [SerializeField] private Vector2 bowllingBallRange;
    [SerializeField] private GameObject bowlingBallPrefab;

    private Plant targetPlant;
    private bool isThrowing;

    #region BowlingBall

    private void ThrowBall()
    {
        if (targetPlant != null)
        {
            GameObject projectileGO = Instantiate(bowlingBallPrefab, transform.position, Quaternion.identity);
            BowlingBall projectile = projectileGO.GetComponent<BowlingBall>();
            // Inicializar o projétil com o alvo
            projectile.Initialize(targetPlant.transform);
            
            currentTimer = 0;
            isThrowing = false;
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, bowllingBallRange);
    }
}
