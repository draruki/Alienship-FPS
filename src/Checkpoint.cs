using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour 
{
    public List<EnemyEntity> enemyList;
    public List<Pickup> pickupList;
    public List<Debaser> debaserList;
    [HideInInspector]
    public bool playerReached;
    private GameManager gameManager;

    // debug variables
    public int expectedSingleAmmo = 20;
    public int expectedSpreadAmmo = 6;
    public int expectedAutoAmmo = 35;
    public bool shotgun = true;
    public bool rifle = false;

    void Awake()
    {
        playerReached = false;
    }

	void Start () 
    {
        
	}

    public void activateDebasers()
    {
        if (debaserList != null && debaserList.Count != 0)
        {
            foreach (Debaser d in debaserList)
            {
                d.activate();
            }
        }
    }
	
    public void spawn()
    {
        if (playerReached == false)
        {
            foreach(EnemyEntity e in enemyList)
            {
                e.respawn();
            }
            foreach(Pickup p in pickupList)
            {
                p.respawn();
            }
        }
        else
        {
            foreach(Pickup p in pickupList)
            {
                if (p.playerTaken == false)
                {
                    p.respawn();
                }
            }
        }
    }

    public void setGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void setReached()
    {
        playerReached = true;

        foreach(Pickup p in pickupList)
        {
            p.playerTaken = true;
            p.kill();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (playerReached == true)
            return;

        PlayerEntity player = other.gameObject.GetComponent<PlayerEntity>();
        if (player != null)
        {
            gameManager.onReachedCheckpoint(this);
            playerReached = true;
        }
    }
}