using UnityEngine;

public class Peashooter : Plant
{
    [Header("Projétil")]
    public GameObject projectilePrefab; // Prefab do projétil
    public Transform firePoint;         // Ponto de origem do projétil

    [Header("Zumbis")]
    private Zombie targetZombie;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void DoAction()
    {
        plantAttackHandler.DoAttack(DettectRange);
        targetZombie = plantAttackHandler.GetTarget();
    }

    //Chamado durante as animações de ataque
    private void Shoot()
    {
        if (targetZombie == null || targetZombie.ZombieState == ZombieState.Dying)
        {
            EndAction();
            return;
        }

        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        PeashooterProjectile projectile = projectileGO.GetComponent<PeashooterProjectile>();

        // Inicializar o projétil com o alvo
        if(projectile != null)
        {
            projectile.Initialize(targetZombie.transform, this);
            projectile.damage = PlantDamage;
        }
        EndAction();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Cor do gizmo para visualizar
        Gizmos.DrawWireCube(transform.position, DettectRange);
    }
}
