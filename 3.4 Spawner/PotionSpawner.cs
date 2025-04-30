using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PotionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _PotionPrefabs;

    private Bounds _landBounds;
    
    void Start()
    {
        //if (MapManager.Instance != null)
        //{
        //    _landBounds = MapManager.Instance.LandBounds;
        //}
    }

    public void MakePotions()
    {
        int potionCount = Random.Range(10, 20);

        for(int i = 0; i < potionCount; i++)
        {
            Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();
            if(spawnPos != Vector3.zero)
            {
                GameObject potions = Instantiate(_PotionPrefabs, spawnPos, Quaternion.identity);
                MapManager.Instance.AdjustOnNavMesh(potions, spawnPos.y);
            }
        }
    }

    //public void adjustWeaponPosition(GameObject potionTypes, float navMeshY)
    //{
    //    Renderer potionRenderer = potionTypes.GetComponent<Renderer>();
    //    if (potionRenderer != null)
    //    {
    //        float bottomY = potionRenderer.bounds.min.y;
    //        float offsetY = navMeshY - bottomY;
    //        potionTypes.transform.position += new Vector3(0, offsetY, 0);
    //    }
    //}
}
