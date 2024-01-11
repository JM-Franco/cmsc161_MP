using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Reference to your enemy prefab
    public int numberOfEnemiesToSpawn = 3; // Adjust as needed
    public float minDistanceBetweenEnemies = 10f; // Minimum distance between spawned enemies

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            Vector3 randomPoint = GetRandomNavMeshPoint();
            Instantiate(enemyPrefab, randomPoint, Quaternion.identity);
        }
    }

    Vector3 GetRandomNavMeshPoint()
    {
        NavMeshHit hit;
        Vector3 randomPoint;

        // Generate a random point on the entire NavMesh with minimum distance constraint
        do
        {
            randomPoint = new Vector3(Random.Range(NavMeshBounds.min.x, NavMeshBounds.max.x),
                                      Random.Range(NavMeshBounds.min.y, NavMeshBounds.max.y),
                                      Random.Range(NavMeshBounds.min.z, NavMeshBounds.max.z));
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
}