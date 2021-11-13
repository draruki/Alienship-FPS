using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    public LevelTrigger nextLevelTrigger;
    public Transform entitiesParent;
    public Transform checkpointsParent;
    public EndGameTrigger endGameTrigger;
    public GameObject animeCollider;

    [HideInInspector]
    public List<EnemyEntity> enemiesList;
    [HideInInspector]
    public List<Checkpoint> checkpointsList;

    void Awake()
    {
        initializeEnemiesList();
        initializeCheckpointsList();

        if (animeCollider != null)
        {
            animeCollider.SetActive(!GC.ANIME);
        }
    }

    private void initializeEnemiesList()
    {
        enemiesList = new List<EnemyEntity>();
        EnemyEntity[] enemies = entitiesParent.GetComponentsInChildren<EnemyEntity>();
        foreach (EnemyEntity e in enemies)
        {
            enemiesList.Add(e);
        }
    }

    private void initializeCheckpointsList()
    {
        checkpointsList = new List<Checkpoint>();
        Checkpoint[] checkpoints = checkpointsParent.GetComponentsInChildren<Checkpoint>();
        foreach (Checkpoint c in checkpoints)
        {
            checkpointsList.Add(c);
        }
    }

    public Checkpoint getStartCheckpoint()
    {
        return checkpointsList[0];
    }
}