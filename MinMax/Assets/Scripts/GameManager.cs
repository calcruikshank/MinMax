using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager g;
    public readonly List<GameObject> Players = new List<GameObject>();
    public readonly List<GameObject> Bullets = new List<GameObject>();
    public GameObject playerPrefab;

    void Awake()
    {
        if(g != null){
            Destroy(this);
        }
        g = this;
    }

    void Start()
    {
        // foreach(PlayerCursor pc in DieRoller.singleton.pcs)
        for (int i = DieRoller.singleton.pcs.Count - 1; i >= 0; i--)
        {
            PlayerCursor pc = DieRoller.singleton.pcs[i];
            NewPlayerInput(playerPrefab, pc.GetComponent<PlayerInput>(), pc.GetComponent<Stats>());
        }

        Destroy(DieRoller.singleton.gameObject);
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

    public void NewPlayerInput(GameObject prefab, PlayerInput newPI, Stats newStats)
    {
        // GameObject prefabToInstantiate = Instantiate(prefab);
        var newPlayer = PlayerInput.Instantiate(prefab, newPI.playerIndex, "Player", -1, newPI.user.pairedDevices[0]);
        Stats newPlayerStats = newPlayer.GetComponent<Stats>();
        // prefabToInstantiate.GetComponent<PlayerInput>().user
        newStats.CopyStatsToOtherComponent(newPlayerStats);
    }
}
