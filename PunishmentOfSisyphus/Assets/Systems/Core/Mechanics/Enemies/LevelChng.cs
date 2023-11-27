using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;

using Ephymeral.EntityNS;
using Ephymeral.BoulderNS;
using Ephymeral.Events;
using Ephymeral.Data;
using Ephymeral.EnemyNS;
using UnityEngine.SceneManagement;
using Ephymeral.PlayerNS;

public class LevelChng : MonoBehaviour
{
    private EnemySpawner enemySpawner;
    [SerializeField] private EnemySpawnerEvent enemySpawnEvent;
    private Player player;


    private void Awake()
    {
        enemySpawner = (EnemySpawner)FindObjectOfType(typeof(EnemySpawner));
        player = (Player)FindObjectOfType(typeof(Player));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("levelchng triggered");
        if (collision.CompareTag("Player"))
        {
            if(enemySpawner.EnemiesCnt == 0)
            {
                enemySpawner.IncrementLevel();
                //player.resetPlayer();
                player.ToggleLevelTransition();
            }
        }
    }
}
