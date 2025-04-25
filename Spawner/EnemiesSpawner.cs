using UnityEngine;
using UnityEngine.AI;

public enum MonsterType
{
    Golem,
    Zombie
}


public class EnemiesSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _GolemPrefabs;
    [SerializeField] private GameObject[] _ZombiePrefabs;

    protected Bounds _landBounds;

    private const int MAXGOLEMCOUNT = 3;
    private const int MAXZOMBIECOUNT = 4;

    void Start()
    {
        if (MapManager.Instance != null)
        {
            _landBounds = MapManager.Instance.LandBounds;
        }
    }

    // Zombie수 제한
    public void MakeZombies(int zombieCount)
    {
        for (int i = 0; i < zombieCount; i++)
        {
            GameObject zombies = _ZombiePrefabs[Random.Range(0, _ZombiePrefabs.Length)];
            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();
            if (spawnPos != Vector3.zero)
            {
                Instantiate(zombies, spawnPos, Quaternion.identity);
            }
        }
    }

    // Golem수 제한
    public void MakeGolems(int golemsCount)
    {
        for (int i = 0; i < golemsCount; i++)
        {
            GameObject golems = _ZombiePrefabs[Random.Range(0, _GolemPrefabs.Length)];
            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();
            if (spawnPos != Vector3.zero)
            {
                Instantiate(golems, spawnPos, Quaternion.identity);
            }
        }
    }

    // Golem만 스폰
    public void MakeGolems()
    {
        int golemCount = Random.Range(4, 9);

        for (int i = 0; i < golemCount; i++)
        {
            GameObject golems = _GolemPrefabs[Random.Range(0, _GolemPrefabs.Length)];
            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();
            if (spawnPos != Vector3.zero)
            {
                Instantiate(golems, spawnPos, Quaternion.identity);
            }
        }
    }

    // Zombie만 스폰
    public void MakeZombies()
    {
        int zombieCount = Random.Range(4, 9);

        for (int i = 0; i < zombieCount; i++)
        {
            GameObject zombies = _ZombiePrefabs[Random.Range(0, _ZombiePrefabs.Length)];
            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();
            if (spawnPos != Vector3.zero)
            {
                Instantiate(zombies, spawnPos, Quaternion.identity);
            }
        }
    }


    public void MakeMonsters()
    {
        int golemCount = Random.Range(5, 11);
        int zombieCount = Random.Range(5, 11);
        
        // Golem 스폰
        for(int i = 0; i < golemCount; i++)
        {
            GameObject golems = _GolemPrefabs[Random.Range(0, _GolemPrefabs.Length)];
            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();
            if(spawnPos != Vector3.zero)
            {
                Instantiate(golems, spawnPos, Quaternion.identity);
            }
        }

        // Zombie 스폰
        for (int i = 0; i < zombieCount; i++)
        {
            GameObject zombies = _ZombiePrefabs[Random.Range(0, _ZombiePrefabs.Length)];
            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();
            if (spawnPos != Vector3.zero)
            {
                Instantiate(zombies, spawnPos, Quaternion.identity);
            }
        }
    }

    public void MixMakeMonsters()
    {
        int totalCount = Random.Range(5, 11);

        for (int i = 0; i < totalCount; i++)
        {

            bool isGolem = Random.value > 0.5f;

            GameObject monster;

            if (isGolem)
            {
                monster = _GolemPrefabs[Random.Range(0, _GolemPrefabs.Length)];
            }
            else
            {
                monster = _ZombiePrefabs[Random.Range(0, _ZombiePrefabs.Length)];
            }

            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();

            if (spawnPos != Vector3.zero)
            {
                Instantiate(monster, spawnPos, Quaternion.identity);
            }
        }

    }
}
