using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndGameTrigger : MonoBehaviour 
{
    public Transform entityParent;
    public GameManager gameManager;
    private List<EnemyEntity> enemyList;
    private Collider col;

	void Start () 
    {
        enemyList = new List<EnemyEntity>();
        EnemyEntity[] enemies = entityParent.GetComponentsInChildren<EnemyEntity>();
        foreach (EnemyEntity e in enemies)
        {
            enemyList.Add(e);
        }

        col = GetComponent<BoxCollider>();
	}

    public void setGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;

        col = GetComponent<BoxCollider>();
        col.enabled = true;
    }

    public void checkAllDead()
    {
        for (int i = 0; i < enemyList.Count; ++i)
        {
            if (enemyList[i].dead == false)
            {
                return;
            }
        }

        gameManager.onGameEnd();

        col.enabled = false;
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        checkAllDead();
    }
    void OnTriggerExit(Collider other)
    {
        checkAllDead();
    }
}