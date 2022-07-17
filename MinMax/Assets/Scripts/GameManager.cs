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
    public GameObject playerPrefab, jefferyPrefab, healthPanelPrefab, canvasPanel, powerUpPrefab;

    void Awake()
    {
        if(g != null){
            Destroy(this);
        }
        g = this;
    }

    void Start()
    {
        foreach(PlayerCursor pc in DieRoller.singleton.pcs)
        // for (int i = DieRoller.singleton.pcs.Count - 1; i >= 0; i--)
        {
            // PlayerCursor pc = DieRoller.singleton.pcs[i];
            pc.SetUnusedStatsToThree();
            
            GameObject hp = Instantiate(healthPanelPrefab, canvasPanel.transform);
            HealthPanelScript hps = hp.GetComponent<HealthPanelScript>();
            hps.playerName.text = pc.playerLabel.text;
            hps.sliderFill.color = new Color(pc.playerImage.color.r, pc.playerImage.color.g, pc.playerImage.color.b, 255.0f);

            if (pc.thisMSS.isBot)
            {
                GameObject jeff = NewJeffery(jefferyPrefab, pc.GetComponent<Stats>());
                jeff.GetComponent<PlayerController>().ChangeColor(pc.playerImage.color);
                jeff.GetComponent<PlayerController>().thisHPS = hps;
            }
            else
            {
                GameObject notJeff = NewPlayerInput(playerPrefab, pc.GetComponent<PlayerInput>(), pc.GetComponent<Stats>());
                notJeff.GetComponent<PlayerController>().ChangeColor(pc.playerImage.color);
                notJeff.GetComponent<PlayerController>().thisHPS = hps;
            }
        }
        InvokeRepeating("SpawnPower", 5f, 10f);
        Destroy(DieRoller.singleton.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnPower(){
        GameObject rolledDie = Instantiate(powerUpPrefab, new Vector3(Random.Range(-15, 15),Random.Range(0,30),Random.Range(-15, 15)), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        Powers.Add(rolledDie);
        rolledDie.GetComponent<Rigidbody>().AddForce(Vector3.right * Random.Range(100, 500));
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

    public float Remap (float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
}
