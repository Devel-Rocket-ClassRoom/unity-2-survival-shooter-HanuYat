using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyData[] enemyDatas;
    public Enemy[] enemyPrefabs;
    public Transform[] spawnPoints;

    public GameManager gameManager;

    private List<Enemy> _enemies = new List<Enemy>();
    private int _wave = 0;

    private void Update()
    {
        if(_enemies.Count == 0)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        _wave++;
        int count = Mathf.RoundToInt(_wave * 1.5f);
        for (int i = 0; i < count; i++)
        {
            EnemyData enemyData = enemyDatas[Random.Range(0, enemyDatas.Length)];
            Enemy enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Enemy newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            newEnemy.Setup(enemyData);
            _enemies.Add(newEnemy);

            newEnemy.OnDead.AddListener(() => _enemies.Remove(newEnemy));
            if(enemyPrefab.CompareTag("BigEnemy"))
            {
                newEnemy.OnDead.AddListener(() => gameManager.AddScore(200));
            }
            else
            {
                newEnemy.OnDead.AddListener(() => gameManager.AddScore(100));
            }
            newEnemy.OnDead.AddListener(() => gameManager.AddScore(50));
            newEnemy.OnDead.AddListener(() => Destroy(newEnemy.gameObject, 5f));
        }
    }
}