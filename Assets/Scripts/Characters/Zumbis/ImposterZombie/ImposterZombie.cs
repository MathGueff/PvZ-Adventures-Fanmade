using System.Collections.Generic;
using UnityEngine;

public class ImposterZombie : Zombie
{
    [Header("imps")]
    public GameObject impPrefab;
    public int impGenerate;
    public float spawnSpacing;
    private Dictionary<int, string> impsDirections = new();
    [SerializeField] private AudioClip impsSpawnSound;

    [Header("Directions")]
    private CharacterDirection walkDirection;
    private CharacterHorizontalDirection walkHorizontalDirection;
    private CharacterDirection oldDirection;
    private CharacterHorizontalDirection oldHorizontalDirection;
    public override void DoMove()
    {
        base.DoMove();
        walkDirection = ZombieDirection;
        walkHorizontalDirection = ZombieHorizontalDirection;
        if (!IsNextToOldWaypoint())
        {
            oldDirection = walkDirection;
            oldHorizontalDirection = walkHorizontalDirection;
        }
    }

    //Chamado como event function da animação de morte
    public void SpawnImps()
    {
        SoundManager.instance.PlaySound(impsSpawnSound);
        for (int i = 0; i < impGenerate; i++)
        {
            CreateImp(i);
            //setImpSettings(newImp, i);
        }
    }

    public void CreateImp(int i)
    {
        GameObject newImp = Instantiate(impPrefab, GetNewPosition(i), Quaternion.identity);
        setImpSettings(newImp, i);
    }

    public void setImpSettings(GameObject impObject, int i)
    {
        Zombie imp = impObject.GetComponent<Zombie>();
        if(imp != null)
        {
            imp.zombieMovHand.SetWaypoints(zombieMovHand.waypoints);
            SetWaypointsSettings(imp, i);
        }
        else
        {
            Debug.LogWarning("Imp gerado por ImposterZombie não possui script Zombie");
        }
    }

    public void SetWaypointsSettings(Zombie z, int i)
    {
        //Se o zumbinho foi spawnado no 
        if(impsDirections[i] == "sameDirection")
        {
            z.ZombieDirection = ZombieDirection;
            z.ZombieHorizontalDirection = ZombieHorizontalDirection;
            z.zombieMovHand.waypointIndex = zombieMovHand.waypointIndex;
        }
        else
        {
            int oldIndex = Mathf.Max(zombieMovHand.waypointIndex - 1, 0);
            z.zombieMovHand.waypointIndex = oldIndex;
            z.ZombieDirection = oldDirection;
            z.ZombieHorizontalDirection = oldHorizontalDirection;
        }
       
        z.zombieMovHand.spawnPoint = zombieMovHand.spawnPoint;
    }

    public Vector2 GetNewPosition(int i)
    {
        CharacterDirection spawnDirection;
        CharacterHorizontalDirection spawnHorizontalDirection;

        spawnDirection = walkDirection;
        spawnHorizontalDirection = walkHorizontalDirection;
        Vector3 startPoint = transform.position;

        if (i == 0)
        {
            impsDirections.Add(i, "sameDirection");
            return transform.position;
        }


        if (IsNextToOldWaypoint(i))
        {
            spawnDirection = oldDirection;
            spawnHorizontalDirection = oldHorizontalDirection;
            startPoint = zombieMovHand.oldWaypoint.transform.position;
            impsDirections.Add(i, "oldDirection");
        }
        else
            impsDirections.Add(i, "sameDirection");

        switch (spawnDirection)
        {
            case CharacterDirection.Top:
                return startPoint + new Vector3(0f, i * spawnSpacing);
            case CharacterDirection.Down:
                return startPoint - new Vector3(0f, i * spawnSpacing);
            case CharacterDirection.Front:
                if (spawnHorizontalDirection == CharacterHorizontalDirection.Right)
                {
                    return startPoint - new Vector3(i * spawnSpacing, 0f);
                }
                else if(spawnHorizontalDirection == CharacterHorizontalDirection.Left)
                {
                    return startPoint + new Vector3(i * spawnSpacing, 0f);
                }
                break;
        }

        return transform.position;
    }

    public bool IsNextToOldWaypoint(int? i = null)
    {
        float distance;

        if(i == null)
            distance = spawnSpacing * (impGenerate - 1);
        else
            distance = i.Value * spawnSpacing;

        Vector2 positionTarget;
        Transform target = zombieMovHand.oldWaypoint;
        if(target == null)
        {
            positionTarget = zombieMovHand.spawnPoint;
        }
        else
        {
            positionTarget = zombieMovHand.oldWaypoint.transform.position;
        }
        return Vector2.Distance(transform.position, positionTarget) <= distance;
    }
}