using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGenerationManager : MonoBehaviour
{
    public int numberOfStartingSegments;
    public float spawnNextSegmentDistance;
    public float destoryPreviousSegmentDistance;

    public GameObject[] towerSegmentPrefabs;
    public GameObject baseTowerSegmentPrefab;
    public Transform nextSpawnPos;

    private List<GameObject> currentActiveSegments = new List<GameObject>();
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        SpawnNextSegment(baseTowerSegmentPrefab);

        for (int i = 0; i < numberOfStartingSegments - 1; i++)
            SpawnNextSegment(towerSegmentPrefabs[Random.Range(0, towerSegmentPrefabs.Length)]);
    }

    private void Update()
    {
        if (nextSpawnPos.position.y - player.position.y <= spawnNextSegmentDistance)
            SpawnNextSegment(towerSegmentPrefabs[Random.Range(0, towerSegmentPrefabs.Length)]);
        if (currentActiveSegments[0].transform.position.y <= player.position.y - destoryPreviousSegmentDistance)
        {
            Destroy(currentActiveSegments[0]);
            currentActiveSegments.RemoveAt(0);
        }
    }

    void SpawnNextSegment(GameObject segmentPrefab)
    {
        currentActiveSegments.Add(Instantiate(segmentPrefab, nextSpawnPos.position, Quaternion.identity));
        if (Random.value < 0.5f)
            currentActiveSegments[currentActiveSegments.Count - 1].transform.localScale = new Vector3(-1, 1, 1);
        nextSpawnPos = currentActiveSegments[currentActiveSegments.Count - 1].transform.GetChild(0);
    }
}
