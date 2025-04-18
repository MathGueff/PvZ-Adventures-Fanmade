using UnityEngine;

public class SnapDragonFire : MonoBehaviour
{
    [Header("Attributes")]
    public float speed = 5f;  // Velocidade do projétil
    [HideInInspector] public int damage;   // Dano do projétil

    [Header("Attack")]
    private Transform target; // Alvo do projétil
    private GameObject snapDragon; // GO do SnapDragon que atacou
    private float overflowDistance;
    private CharacterDirection directionToGo;

    private Plant parentScript;
    private Vector3 endPosition;


    public void Initialize(Transform target, GameObject snapDragon, CharacterDirection direction, float overflowDistance)
    {
        this.target = target; //Direção target
        this.snapDragon = snapDragon; //Para obter a posição
        parentScript = snapDragon.GetComponent<Plant>(); //
        directionToGo = direction;
        this.overflowDistance = overflowDistance;
    }

    void Update()
    {
        if (snapDragon == null || parentScript.PlantLifeState != PlantLifeState.Alive)
        {
            Destroy(gameObject);
            return;
        }

        MoveProjectile();
    }

    private void MoveProjectile()
    {
        if(endPosition == Vector3.zero)
            endPosition = GetTargetDirection();

        Vector3 direction = (endPosition - transform.position).normalized;
        float distanceThisFrame = speed * Time.deltaTime;

        // Move o projétil em direção ao alvo
        transform.Translate(direction * distanceThisFrame, Space.World);

        // Verifica se o projétil atingiu a posição final
        if (Vector3.Distance(transform.position, endPosition) <= distanceThisFrame)
        {
            DestroyProjectile();
        }
    }

    private Vector3 GetTargetDirection()
    {
        Vector3 directionPlus = Vector3.zero;
        switch (directionToGo)
        {
            case CharacterDirection.Front:
                directionPlus = Mathf.Approximately(snapDragon.transform.eulerAngles.y, 0) ?
                   new Vector3(overflowDistance, 0, 0) : new Vector3(-overflowDistance, 0, 0);
                break;
            case CharacterDirection.Top:
                directionPlus = new Vector3(0, -overflowDistance, 0);
                break;
            case CharacterDirection.Down:
                directionPlus = new Vector3(0, overflowDistance, 0);
                break;
        }

        return (snapDragon.transform.position + directionPlus);
    }


    void HitTarget(Collider2D zombieCollider)
    {
        Zombie zombie = zombieCollider.GetComponent<Zombie>();
        parentScript.plantAttackHandler.AttackZombie(zombie);
    }

    void DestroyProjectile()
    {
        // Destroi o projétil
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            HitTarget(collision);
        }
    }
}