using UnityEngine;

public class SnapDragon : Plant
{
    [Header("Projétil")]
    public GameObject frontLeftProjectileprefab; // Prefab do projétil em visão lateral esquerda
    public GameObject frontRightProjectileprefab; // Prefab do projétil em visão lateral direita
    public GameObject topProjectileprefab; // Prefab do projétil em visão top
    public GameObject downProjectileprefab; // Prefab do projétil em visão down
    public Vector3 firePoint;         // Ponto de origem do projétil
    public float ProjectileOverflowDistance;

    [Header("Zumbis")]
    private Zombie targetZombie;

    [Header("Sounds")]
    [SerializeField] private AudioClip attackSound;

    protected override void DoAction()
    {
        plantAttackHandler.DoAttack(DettectRange);
        targetZombie = plantAttackHandler.GetTarget();
    }

    private void ShootFire()
    {
        if (targetZombie == null || targetZombie.ZombieState == ZombieState.Dying)
        {
            EndAction();
            return;
        }

        GameObject projectilePrefab = null;
        switch (PlantDirection)
        {
            case CharacterDirection.Top:
                projectilePrefab = topProjectileprefab;
                break;
            case CharacterDirection.Front:
                //Caso euler angles seja proximo a 0, significa que o personagem está olhando para a direita
                projectilePrefab = Mathf.Approximately(transform.eulerAngles.y, 0) ?
                    frontRightProjectileprefab : frontLeftProjectileprefab;
                break;
            case CharacterDirection.Down:
                projectilePrefab = downProjectileprefab;
                break;
        }
        if(projectilePrefab != null)
        {
            SoundManager.instance.PlaySound(attackSound);
            firePoint = CalculateFirePoint();
            GameObject projectileGO = Instantiate(projectilePrefab, firePoint, Quaternion.identity);
            SnapDragonFire dragonFire = projectileGO.GetComponent<SnapDragonFire>();
            // Inicializar o projétil com o alvo
            if (dragonFire != null)
            {
                dragonFire.Initialize(targetZombie.transform, gameObject, PlantDirection, ProjectileOverflowDistance);
                dragonFire.damage = PlantDamage;
            }
        }
       
        EndAction();
    }

    private Vector3 CalculateFirePoint()
    {
        Vector3 directionPlus = Vector3.zero;
        float plusValue = 0.5f;
        switch (PlantDirection)
        {
            case CharacterDirection.Front:
                directionPlus = Mathf.Approximately(transform.eulerAngles.y, 0) ?
                   new Vector3(plusValue, 0, 0) : new Vector3(-plusValue, 0, 0);
                break;
            case CharacterDirection.Top:
                directionPlus = new Vector3(0, -plusValue, 0);
                break;
            case CharacterDirection.Down:
                directionPlus = new Vector3(0, plusValue, 0);
                break;
        }

        return (transform.position + directionPlus);
    }
    public override void BoostPlant()
    {
        ProjectileOverflowDistance += 2;
        base.BoostPlant();
    }

    public override void EndBoost()
    {
        ProjectileOverflowDistance -= 2;
        base.EndBoost();
    }
}