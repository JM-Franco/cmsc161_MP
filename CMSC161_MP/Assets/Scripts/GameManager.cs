using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public Transform[] playerSpawnPoints;

    public int levelCounter = 1;

    public int keysCollected = 0;
    public int keysRequired = 3;

    public GameObject keyPrefab;
    public GameObject flashlightPrefab;
    public GameObject[] itemsPrefab;

    public GameObject enemyPrefab;
    public int numberOfEnemiesToSpawn = 5; 
    public float minDistanceBetweenEnemies = 10f; 

    public GameObject exitArea;

    public float difficultyScale = 1.5f;

    public float currentTime;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentTime = 0f;
        player = GameObject.Find("FirstPersonPlayer");
        exitArea = GameObject.FindObjectsOfType<ExitArea>(true)[0].gameObject;
        // SpawnEnemies();
        // SpawnItems();
        // SpawnPlayer();
        SceneManager.sceneLoaded += OnSceneLoaded;
        Time.timeScale = 1f;
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (keysCollected >= keysRequired) exitArea.SetActive(true);
        
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    public void PlayTime()
    {
        Time.timeScale = 1f;
    }

    public void ChangeTimeScale(float targetTimeScale, float duration)
    {
        StartCoroutine(GradualTimeScaleChange(targetTimeScale, duration));
    }

    private IEnumerator GradualTimeScaleChange(float targetTimeScale, float duration)
    {
        float currentTimeScale = Time.timeScale;
        float startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime < duration)
        {
            float progress = (Time.realtimeSinceStartup - startTime) / duration;
            Time.timeScale = Mathf.Lerp(currentTimeScale, targetTimeScale, progress);

            yield return null; // Wait for the next frame
        }

        Time.timeScale = targetTimeScale;
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            Vector3 randomPoint = GetRandomNavMeshPoint();
            Instantiate(enemyPrefab, randomPoint, Quaternion.identity);
        }
    }

    void SpawnItems()
    {
        Instantiate(flashlightPrefab, GetRandomNavMeshPoint(), Quaternion.identity);

        for (int i = 0; i < keysRequired; i++)
        {
            Instantiate(keyPrefab, GetRandomNavMeshPoint(), Quaternion.identity);
        }

        for (int i = 0; i < keysRequired * 2; i++)
        {
            int itemID = UnityEngine.Random.Range(0, itemsPrefab.Length - 1);
            Vector3 randomPoint = GetRandomNavMeshPoint();
            Instantiate(itemsPrefab[itemID], randomPoint, Quaternion.identity);
        }
    }

    void SpawnPlayer()
    {
        int randSpawnLocation = UnityEngine.Random.Range(0, playerSpawnPoints.Length);
        player.transform.position = playerSpawnPoints[randSpawnLocation].gameObject.transform.position;
    }

    Vector3 GetRandomNavMeshPoint()
    {
        NavMeshHit hit;
        Vector3 randomPoint;

        // Generate a random point on the entire NavMesh with minimum distance constraint
        do
        {
            randomPoint = new Vector3(UnityEngine.Random.Range(NavMeshBounds.min.x, NavMeshBounds.max.x),
                                      UnityEngine.Random.Range(NavMeshBounds.min.y, NavMeshBounds.max.y),
                                      UnityEngine.Random.Range(NavMeshBounds.min.z, NavMeshBounds.max.z));
        } while (!NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas) || IsTooCloseToExistingEnemy(randomPoint));

        return hit.position;
    }

    bool IsTooCloseToExistingEnemy(Vector3 point)
    {
        // Check if the point is too close to any existing enemies
        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy"); // Assuming enemies have the "Enemy" tag

        foreach (var enemy in existingEnemies)
        {
            if (Vector3.Distance(point, enemy.transform.position) < minDistanceBetweenEnemies)
            {
                return true; // Too close to an existing enemy
            }
        }

        return false; // Not too close to any existing enemy
    }

    Bounds NavMeshBounds
    {
        get
        {
            // Calculate the bounds of the entire NavMesh
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (Vector3 vertex in triangulation.vertices)
            {
                min = Vector3.Min(min, vertex);
                max = Vector3.Max(max, vertex);
            }

            return new Bounds((max + min) * 0.5f, max - min);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);

        Time.timeScale = 1f;

        exitArea = GameObject.FindObjectsOfType<ExitArea>(true)[0].gameObject;
        player = GameObject.Find("FirstPersonPlayer");
        levelCounter++;
        keysCollected = 0;
        if (numberOfEnemiesToSpawn < 20 && keysRequired < 20)
        {
            numberOfEnemiesToSpawn = (int)Math.Ceiling(numberOfEnemiesToSpawn * difficultyScale);
            keysRequired = (int)Math.Ceiling(keysRequired * difficultyScale);
        }

        Debug.Log("Spawning Entities");
        SpawnEnemies();
        SpawnItems();
        SpawnPlayer();
    }
}
