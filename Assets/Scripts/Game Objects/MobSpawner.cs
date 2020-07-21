using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> spawnPoints = new List<GameObject>();
    private List<GameObject> spawnedMobs = new List<GameObject>();
    [SerializeField] private Vector2 spawnerOffsetPosition;
    [SerializeField] private int spawnInterval;
    [SerializeField] private int mobCap;

    private bool spawning;
    private Transform targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(spawnerOffsetPosition.x + targetPosition.position.x, spawnerOffsetPosition.y);
        SpawnMobs();
    }
    
    void SpawnMobs()
    {
        while(!spawning)
        {
            StartCoroutine(SpawnTimer(spawnInterval));
        }
    }
    IEnumerator SpawnTimer(float time)
    {
        spawning = true;
        foreach (GameObject spawner in spawnPoints)
        {
            foreach (GameObject mob in prefabs)
            {
                if (prefabs.Count > 0 && mobCap > spawnedMobs.Count)
                {
                    GameObject newMob = Instantiate(mob, spawner.transform.position, Quaternion.identity);
                    newMob.GetComponent<Enemy>().Initialize();
                    spawnedMobs.Add(newMob);
                }
                if (mobCap == spawnedMobs.Count)
                {
                    foreach (GameObject enemy in spawnedMobs)
                    {
                        if (!enemy.activeSelf)
                        {
                            Destroy(enemy);
                            spawnedMobs.Remove(enemy);
                            break;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(time);
        }
        spawning = false;
    }
}

