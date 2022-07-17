using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager g;
    public readonly List<GameObject> Players = new List<GameObject>();
    public readonly List<GameObject> Bullets = new List<GameObject>();
    public readonly List<GameObject> Powers = new List<GameObject>();
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
                NewJeffery(jefferyPrefab, pc.GetComponent<Stats>());
            }
            else
            {
                NewPlayerInput(playerPrefab, pc.GetComponent<PlayerInput>(), pc.GetComponent<Stats>());
            }
        }
        InvokeRepeating("SpawnPower", 10f, 10f);
        Destroy(DieRoller.singleton.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnPower(){
        
    }
    public void AddPlayer(PlayerController player){
            Players.Add(player.gameObject);
    }

    public void RemovePlayer(PlayerController player){
        Players.Remove(player.gameObject);
    }

    public void RemovePower(PowerUp power){
        Powers.Remove(power.gameObject);
    }

    public void AddBullet(Bullet bullet){
        Bullets.Add(bullet.gameObject);
    }

    public void RemoveBullet(Bullet bullet){
        Bullets.Remove(bullet.gameObject);
    }

    public void NewPlayerInput(GameObject prefab, PlayerInput newPI, Stats newStats)
    {
        // Debug.Log(newPI.devices);
        InputDevice[] playerDevices = new InputDevice[newPI.user.pairedDevices.Count];
        // foreach(var v in newPI.devices)
        // {
        //     Debug.Log("device " + v);
        // }
        // foreach(var v in newPI.user.pairedDevices)
        for(int i = 0; i < newPI.user.pairedDevices.Count; i++)
        {
            // Debug.Log("paired device " + newPI.user.pairedDevices[i]);
            playerDevices[i] = newPI.user.pairedDevices[i];
        }
        // GameObject prefab, int playerIndex = -1, string controlScheme = null, int splitScreenIndex = -1, params InputDevice[] pairWithDevices
        var newPlayer = PlayerInput.Instantiate(prefab, newPI.playerIndex, "PlayerInput", -1, playerDevices);
        Stats newPlayerStats = newPlayer.GetComponent<Stats>();
        newStats.CopyStatsToOtherComponent(newPlayerStats);
    }

    public void NewJeffery(GameObject prefab, Stats newStats)
    {
        GameObject newJeff = Instantiate(prefab);
        Stats jeffStats = newJeff.GetComponent<Stats>();
        newStats.CopyStatsToOtherComponent(jeffStats);
    }
}
