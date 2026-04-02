using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EnemySpawner enemySpawner;

    private int _score = 0;

    public bool IsGameover { get; private set; }

    public void AddScore(int score)
    {
        if (IsGameover)
        {
            return;
        }

        _score += score;
    }

    public void EndGame()
    {
        IsGameover = true;
        enemySpawner.enabled = false;
    }
}