using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager singleton;

    public List<Transform> spawns = new List<Transform>();

    void Awake()
    {
        if (singleton is null)
        {
            singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform RandomSpawn()
    {
        Transform t = spawns[Mathf.FloorToInt(Random.Range(0, spawns.Count))];
        spawns.Remove(t);
        return t;
    }


}
