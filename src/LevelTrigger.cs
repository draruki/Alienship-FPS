using UnityEngine;
using System.Collections;

public class LevelTrigger : MonoBehaviour 
{
    [Range(1, 3)]
    public int level;
    private GameManager gameManager;

    public void setGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerEntity player = other.gameObject.GetComponent<PlayerEntity>();
        if (player != null)
        {
            gameManager.onLevelTrigger(level);
        }
    }
}