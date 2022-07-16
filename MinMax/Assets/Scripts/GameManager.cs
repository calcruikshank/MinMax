using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager g;
    public readonly List<GameObject> Players = new List<GameObject>();
    public readonly List<GameObject> Bullets = new List<GameObject>();
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
            Players.Add(player.gameObject);
    }
    public void RemovePlayer(PlayerController player){
        Players.Remove(player.gameObject);
    }

    public void AddBullet(Bullet bullet){
        Bullets.Add(bullet.gameObject);
    }
    public void RemoveBullet(Bullet bullet){
        Bullets.Remove(bullet.gameObject);
    }
}
