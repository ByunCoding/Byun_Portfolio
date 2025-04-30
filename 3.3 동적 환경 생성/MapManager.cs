using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MapManager : Singleton<MapManager>
{
    public Bounds LandBounds { set; get; }

    [SerializeField] public GameObject[] _landObject;

    void Awake()
    {
        SelectRandomMap();
    }

    public void SelectRandomMap()
    {
        if (_landObject != null && _landObject.Length > 0)
        {
            int randomIndex = Random.Range(0, _landObject.Length);
            GameObject startLand = _landObject[randomIndex];

            if (startLand != null)
            {
                Renderer[] renderers = startLand.GetComponentsInChildren<Renderer>();

                if (renderers.Length > 0)
                {
                    Bounds combinedBounds = renderers[0].bounds;
                    for(int i = 1; i < renderers.Length; i++)
                    {
                        combinedBounds.Encapsulate(renderers[i].bounds);
                    }

                    LandBounds = combinedBounds;
                }
            }
        }
    }

    public Vector3 GetRandomPositionOnNavMesh()
    {
        while (true)
        {
            float randomX = Random.Range(LandBounds.min.x, LandBounds.max.x);
            float randomZ = Random.Range(LandBounds.min.z, LandBounds.max.z);

            Vector3 randomPoint = new Vector3(randomX, LandBounds.max.y + 10f, randomZ);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }   
        }
    }

    public void AdjustOnNavMesh(GameObject obj, float navMeshY)
    {
        Renderer objRenderer = obj.GetComponent<Renderer>();
        if (objRenderer != null)
        {
            float additionalOffset = 0;

            if (obj.CompareTag("Rifle"))
            {
                additionalOffset = 0.2f;
            }

            float bottomY = objRenderer.bounds.min.y;
            float offsetY = navMeshY - bottomY + additionalOffset;
            obj.transform.position += new Vector3(0, offsetY, 0);
        }
    }

    //public Vector3 GetRandomPositionOnNavMesh()
    //{
    //    Vector3 navMeshPosition = Vector3.zero;
    //    bool foundnavMeshPosition = false;

    //    while(!foundnavMeshPosition)
    //    {
    //        float randomX = Random.Range(LandBounds.min.x, LandBounds.max.x);
    //        float randomZ = Random.Range(LandBounds.min.z, LandBounds.max.z);

    //        Vector3 randomPoint = new Vector3(randomX, LandBounds.max.y + 10f, randomZ);
    //        NavMeshHit hit;
    //        if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
    //        {
    //            navMeshPosition = hit.position;
    //            foundnavMeshPosition = true;
    //        }

    //    }

    //    return navMeshPosition;
    //}

}
