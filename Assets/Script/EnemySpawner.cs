using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy[] enemyPrefabs;

    public GameManager gameManager;

    public float _spawnInterval = 7f;

    private List<Enemy> _enemies = new List<Enemy>();
    private float _lastSpawnTime = 0;

    private void Update()
    {
        if(Time.time > _lastSpawnTime + _spawnInterval)
        {
            _lastSpawnTime = Time.time;

            SpawnEnemy(0);
            SpawnEnemy(1);
            SpawnEnemy(2);
        }
    }

    private void SpawnEnemy(int index)
    {
        Enemy enemyPrefab = enemyPrefabs[index];

        Enemy newEnemy = Instantiate(enemyPrefab, enemyPrefab.spawnPoint.position, enemyPrefab.spawnPoint.rotation);
        newEnemy.Setup();
        _enemies.Add(newEnemy);

        newEnemy.OnDead.AddListener(() => _enemies.Remove(newEnemy));
        if (enemyPrefab.CompareTag("BigEnemy"))
        {
            newEnemy.OnDead.AddListener(() => gameManager.AddScore(200));
        }
        else
        {
            newEnemy.OnDead.AddListener(() => gameManager.AddScore(100));
        }
        newEnemy.OnDead.AddListener(() => Destroy(newEnemy.gameObject, 5f));
    }
}