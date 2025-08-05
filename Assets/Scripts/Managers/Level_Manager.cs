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

    private Vector2 GetRandomSpawnPoint(float spawnRadius)
    {
        // Define the safe radius (e.g., a fraction of spawnRadius)
        var safeRadius = 3f;
        Vector2 spawnPoint;

        do
        {
            // Get a random angle in radians
            var angle = Random.Range(0f, Mathf.PI * 2f);

            // Get a random radius between safe radius and max radius
            var randomRadius = Random.Range(safeRadius, spawnRadius);

            // Convert polar coordinates to Cartesian
            var x = Mathf.Cos(angle) * randomRadius;
            var y = Mathf.Sin(angle) * randomRadius;

            spawnPoint = new Vector2(x, y);
        }
        // Ensure that the spawn point is outside the safe radius
        while (spawnPoint.magnitude < safeRadius);

        return spawnPoint;
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

        Enemy_Movetowards.ToggleKinematic?.Invoke(true);
        float spawnRadius = levelHandlers[levelIndex].LevelData.SpawnRadius;
        for (var i = 0; i < levelHandlers[levelIndex].LevelData.NormalDifficultyEnemyAmount; i++)
        {
            var enemy = Pool_Manager.Instance.GetObjectFromPool(PoolType.Enemy_01);
            var randomPoint = GetRandomSpawnPoint(spawnRadius);
            enemy.transform.position = new Vector3(randomPoint.x, enemy.transform.position.y, randomPoint.y);
            activeEnemies.Add(enemy);
        }
        Enemy_Movetowards.ToggleKinematic?.Invoke(false);

        levelMeshRenderer.material = levelHandlers[levelIndex].LevelData.LevelMaterial;
        levelHandlers[currentLevelHandlerIndex].gameObject.SetActive(true);
    }

    public void StartLevel()
    {
        StartLevel(currentLevelHandlerIndex, currentDifficulty);
    }

    private void OnActiveEnemyDied(GameObject obj)
    {
        Enemy_Movetowards.ToggleKinematic?.Invoke(true);
        for (var i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i] != obj.gameObject) continue;
            activeEnemies[i].gameObject.transform.position = spawnTransform.position;
            activeEnemies.RemoveAt(i);
            Pool_Manager.Instance.ReturnToPool(obj.gameObject, PoolType.Enemy_01);
            break;
        }
        Enemy_Movetowards.ToggleKinematic?.Invoke(false);

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
        Enemy_Movetowards.ToggleKinematic?.Invoke(true);
        foreach (var enemy in activeEnemies)
        {
            enemy.gameObject.transform.position = spawnTransform.position;
            Pool_Manager.Instance.ReturnToPool(enemy, PoolType.Enemy_01);
        }
        Enemy_Movetowards.ToggleKinematic?.Invoke(false);

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