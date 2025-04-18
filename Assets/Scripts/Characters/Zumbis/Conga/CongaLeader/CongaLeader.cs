using System.Collections;
using UnityEngine;

public class CongaLeader : Zombie
{
    [Header("CongaDancers")]
    public Vector3 spawnPoint;
    public GameObject congaDancerPrefab;

    [Header("Time")]
    public float spawnInterval;
    public float currentSpawnTimer;

    private CongaLeaderBody body;
    private CongaLeaderHat armor;

    #region Override UnityMethods
    public override void Awake()
    {
        base.Awake();
        body = GetComponentInChildren<CongaLeaderBody>();
        armor = GetComponentInChildren<CongaLeaderHat>();
        spawnPoint = transform.position;
    }

    public override void Start()
    {
        base.Start();
        Animator bodyAnim = body.GetAnimator();

        if (bodyAnim != null)
        {
            anim = bodyAnim;
        }
        else
        {
            Debug.LogWarning("Animator do body não encontrado");
        }
        EventHandler.Instance.LeaderCongaSpawned(this);
    }

    public override void Update()
    {
        if (ZombieState == ZombieState.EspecialAction || ZombieState == ZombieState.Dying)
            return;

        if (zombieEffects.currentEffect == Effects.Freeze || zombieEffects.currentEffect == Effects.Stun)
        {
            StopCoroutine(WaitToEnd());
            ResetTimeToSpawn();
            return;
        }

        if (CheckTimeToSpawn())
        {
            SpawnCongaDancer();
        }
        else
        {
            base.Update();
        }
    }
    #endregion


    #region Spawn Dancers
    public bool CheckTimeToSpawn()
    {
        if (currentSpawnTimer >= spawnInterval && ZombieState != ZombieState.Attacking)
        {
            return true;
        }
        currentSpawnTimer += Time.deltaTime;
        return false;
    }

    public void ResetTimeToSpawn()
    {
        currentSpawnTimer = 0;
        if(ZombieState != ZombieState.Dying)
            ZombieState = ZombieState.None;
    }

    public void SpawnCongaDancer()
    {
        ZombieState = ZombieState.EspecialAction;
        GameObject newcongaDancer = Instantiate(congaDancerPrefab, spawnPoint, Quaternion.identity);
        CongaDancer congaDancerScript = newcongaDancer.GetComponent<CongaDancer>();
        if (congaDancerScript != null)
        {
            congaDancerScript.SetWaypointDefinitions(zombieMovHand.waypoints);
            congaDancerScript.SetLeader(this);
            congaDancerScript.ZombieHorizontalDirection = ZombieHorizontalDirection;
            //DirectionManager.TurnCharacter(newcongaDancer, gameObject.transform.eulerAngles.y);
        }
        StartCoroutine(WaitToEnd());
    }

    public IEnumerator WaitToEnd()
    {
        yield return new WaitForSeconds(4f); // Espera 4 segundos
        ResetTimeToSpawn();
    }

    #endregion

    #region Override Do's
    public override void DoTakeDamage(int amount, DamageType damageType)
    {
        if (armor.currentArmorHealth > 0)
        {
            armor.TakeDamage(amount, damageType);
        }
        else
        {
            base.DoTakeDamage(amount, damageType);
        }
    }

    public override void DoAttack(Plant targetEnemy)
    {
        currentSpawnTimer = 0;
        base.DoAttack(targetEnemy);
    }

    public override void DoDie()
    {
        EventHandler.Instance.LeaderCongaDied(this);
        StopCoroutine(WaitToEnd());
        base.DoDie();
    }

    #endregion

}