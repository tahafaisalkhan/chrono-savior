using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;
    // [SerializeField] private BossScript boss;
    [SerializeField] private GameObject basicRobotPrefab;
    [SerializeField] private GameObject shieldedRobotPrefab;
    [SerializeField] private GameObject gunRobotPrefab;
    [SerializeField] private GameObject explosiveRobotPrefab;
    [SerializeField] private GameObject heavyRobotPrefab;
    [SerializeField] private GameObject securityTurretPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private TriggerEnter[] triggers;
    [SerializeField] private DoorAnimate[] doors;
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private GameObject powerupRoom;
    [SerializeField] private GameCompleteScript gameComplete;
    [SerializeField] private List<OnPowerupInteract> powerUps = new List<OnPowerupInteract>();
    private bool[] triggered = {false,false,false,false,false};
    private const int  ROOM_1 = 1;
    private const int ROOM_2 = 2;
    private const int ROOM_3 = 3;
    private const int ROOM_4 = 4;
    private const int ROOM_5 = 5;
    private int[] enemies = {8,12,16,25};
    private int currentWave = 0;
    private int enemiesRemaining = 0;

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        // if(boss != null)
        // {
        //     boss.OnSpawnEnemies += SpawnMinions;
        //     Debug.Log("test");
        // }
        TriggerEnter.onEnter += OnEnter;
        enemiesRemaining = enemies[currentWave];
    }

    void OnEnter(int index)
    {
        if(index < 0 || index > 5)
        {
            Debug.LogError("Invalid index: " + index);
            return;
        }

        if(triggered[index - 1])
        {
            return;
        }
        triggered[index - 1] = true;

        if(index == ROOM_1)
        {
            HandleRoom1();
        }
        else if(index == ROOM_2)
        {
            HandleRoom2();
        }
        else if(index == ROOM_3)
        {
            HandleRoom3();
        }
        else if(index == ROOM_4)
        {
            HandleRoom4();
        }
        else if(index == ROOM_5)
        {
            HandleRoom5();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(currentWave == 4 && enemiesRemaining == 0)
        {
            StartCoroutine(OnLevelComplete());
            
        }
    }

    IEnumerator OpenDoorCo(int index,float time)
    {
        yield return new WaitForSeconds(time);
        if(index <= doors.Length)
        {
            doors[index-1].OpenDoor();
        }
    }

    IEnumerator OnLevelComplete()
    {
        yield return new WaitForSeconds(5f);
        if(gameComplete != null)
        {
            gameComplete.ShowGameCompleteScreen();
        }
        else
        {
            Debug.LogError("GameCompleteScript not found in StoryManager.");
        }
        
        if(StateManagement.Instance != null)
        {
            StateManagement.Instance.SetSessionLoad(false);
            StateManagement.Instance.SetTokens(0);
            int total_coins = StateManagement.Instance.GetCoins();
            if(Player.Instance != null)
            {
                total_coins += Player.Instance.GetCoins();
            }
            StateManagement.Instance.SetCoins(total_coins);
            AchievementManager.Instance.CheckLocked("Mission Master");
            if (!Player.Instance.BossDamage)
            {
                AchievementManager.Instance.CheckLocked("Boss Conqueror");
            }
            if (!Player.Instance.DamageTaken)
            {
                AchievementManager.Instance.CheckLocked("Untouchable");
            }
            if (Player.Instance.GetCurrentHealth()< (0.25*Player.Instance.MAXHEALTH)){
                AchievementManager.Instance.CheckLocked("Fearless Fighter");
            }
        }
        
    }
    void HandleRoom1()
    {
        StartCoroutine(OpenDoorCo(ROOM_1,0.5f));
    }   

    void HandleRoom2()
    {
        doors[ROOM_1-1].CloseDoor();
        StartCoroutine(SpawnEnemiesCo(ROOM_2));
    }

    void HandleRoom3()
    {
        doors[ROOM_2-1].CloseDoor();
        StartCoroutine(SpawnEnemiesCo(ROOM_3));
    }

    void HandleRoom4()
    {
        doors[ROOM_3-1].CloseDoor();
        SpawnPowerups();
        
        StartCoroutine(SpawnEnemiesCo(ROOM_5));
        if(Random.Range(0,10) == 0)
        {
            StartCoroutine(OpenDoorCo(ROOM_4,5f));
        }
        
    }

    void HandleRoom5()
    {
        doors[ROOM_5-1].CloseDoor();
        StartCoroutine(SpawnEnemiesCo(6));
    }

    

    public void DecreaseEnemyCount()
    {
        enemiesRemaining--;
    }

    private void SpawnEnemiesForWave(GameObject[] spawnPoints)
    {
        List<GameObject> enemiesList = new List<GameObject>();
        int enemiesToSpawn = enemies[currentWave];
        if (currentWave == 3)
        {
            AddEnemies(enemiesList, bossPrefab, 1);
        }
        else if (currentWave == 2)
        {
            AddEnemies(enemiesList, basicRobotPrefab, enemiesToSpawn * 50 / 100);
            AddEnemies(enemiesList, shieldedRobotPrefab, enemiesToSpawn * 25 / 100);
            AddEnemies(enemiesList, gunRobotPrefab, enemiesToSpawn * 25 / 100);
        }
        else if (currentWave == 1)
        {
            AddEnemies(enemiesList, basicRobotPrefab, enemiesToSpawn * 25 / 100);
            AddEnemies(enemiesList, shieldedRobotPrefab, enemiesToSpawn * 75 / 100);
        }
        else
        {
            AddEnemies(enemiesList, basicRobotPrefab, enemiesToSpawn * 75 / 100);
            AddEnemies(enemiesList, shieldedRobotPrefab, enemiesToSpawn * 25 / 100);
            // AddEnemies(enemiesList, securityTurretPrefab, enemiesToSpawn * 20 /100);
        }

        SpawnAtPoints(enemiesList, spawnPoints);
    }

    private void AddEnemies(List<GameObject> enemiesList, GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            enemiesList.Add(prefab);
        }
    }

    private void SpawnAtPoints(List<GameObject> enemiesList, GameObject[] spawnPoints)
    {
        Vector2 randomJitter = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f),0);
        foreach (GameObject enemy in enemiesList)
        {
            GameObject spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            Instantiate(enemy, new Vector2 (spawnPoint.transform.position.x + randomJitter.x, + spawnPoint.transform.position.y + randomJitter.y) , spawnPoint.transform.rotation);
        }

    }

    IEnumerator SpawnEnemiesCo(int roomNo)
    {
        int childCount = rooms[currentWave].transform.childCount;
        
        GameObject[] spawnPoints = new GameObject[childCount];
        for (int i = 0; i < childCount; i++)
        {
            spawnPoints[i] = rooms[currentWave].transform.GetChild(i).gameObject;
        }
        SpawnEnemiesForWave(spawnPoints);
        
        while(enemiesRemaining != 0)
        {
            yield return null;
        }
        currentWave++;
        if(currentWave == 4)
        {
            enemiesRemaining = 0;
            yield break;
        }
        else
        {
            enemiesRemaining = enemies[currentWave];
        }
        
        Debug.Log("Wave over!");
        if(roomNo < 6)
        {
            StartCoroutine(OpenDoorCo(roomNo,2f));
        }
        
    }

    public void SpawnMinions()
    {
        Debug.Log("test");
        List<GameObject> enemiesList = new List<GameObject>();

        int enemiesToSpawn = enemies[currentWave] - 1;

        AddEnemies(enemiesList, shieldedRobotPrefab, enemiesToSpawn * 25 / 100);
        AddEnemies(enemiesList, gunRobotPrefab, enemiesToSpawn * 25 / 100);
        AddEnemies(enemiesList, explosiveRobotPrefab, enemiesToSpawn * 25 / 100);
        AddEnemies(enemiesList, heavyRobotPrefab, enemiesToSpawn * 25 / 100);

        GameObject[] spawnPoints = new GameObject[rooms[currentWave].transform.childCount];
        for (int i = 0; i < rooms[currentWave].transform.childCount; i++)
        {
            spawnPoints[i] = rooms[currentWave].transform.GetChild(i).gameObject;
        }
        SpawnAtPoints(enemiesList,spawnPoints);
    }
    void SpawnPowerups()
    {
        Transform poweruproom = powerupRoom.transform;
        for(int i = 0; i < poweruproom.childCount; i++)
        {
            GameObject spawnPoint = poweruproom.GetChild(i).gameObject;
            int randomIndex = Random.Range(0, powerUps.Count);
            Instantiate(powerUps[randomIndex], spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }

    void OnDisable()
    {
        TriggerEnter.onEnter -= OnEnter;
        // boss.OnSpawnEnemies -= SpawnMinions;
    }
}