using System.Collections;
using System.Collections.Generic; // Import List
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    public GameObject fighterShipPrefab;
    public GameObject bomberShipPrefab;
    public GameObject sniperShipPrefab;

    private int[] fighterShipsPerWave = {3, 2, 0, 2, 0};
    private int[] bomberShipsPerWave = {0, 0, 3, 3, 5};
    private int[] sniperShipsPerWave = {0, 3, 2, 2, 0};

    private int currentWave = 0; // Current wave index
    private int enemiesRemaining; // Number of enemies remaining in the current wave

    public float minSpacing = 0.5f; // Minimum spacing between ships

    private List<Vector3> spawnedPositions = new List<Vector3>(); // List to keep track of spawned positions

    private void Start()
    {
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (currentWave < fighterShipsPerWave.Length)
        {
            enemiesRemaining = fighterShipsPerWave[currentWave] + bomberShipsPerWave[currentWave] + sniperShipsPerWave[currentWave];
            StartCoroutine(SpawnWave());
        }
        else
        {
            Debug.Log("All waves cleared!");
            // Handle game completion or restart here
        }
    }

    private IEnumerator SpawnWave()
    {
        spawnedPositions.Clear(); // Clear the list of positions for the new wave

        // Spawn fighter ships
        for (int i = 0; i < fighterShipsPerWave[currentWave]; i++)
        {
            Vector3 spawnPosition = CalculateSpawnPosition();
            Instantiate(fighterShipPrefab, spawnPosition, Quaternion.identity);
            spawnedPositions.Add(spawnPosition); // Add position to the list
            yield return new WaitForSeconds(0.5f); // Adjust spawn rate as needed
        }

        // Spawn bomber ships
        for (int i = 0; i < bomberShipsPerWave[currentWave]; i++)
        {
            Vector3 spawnPosition = CalculateSpawnPosition();
            Instantiate(bomberShipPrefab, spawnPosition, Quaternion.identity);
            spawnedPositions.Add(spawnPosition); // Add position to the list
            yield return new WaitForSeconds(0.5f); // Adjust spawn rate as needed
        }

        // Spawn sniper ships
        for (int i = 0; i < sniperShipsPerWave[currentWave]; i++)
        {
            Vector3 spawnPosition = CalculateSpawnPosition();
            Instantiate(sniperShipPrefab, spawnPosition, Quaternion.identity);
            spawnedPositions.Add(spawnPosition); // Add position to the list
            yield return new WaitForSeconds(0.5f); // Adjust spawn rate as needed
        }

        // Wait until all enemies are destroyed
        while (enemiesRemaining > 0)
        {
            yield return null;
        }

        // Move to the next wave
        currentWave++;
        yield return new WaitForSeconds(2.0f); // Delay before starting next wave
        StartNextWave();
    }

    private Vector3 CalculateSpawnPosition()
    {
        Vector3 spawnPosition;
        bool validPosition;

        do
        {
            validPosition = true;
            float spawnX = 8.0f;
            float spawnY = Random.Range(-3f, 3f);
            spawnPosition = new Vector3(spawnX, spawnY, 0f);

            // Check if the new spawn position is too close to any existing positions
            foreach (Vector3 pos in spawnedPositions)
            {
                if (Vector3.Distance(spawnPosition, pos) < minSpacing)
                {
                    validPosition = false;
                    break;
                }
            }
        }
        while (!validPosition);

        return spawnPosition;
    }

    // Called by enemy ships when destroyed
    public void EnemyDestroyed()
    {
        enemiesRemaining--;
    }
}