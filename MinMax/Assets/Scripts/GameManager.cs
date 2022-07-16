using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager g;
    public readonly List<PlayerController> Players;

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

    public void AddPlayer(PlayerController player){
        Players.Add(player);
    }
}
