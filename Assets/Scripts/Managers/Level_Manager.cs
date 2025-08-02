using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Level_Manager : MonoBehaviour
{
    private static Level_Manager instance;
    public static Level_Manager Instance => instance;
    
    [Header("Level Management")]
    [SerializeField] private List<PoolType> whatPoolTypes = new List<PoolType>();
    [SerializeField] private Transform spawnTransform;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    
    [Header("Level Data Management")]
    [SerializeField] private MeshRenderer levelMeshRenderer;
    [SerializeField] private List<Level_Handler> levelHandlers;
    private int currentLevelHandlerIndex;
    private Difficulty currentDifficulty ;
    
    public static UnityAction OnGameOver;
    public static UnityAction OnGameWin;
    public static UnityAction OnLevelStart;
    
    //TODO
    //difficulty managemnet + spawn radius
    // amount of enemies per difficulty
    // level switching
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        Level_Handler.OnCollisionAction += CollisionLogic;
    }
    
    public void StartLevel(int levelIndex ,Difficulty difficulty = Difficulty.Normal)
    {
        if (levelIndex < 0 || levelIndex >= levelHandlers.Count)
        {
            Debug.LogError("Invalid level index: " + levelIndex);
            return;
        }
        
        levelHandlers[currentLevelHandlerIndex].gameObject.SetActive(false);
        
        OnLevelStart?.Invoke();
        Time.timeScale = 1;

        currentLevelHandlerIndex = levelIndex;
        currentDifficulty = difficulty;

        Player_Manager.Instance.MovementHandler.ResetPlayer();
        
        for (int i = 0; i < levelHandlers[levelIndex].LevelData.NormalDifficultyEnemyAmount; i++)
        {
            var enemy = Pool_Manager.Instance.GetObjectFromPool(PoolType.Enemy);
            Vector2 randomPoint = Random.insideUnitCircle;
            randomPoint *= levelHandlers[levelIndex].LevelData.SpawnRadius;
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
        for(int i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i] == obj.gameObject)
            {
                activeEnemies.RemoveAt(i);
                Pool_Manager.Instance.ReturnToPool(obj.gameObject, PoolType.Enemy );
                break;
            }
        }
        if (activeEnemies.Count == 0)
        {
            //Level Complete
            OnGameWin?.Invoke();
            ResetLevel();
            Debug.Log("Level Complete");
        }
    }

    private void CollisionLogic(Collision other)
    {
        Debug.Log("Enemy died");
        if (other.gameObject.CompareTag("Enemy"))
        {
            OnActiveEnemyDied(other.gameObject);
            
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            Player_Manager.Instance.MovementHandler.StatHandler.PlayerDied();
            int health = Player_Manager.Instance.MovementHandler.StatHandler.Health;
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
    
    public void InvokeOnGameOver()
    {
        OnGameOver?.Invoke();
        ResetLevel();
        Debug.Log("Game Over");
    }

    public void ResetLevel()
    {
        foreach (var enemy in activeEnemies)
        {
            Pool_Manager.Instance.ReturnToPool(enemy, PoolType.Enemy);
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
