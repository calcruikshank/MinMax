using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager g;
    public readonly List<GameObject> Players = new List<GameObject>();
    public readonly List<GameObject> Bullets = new List<GameObject>();
    public GameObject playerPrefab, jefferyPrefab;

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
            pc.SetUnusedStatsToThree();
            if (pc.thisMSS.isBot)
            {
                GameObject jeff = NewJeffery(jefferyPrefab, pc.GetComponent<Stats>());
                jeff.GetComponent<PlayerController>().ChangeColor(pc.playerImage.color);
            }
            else
            {
                GameObject notJeff = NewPlayerInput(playerPrefab, pc.GetComponent<PlayerInput>(), pc.GetComponent<Stats>());
                notJeff.GetComponent<PlayerController>().ChangeColor(pc.playerImage.color);
            }
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

    GameObject NewPlayerInput(GameObject prefab, PlayerInput newPI, Stats newStats)
    {
        InputDevice[] playerDevices = new InputDevice[newPI.user.pairedDevices.Count];
        for(int i = 0; i < newPI.user.pairedDevices.Count; i++)
        {
            playerDevices[i] = newPI.user.pairedDevices[i];
        }
        var newPlayer = PlayerInput.Instantiate(prefab, newPI.playerIndex, "PlayerInput", -1, playerDevices);
        Stats newPlayerStats = newPlayer.GetComponent<Stats>();
        newStats.CopyStatsToOtherComponent(newPlayerStats);
        newPlayer.transform.position = SpawnManager.singleton.RandomSpawn().position;
        return newPlayer.gameObject;
    }

    GameObject NewJeffery(GameObject prefab, Stats newStats)
    {
        GameObject newJeff = Instantiate(prefab, SpawnManager.singleton.RandomSpawn().position, Quaternion.identity);
        Stats jeffStats = newJeff.GetComponent<Stats>();
        newStats.CopyStatsToOtherComponent(jeffStats);
        return newJeff;
    }
}
