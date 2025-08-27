using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;
using UnityEngine.UIElements;

public class OrbsSpawner : MonoBehaviour
{
    public int numberOfOrbsToSpawn = 5;
    public GameObject orbPrefab;
    public float height;

    public List<GameObject> spawnedOrbs;

    public int maxNumberOfTry = 100;
    private int currentNumberOfTry = 0;

    public static OrbsSpawner instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        MRUK.Instance.RegisterSceneLoadedCallback(SpawnOrbs);
    }

    public void DestroyOrb(GameObject orb)
    {
        spawnedOrbs.Remove(orb);
        Destroy(orb);

        if (spawnedOrbs.Count == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    public void SpawnOrbs()
    {
        for (int i = 0; i < numberOfOrbsToSpawn; i++)
        {
            Vector3 randomPosition = Vector3.zero;

            MRUKRoom room = MRUK.Instance.GetCurrentRoom();

            while(currentNumberOfTry < maxNumberOfTry)
            {
                #pragma warning disable CS0618 // LabelFilter.Included is obsolete in newer MRUK versions
                bool hasFound = room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_UP,
                    1, LabelFilter.Included(MRUKAnchor.SceneLabels.FLOOR), out randomPosition, out Vector3 n);
                #pragma warning restore CS0618

                if (hasFound)
                {
                    break;
                }

                currentNumberOfTry++;
            }

            randomPosition.y = height;

            GameObject spawned = Instantiate(orbPrefab, randomPosition, Quaternion.identity);

            spawnedOrbs.Add(spawned);
        }
    }

}
