using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level_Manager : MonoBehaviour
{
    private static Level_Manager instance;
    public static Level_Manager Instance => instance;

    [Header("Level Management")] [SerializeField]
    private List<PoolType> whatPoolTypes = new List<PoolType>();

    [SerializeField] private Transform spawnTransform;

    private List<GameObject> activeEnemies = new List<GameObject>();

    [Header("Level Data Management")] [SerializeField]
    private MeshRenderer levelMeshRenderer;

    [SerializeField] private List<Level_Handler> levelHandlers;
    private int currentLevelHandlerIndex;
    public int currentLevelIndex => currentLevelHandlerIndex;
    private Difficulty currentDifficulty;

    public static Action OnGameOver;
    public static Action OnGameWin;
    public static Action OnLevelStart;
    public static Action<int, int> RoundScoreCalculated;


    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Level_Handler.OnCollisionAction += CollisionLogic;
        
        levelHandlers.ForEach(level => level.gameObject.SetActive(false));
    }

    private Vector2 GetRandomSpawnPoint(float radius)
    {
        Vector2 randomPoint;
        do
        {
            // Get random point inside unit circle
            randomPoint = Random.insideUnitCircle * radius;
        } 
        // Repeat if point is too close to origin (player position)
        while (Mathf.Abs(randomPoint.x) <= 0.2f && Mathf.Abs(randomPoint.y) <= 0.2f);
    
        return randomPoint;
    }

    public void StartLevel(int levelIndex, Difficulty difficulty = Difficulty.Normal)
    {
        if (levelIndex < 0 || levelIndex >= levelHandlers.Count)
        {
#if UNITY_EDITOR
            Debug.LogError("Invalid level index: " + levelIndex);
#endif
            return;
        }

        Menu_Manager.Instance.SwitchMenu(MenuState.InGame);
        levelHandlers[currentLevelHandlerIndex].gameObject.SetActive(false);
        OnLevelStart?.Invoke();
        Time.timeScale = 1;

        currentLevelHandlerIndex = levelIndex;
        currentDifficulty = difficulty;
        Player_Manager.Instance.MovementHandler.ResetPlayer();

        float spawnRadius = levelHandlers[levelIndex].LevelData.SpawnRadius;
        for (var i = 0; i < levelHandlers[levelIndex].LevelData.NormalDifficultyEnemyAmount; i++)
        {
            var enemy = Pool_Manager.Instance.GetObjectFromPool(PoolType.Enemy_01);
            var randomPoint = GetRandomSpawnPoint(spawnRadius);
            enemy.transform.position = new Vector3(randomPoint.x, enemy.transform.position.y, randomPoint.y);
            activeEnemies.Add(enemy);
        }

        levelMeshRenderer.material = levelHandlers[levelIndex].LevelData.LevelMaterial;
        levelHandlers[currentLevelHandlerIndex].gameObject.SetActive(true);
    }

    public void StartLevel()
    {
        StartLevel(currentLevelHandlerIndex, currentDifficulty);
    }

    private void OnActiveEnemyDied(GameObject obj)
    {
        for (var i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i] != obj.gameObject) continue;
            activeEnemies[i].gameObject.transform.position = spawnTransform.position;
            activeEnemies.RemoveAt(i);
            Pool_Manager.Instance.ReturnToPool(obj.gameObject, PoolType.Enemy_01);
            break;
        }

        if (activeEnemies.Count != 0) return;
        //Level Complete
        OnGameWin?.Invoke();
        CalculateStars();
        ResetLevel();
    }

    private void CollisionLogic(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            OnActiveEnemyDied(other.gameObject);
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            Player_Manager.Instance.MovementHandler.StatHandler.PlayerDied();
            var health = Player_Manager.Instance.MovementHandler.StatHandler.Health;
            if (health > 0)
            {
                Player_Manager.Instance.MovementHandler.ResetPlayer(health);
            }
            else
            {
                InvokeOnGameOver();
            }
        }
    }

    private void CalculateStars()
    {
        var starsCount = 1;

        if (Timer_Handler.CanGetExtraStar)
        {
            starsCount++;
            if (Player_Manager.Instance.MovementHandler.StatHandler.Health == 3)
            {
                starsCount++;
            }
        }
#if UNITY_EDITOR
        Debug.Log("level index: " + currentLevelHandlerIndex + " stars earned: " + starsCount);
#endif
        RoundScoreCalculated?.Invoke(currentLevelHandlerIndex, starsCount);
    }

    public void InvokeOnGameOver()
    {
        RoundScoreCalculated?.Invoke(currentLevelHandlerIndex, 0);
        OnGameOver?.Invoke();
        ResetLevel();
        Debug.Log("Game Over");
    }

    public void ResetLevel()
    {
        foreach (var enemy in activeEnemies)
        {
            Pool_Manager.Instance.ReturnToPool(enemy, PoolType.Enemy_01);
        }

        activeEnemies.Clear();
        Player_Manager.Instance.MovementHandler.ResetPlayer(-1);
        Pickup_Base.ReturnAllPickups?.Invoke();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        Level_Handler.OnCollisionAction -= CollisionLogic;
    }
}

public enum Difficulty
{
    Normal = 0,
    Infinite = 1,
}