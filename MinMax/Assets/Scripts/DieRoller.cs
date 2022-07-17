using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class DieRoller : MonoBehaviour
{
    public static DieRoller singleton;
    public GameObject diePrefab;
    public Transform placeDiceHere, placePlayersHere;
    public DieScript currentDie;
    public TMP_Text valueText, timerText;
    // public int playerIndex = 0;
    public Button rollButton, resetButton, optionsButton, xButton, playButton;
    // public GameObject[] playerCursors;
    public MenuStatScript[] playerPanels;
    // public List<PlayerCursor> pcs = new List<PlayerCursor>();
    public GameObject joinText;
    public bool useBots = true, useSameStats = true;
    public Toggle botsToggle, sameStatsToggle;
    public List<string> sameStats = new List<string>();
    public List<string> statStrings = new List<string>();
    // public Button[] addBotButtons;
    public GameObject botPrefab;
    bool isRemovingPlayer = false;
    bool isAddingPlayer = false;
    public float dieSpawnSize = 0.75f;
    // public GameObject dragCanvas;

    void Awake()
    {
        if (singleton is null)
        {
            Debug.Log("setting die roller singleton");
            singleton = this;
            // DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        Button_RollSameStats();
        Toggle_UseBots(true);
        botsToggle.isOn = useBots;
        sameStatsToggle.isOn = useSameStats;
        // foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        for(int i = SoundManager.singleton.pcs.Count - 1; i >= 0; i--)
        {
            // GameObject newPC = Instantiate(pc.gameObject);
            // newPC.transform.SetParent(SoundManager.singleton.transform);
            // pc.transform.SetParent(placePlayersHere.transform);
            // SoundManager.singleton.pcs.Add(pc);
            PlayerCursor pc = SoundManager.singleton.pcs[i];
            if (pc.GetComponent<PlayerInput>()) {
                OnPlayerJoined(pc.GetComponent<PlayerInput>());
            }
            else {
                SoundManager.singleton.pcs.Remove(pc);
                Destroy(pc.gameObject);
            }
        }
    }

    public void Button_RollDie()
    {
        // Debug.Log("rolling die");
        if (isRemovingPlayer || isAddingPlayer) return;
        if (PlayersAreReady()) return;
        if (currentDie != null && !EveryoneHasUsedCurrentDie()) return;
        SoundManager.singleton.PlaySound(3, 0.5f, 0.5f);
        currentDie = null;
        rollButton.interactable = false;
        GameObject rolledDie = Instantiate(diePrefab, placeDiceHere.position, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        // currentDie = rolledDie.GetComponent<DieScript>();
        rolledDie.transform.localScale = new Vector3(dieSpawnSize, dieSpawnSize, dieSpawnSize);
        rolledDie.GetComponent<DieScript>().thisRB.AddForce(Vector3.right * Random.Range(200, 1000));
        rolledDie.GetComponent<DieScript>().thisRB.AddTorque(Vector3.right * Random.Range(-200000, 200000));
        rolledDie.GetComponent<DieScript>().thisDR = this;
        // currentDie.stopped = false;
        // currentDie.thisRB.isKinematic = false;
        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            if (!pc.gameObject.activeSelf) continue;
            // PlayerCursor pc = go.GetComponent<PlayerCursor>();
            pc.usedCurrentDie = false;
            // pc.speed = 0;
            pc.thisMSS.readyPanel.SetActive(false);
            pc.thisMSS.addBotButton.gameObject.SetActive(false);
        }
        // StartCoroutine(Timer());
        time = 6;
    }

    public void TogglePlayerJoining(bool on)
    {
        if (on)
        {
            GetComponent<PlayerInputManager>().EnableJoining();
            // GetComponent<PlayerInputManager>().joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        }
        else
        {
            GetComponent<PlayerInputManager>().DisableJoining();
            // GetComponent<PlayerInputManager>().joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        }
    }

    bool isStartingGame = false;

    public void Button_LoadGamesScene()
    {
        isStartingGame = true;
        GetComponent<PlayerInputManager>().DisableJoining();

        // SoundManager.singleton.pcs = new List<PlayerCursor>();

        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            // GameObject newPC = Instantiate(pc.gameObject);
            // newPC.transform.SetParent(SoundManager.singleton.transform);
            pc.transform.SetParent(SoundManager.singleton.transform);
            if (pc.gameObject.GetComponent<PlayerInput>())
            {
                OnPlayerLeft(pc.gameObject.GetComponent<PlayerInput>());
            }
            // SoundManager.singleton.pcs.Add(pc);
        }

        // GetComponent<PlayerInputManager>().joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

        SceneManager.LoadScene("SampleScene");
    }

    public void Button_RollSameStats()
    {
        List<string> statsCopy = new List<string>(statStrings);
        for(int i = 0; i < 6; i++)
        {
            int randStat = Mathf.FloorToInt(Random.Range(0, statsCopy.Count));
            string newStat = statsCopy[randStat];
            sameStats.Add(newStat);
            statsCopy.RemoveAt(randStat);
        }
    }

    public bool EveryoneHasUsedCurrentDie()
    {
        if (SoundManager.singleton.pcs.Count < 1) return false;
        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            if (!pc.gameObject.activeSelf) continue;
            // PlayerCursor pc = go.GetComponent<PlayerCursor>();
            if (!pc.usedCurrentDie) return false;
        }
        return true;
    }

    public bool PlayersAreRollingStats()
    {
        if (SoundManager.singleton.pcs.Count < 1) return false;
        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            if (pc is null) continue;
            if (!pc.gameObject.activeSelf) continue;
            if (pc.thisMSS.waitPanel.activeSelf) return true;
        }
        return false;
    }

    public void CheckForDieUsage()
    {
        if (currentDie is null || !EveryoneHasUsedCurrentDie()) return;
        
        Destroy(currentDie.gameObject);
        currentDie = null;
        valueText.text = "-";
        rollButton.interactable = !PlayersAreReady();

        // currentDie.thisMR.material = EveryoneHasUsedCurrentDie() ? usedDie : unusedDie;
    }

    public void OnPlayerJoined(PlayerInput pi)
    {
        if (isRemovingPlayer || isStartingGame) return;
        isAddingPlayer = true;
        // playerCursors[playerIndex].SetActive(true);
        pi.transform.SetParent(placePlayersHere);
        PlayerCursor joinedPlayer = pi.GetComponent<PlayerCursor>();
        // FirstUnusedMenuStatScript().gameObject.SetActive(true);
        // addBotButtons[pcs.Count].gameObject.SetActive(false);
        joinedPlayer.Initialize(FirstUnusedMenuStatScript(), false);
        SoundManager.singleton.pcs.Add(joinedPlayer);
        // playerIndex++;
        joinText.SetActive(SoundManager.singleton.pcs.Count < 4);
        rollButton.interactable = !PlayersAreRollingStats();
        resetButton.interactable = SoundManager.singleton.pcs.Count > 0;
        ResetAllStats();
        Invoke("SetRemovingPlayerToFalse", 0.1f);
    }

    public void OnPlayerLeft(PlayerInput pi)
    {
        if (isAddingPlayer) return;
        isRemovingPlayer = true;
        // playerIndex--;
        // playerCursors[playerIndex].SetActive(false);
        PlayerCursor leftPlayer = pi.GetComponent<PlayerCursor>();
        // SoundManager.singleton.pcs.Remove(leftPlayer);
        leftPlayer.thisMSS.backgroundPanel.gameObject.SetActive(false);
        joinText.SetActive(SoundManager.singleton.pcs.Count < 4);
        rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
        resetButton.interactable = SoundManager.singleton.pcs.Count > 0;
        // addBotButtons[pcs.Count].gameObject.SetActive(useBots);
        Invoke("SetRemovingPlayerToFalse", 0.1f);
    }

    void SetRemovingPlayerToFalse()
    {
        isRemovingPlayer = false;
        isAddingPlayer = false;
    }

    // public void SetPlayersMovement()
    // {
    //     foreach(PlayerCursor pc in pcs)
    //     {
    //         pc.speed = 200;
    //     }
    // }

    public MenuStatScript FirstUnusedMenuStatScript()
    {
        foreach(MenuStatScript mss in playerPanels)
        {
            if (mss.thisPC is null)
            {
                return mss;
            }
        }
        return null;
    }

    public void ResetAllStats()
    {
        foreach(MenuStatScript mss in playerPanels)
        {
            if (!mss.gameObject.activeSelf) return;
            mss.ResetTexts();
        }
        
        if (currentDie != null)
        {
            Destroy(currentDie.gameObject);
            currentDie = null;
        }
        valueText.text = "-";
        timerText.text = "";
        rollButton.interactable = true;

        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            pc.speed = 200;
            pc.usedCurrentDie = false;
        }

        StopCoroutine(Timer());

        if (currentDie != null)
        {
            Destroy(currentDie.gameObject);
        }

        currentDie = null;
    }

    public void RerollAllStats()
    {
        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            StartCoroutine(pc.thisMSS.RollStats());
        }
    }
    
    public int time = 6;

    public IEnumerator Timer()
    {
        timerText.text = ":0" + time.ToString();
        SoundManager.singleton.PlayRandomDieSound();

        while (time > 0)
        {
            yield return new WaitForSeconds(1.35f);
            time--;
            timerText.text = ":0" + time.ToString();

            foreach(PlayerCursor pc in SoundManager.singleton.pcs)
            {
                if (pc.thisMSS.isBot && !pc.usedCurrentDie)
                {
                    pc.ChooseRandomStat(currentDie.CurrentValue());
                }
            }
        }

        SoundManager.singleton.PlaySound(4, 0.3f, 0.2f);

        timerText.text = "";

        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            if (!pc.usedCurrentDie)
            {
                pc.ChooseRandomStat(currentDie.CurrentValue());
            }
            pc.usedCurrentDie = false;
        }

        if (currentDie != null)
        {
            Destroy(currentDie.gameObject);
        }

        currentDie = null;
        valueText.text = "-";
        // rollButton.interactable = !PlayersAreReady();
        Button_RollDie();
    }

    public void Toggle_SameStats(bool s)
    {
        if (PlayersAreRollingStats()) return;
        useSameStats = s;
    }

    public void Toggle_UseBots(bool b)
    {
        useBots = b;
        // b = !PlayersAreReady();
        // foreach(Button bu in addBotButtons)
        for(int i = SoundManager.singleton.pcs.Count - 1; i >= 0; i--)
        {
            if (SoundManager.singleton.pcs[i].thisMSS.isBot)
            {
                isRemovingPlayer = true;
                PlayerCursor leftPlayer = SoundManager.singleton.pcs[i];
                SoundManager.singleton.pcs[i].thisMSS.backgroundPanel.gameObject.SetActive(false);
                SoundManager.singleton.pcs[i].thisMSS.addBotButton.gameObject.SetActive(false);
                SoundManager.singleton.pcs.Remove(leftPlayer);
                joinText.SetActive(SoundManager.singleton.pcs.Count < 4);
                rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
                resetButton.interactable = SoundManager.singleton.pcs.Count > 0;
                // addBotButtons[pcs.Count].gameObject.SetActive(useBots);
                Destroy(leftPlayer.gameObject);
                Invoke("SetRemovingPlayerToFalse", 0.1f);
            }
        }
        // for (int i = 0; i < addBotButtons.Length; i++)
        // {
        //     // int buttonIndex = System.Array.IndexOf(AddBotButtons, bu);
        //     bool isPlayer = i < pcs.Count;
        //     // addBotButtons[i].gameObject.SetActive(b && !isPlayer);
        // }
    }

    public void AddBot()
    {
        if (!useBots || currentDie != null || isStartingGame) return;
        isAddingPlayer = true;
        GameObject botCursor = Instantiate(botPrefab);
        botCursor.transform.SetParent(placePlayersHere);
        PlayerCursor joinedPlayer = botCursor.GetComponent<PlayerCursor>();
        MenuStatScript first = FirstUnusedMenuStatScript();
        int mssIndex = System.Array.IndexOf(playerPanels, first);
        // addBotButtons[mssIndex].gameObject.SetActive(false);
        joinedPlayer.Initialize(first, true);
        SoundManager.singleton.pcs.Add(joinedPlayer);
        joinText.SetActive(SoundManager.singleton.pcs.Count < 4);
        rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
        resetButton.interactable = SoundManager.singleton.pcs.Count > 0;
        ResetAllStats();
        Invoke("SetRemovingPlayerToFalse", 0.1f);
    }
    public void ResetAllThings(){
        foreach(var mss in playerPanels){
            RemovePlayer(mss);
        }
        playButton.gameObject.SetActive(false);
        ResetAllStats();
    }
    public bool PlayersAreReady()
    {
        if (SoundManager.singleton.pcs.Count < 1) return false;
        foreach(PlayerCursor pc in SoundManager.singleton.pcs)
        {
            if (!pc.thisMSS.IsFull()) return false;
        }
        playButton.gameObject.SetActive(true);
        return true;
    }

    public void RemovePlayer(MenuStatScript mss)
    {
        if (isAddingPlayer) return;
        isRemovingPlayer = true;
        PlayerCursor leftPlayer = mss.thisPC;
        SoundManager.singleton.pcs.Remove(leftPlayer);
        int mssIndex = System.Array.IndexOf(playerPanels, leftPlayer.thisMSS);
        leftPlayer.thisMSS.backgroundPanel.gameObject.SetActive(false);
        leftPlayer.thisMSS.addBotButton.gameObject.SetActive(true);
        joinText.SetActive(SoundManager.singleton.pcs.Count < 4);
        rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
        resetButton.interactable = SoundManager.singleton.pcs.Count > 0;
        // addBotButtons[mssIndex].gameObject.SetActive(useBots);
        Destroy(leftPlayer.gameObject);
        Invoke("SetRemovingPlayerToFalse", 0.1f);
    }
}
