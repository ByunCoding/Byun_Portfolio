using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.UIElements;

public enum WeaponsType
{
    Sword,
    Rifles,
}

public class WeaponsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _SwordPrefabs;
    [SerializeField] private GameObject[] _RiflePrefabs;

    private Bounds _landBounds;

    void Start()
    {
        if (MapManager.Instance != null)
        {
            _landBounds = MapManager.Instance.LandBounds;
        }

        //MakeSwords();
        //MakeRifles();
    }

    public void MakeSwords(int swordCount)
    {
        List<GameObject> swordList = new List<GameObject>(_SwordPrefabs);

        for (int i = 0; i < swordCount; i++)
        {
            int randomIndex = Random.Range(0, swordList.Count);
            GameObject swords = swordList[randomIndex];
            swordList.RemoveAt(randomIndex);

            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();
            if (spawnPos != Vector3.zero)
            {
                GameObject weapons = Instantiate(swords, spawnPos, Quaternion.Euler(90.0f, 0.0f, 0.0f));
                MapManager.Instance.AdjustOnNavMesh(weapons, spawnPos.y);
            }
        }
    }

    public void MakeSwords()
    {
        int swordCount = Random.Range(5, _SwordPrefabs.Length);

        List<GameObject> swordList = new List<GameObject>(_SwordPrefabs);

        for (int i = 0; i < swordList.Count; i++)
        {
            int randomIndex = Random.Range(0, swordList.Count);
            GameObject swords = swordList[randomIndex];
            swordList.RemoveAt(randomIndex);

            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();
            if (spawnPos != Vector3.zero)
            {
                GameObject weapons = Instantiate(swords, spawnPos, Quaternion.Euler(90.0f, 0.0f, 0.0f));
                MapManager.Instance.AdjustOnNavMesh(weapons, spawnPos.y);
            }
        }
    }

    public void MakeRifles()
    {
        int rifleCount = Random.Range(1, _RiflePrefabs.Length);

        List<GameObject> rifleList = new List<GameObject>(_RiflePrefabs);

        for (int i = 0; i < rifleList.Count; i++)
        {
            int randomIndex = Random.Range(0, rifleList.Count);
            GameObject rifles = rifleList[randomIndex];
            rifleList.RemoveAt(randomIndex);

            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();
            if (spawnPos != Vector3.zero)
            {
                GameObject weapons = Instantiate(rifles, spawnPos, Quaternion.Euler(19.144f, 0.0f, -37.88f));
                MapManager.Instance.AdjustOnNavMesh(weapons, spawnPos.y);
            }
        }
    }

    public void MakeRifles(int rifleCount)
    {
        List<GameObject> rifleList = new List<GameObject>(_RiflePrefabs);

        for (int i = 0; i < rifleCount; i++)
        {
            int randomIndex = Random.Range(0, rifleList.Count);
            GameObject rifles = rifleList[randomIndex];
            rifleList.RemoveAt(randomIndex);

            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();
            if (spawnPos != Vector3.zero)
            {
                GameObject weapons = Instantiate(rifles, spawnPos, Quaternion.Euler(19.144f, 0.0f, -37.88f));
                MapManager.Instance.AdjustOnNavMesh(weapons, spawnPos.y);
            }
        }
    }

    //private void adjustWeaponPosition(GameObject weaponTypes, float navMeshY)
    //{
    //    Renderer weaponRenderer = weaponTypes.GetComponent<Renderer>();
    //    if(weaponRenderer != null)
    //    {
    //        float bottomY = weaponRenderer.bounds.min.y;
    //        float offsetY = navMeshY - bottomY;
    //        weaponTypes.transform.position += new Vector3(0, offsetY, 0);
    //    }
    //}
}