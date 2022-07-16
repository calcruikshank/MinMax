using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager g;
    public readonly List<GameObject> Players = new List<GameObject>();

    void Awake()
    {
        if(g != null){
            Destroy(this);
        }
        g = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayer(GameObject player){
        if(player.GetComponent<PlayerController>() != null){
            Players.Add(player);
        }
    }
    public void RemovePlayer(GameObject player){
        Players.Remove(player);
    }
}
