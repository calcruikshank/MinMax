using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager g;
    public SpawnManager sManager;
    public readonly List<GameObject> Players = new List<GameObject>();
    public readonly List<GameObject> Bullets = new List<GameObject>();
    public readonly List<GameObject> Powers = new List<GameObject>();
    public GameObject playerPrefab, jefferyPrefab, healthPanelPrefab, canvasPanel, powerUpPrefab;
    public bool startingGame;

    //camera refs
    public ZoomScript zoom;
    public FollowScript follow;
    public PointBetweenPlayers pbp;
    public CanvasGroup winnerCg;
    public TMP_Text winnerText;

    void Awake()
    {
        if(g != null){
            Destroy(this);
        }
        g = this;
    }

    void Start()
    {
        winnerCg.transform.localScale = Vector3.zero;
        winnerCg.alpha = 1f;
        winnerCg.transform.DOScale(Vector3.one*2.15f, 0.35f);
        winnerText.text = "Get Ready...";

        startingGame = true;
        DieRoller.singleton = null;
        startedRestart = false;
        if (SoundManager.singleton == null) return;
        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            GameObject hp = Instantiate(healthPanelPrefab, canvasPanel.transform);
            HealthPanelScript hps = hp.GetComponent<HealthPanelScript>();
            Color playerColor = new Color(pc.playerImage.color.r, pc.playerImage.color.g, pc.playerImage.color.b, 255.0f);
            hps.playerName.text = pc.playerLabel.text;
            hps.sliderFill.color = playerColor;
            hps.playerBackground.color = playerColor;
            hps.playerHealthName.color = playerColor;

            if (pc.GetComponent<PlayerInput>())
            {
                GameObject notJeff = NewPlayerInput(playerPrefab, pc.playerInput, pc.GetComponent<Stats>());
                notJeff.GetComponent<PlayerController>().ChangeColor(pc.playerImage.color);
                notJeff.GetComponent<PlayerController>().thisHPS = hps;
                if (pc.playerInput.currentControlScheme == "Keyboard&Mouse")
                {
                    pc.playerInput.enabled = false;
                    Cursor.visible = true;
                }
            }
            else
            {
                GameObject jeff = NewJeffery(jefferyPrefab, pc.GetComponent<Stats>());
                jeff.GetComponent<PlayerController>().ChangeColor(pc.playerImage.color);
                jeff.GetComponent<PlayerController>().thisHPS = hps;
            }
        }
        InvokeRepeating("SpawnPower", 5f, 10f);
        Invoke("SetStartingFalse", 3);
    }

    void SetStartingFalse()
    {
        if (startingGame)
        {
            startingGame = false;
            if (Players.Count > 1) 
            {
                winnerCg.transform.localScale = Vector3.zero;
                winnerCg.alpha = 1f;
                winnerCg.transform.DOScale(Vector3.one*2.15f, 0.35f);
                winnerText.text = "GO!";
                Invoke("SetStartingFalse", 1);
            }
            else
            {
                winnerText.text = "";
                winnerCg.alpha = 0;
            }
        }
        else
        {
            winnerText.text = "";
            winnerCg.alpha = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!startedRestart)
        {
            if(Players.Count == 1)
            {
                Debug.Log("Winner winner");
                follow.enabled = false;
                zoom.enabled = false;
                pbp.enabled = false;

                //show winner text here.
                winnerCg.transform.localScale = Vector3.zero;
                winnerCg.alpha = 1f;
                winnerCg.transform.DOScale(Vector3.one*2.15f, 0.35f);
                winnerText.text = Players[0].GetComponentInChildren<Jeffery>() ? (Random.Range(0,100) == 0 ? "JEFFERY WINS" : "BOTS WIN") : "YOU WIN";

               StartCoroutine(Restart());
            }
        }

    }
    bool startedRestart = false;
    IEnumerator Restart()
    {
        if (!startedRestart)
        {
            startedRestart = true;
            for (int i = SoundManager.singleton.pcs.Count - 1; i >= 0; i--)
            {
                Destroy(SoundManager.singleton.pcs[i].gameObject);
            }
            SoundManager.singleton.pcs.Clear();
            yield return new WaitForSeconds(7f);
            SceneManager.LoadScene("Main Menu");
        }
    }

    public void SpawnPower()
    {
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
        newPlayer.transform.position = sManager.RandomSpawn().position;
        return newPlayer.gameObject;
    }

    GameObject NewJeffery(GameObject prefab, Stats newStats)
    {
        GameObject newJeff = Instantiate(prefab, sManager.RandomSpawn().position, Quaternion.identity);
        Stats jeffStats = newJeff.GetComponent<Stats>();
        newStats.CopyStatsToOtherComponent(jeffStats);
        return newJeff;
    }


    
}
