using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _PlayerPrefabs;


    void Start()
    {

    }

    public void MakePlayer()
    {
        Vector3 spawnPos = MapManager.Instance.GetRandomPositionOnNavMesh();

        if(spawnPos != Vector3.zero)
        {
            GameObject player = Instantiate(_PlayerPrefabs, spawnPos, Quaternion.identity);

            Collider col = player.GetComponent<Collider>();
            if (col != null)
            {
                float offsetY = spawnPos.y - col.bounds.min.y;
                player.transform.position += new Vector3(0, offsetY, 0);
            }
        }
    }
}
