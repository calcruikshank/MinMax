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
    public List<PlayerCursor> pcs = new List<PlayerCursor>();
    public GameObject joinText;
    public bool useBots = true, useSameStats = true;
    public Toggle botsToggle, sameStatsToggle;
    public List<string> sameStats = new List<string>();
    public List<string> statStrings = new List<string>();
    // public Button[] addBotButtons;
    public GameObject botPrefab;
    bool isRemovingPlayer = false;
    bool isAddingPlayer = false;

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

    void Start()
    {
        Button_RollSameStats();
        botsToggle.isOn = useBots;
        sameStatsToggle.isOn = useSameStats;
        Toggle_UseBots(true);
    }

    public void Button_RollDie()
    {
        // Debug.Log("rolling die");
        if (isRemovingPlayer || isAddingPlayer) return;
        if (PlayersAreReady()) return;
        if (currentDie != null && !EveryoneHasUsedCurrentDie()) return;
        currentDie = null;
        rollButton.interactable = false;
        GameObject rolledDie = Instantiate(diePrefab, placeDiceHere.position, Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
        // currentDie = rolledDie.GetComponent<DieScript>();
        rolledDie.GetComponent<DieScript>().thisRB.AddForce(Vector3.right * Random.Range(100, 300));
        rolledDie.GetComponent<DieScript>().thisDR = this;
        // currentDie.stopped = false;
        // currentDie.thisRB.isKinematic = false;
        foreach(PlayerCursor pc in pcs)
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

    public void Button_LoadGamesScene()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
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
        if (pcs.Count < 1) return false;
        foreach(PlayerCursor pc in pcs)
        {
            if (!pc.gameObject.activeSelf) continue;
            // PlayerCursor pc = go.GetComponent<PlayerCursor>();
            if (!pc.usedCurrentDie) return false;
        }
        return true;
    }

    public bool PlayersAreRollingStats()
    {
        if (pcs.Count < 1) return false;
        foreach(PlayerCursor pc in pcs)
        {
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
        if (isRemovingPlayer) return;
        isAddingPlayer = true;
        // playerCursors[playerIndex].SetActive(true);
        pi.transform.SetParent(placePlayersHere);
        PlayerCursor joinedPlayer = pi.GetComponent<PlayerCursor>();
        // FirstUnusedMenuStatScript().gameObject.SetActive(true);
        // addBotButtons[pcs.Count].gameObject.SetActive(false);
        joinedPlayer.Initialize(FirstUnusedMenuStatScript(), false);
        pcs.Add(joinedPlayer);
        // playerIndex++;
        joinText.SetActive(pcs.Count < 4);
        rollButton.interactable = !PlayersAreRollingStats();
        resetButton.interactable = pcs.Count > 0;
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
        pcs.Remove(leftPlayer);
        // leftPlayer.thisMSS.backgroundPanel.gameObject.SetActive(false);
        joinText.SetActive(pcs.Count < 4);
        rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
        resetButton.interactable = pcs.Count > 0;
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
        rollButton.interactable = true;

        foreach(PlayerCursor pc in pcs)
        {
            pc.speed = 200;
            pc.usedCurrentDie = false;
        }
    }

    public void RerollAllStats()
    {
        foreach(PlayerCursor pc in pcs)
        {
            StartCoroutine(pc.thisMSS.RollStats());
        }
    }
    
    public int time = 6;

    public IEnumerator Timer()
    {
        timerText.text = ":0" + time.ToString();

        while (time > 0)
        {
            yield return new WaitForSeconds(1.5f);
            time--;
            timerText.text = ":0" + time.ToString();

            foreach(PlayerCursor pc in pcs)
            {
                if (pc.thisMSS.isBot && !pc.usedCurrentDie)
                {
                    pc.ChooseRandomStat(currentDie.CurrentValue());
                }
            }
        }

        timerText.text = "";

        foreach(PlayerCursor pc in pcs)
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
        for(int i = pcs.Count - 1; i >= 0; i--)
        {
            if (pcs[i].thisMSS.isBot)
            {
                isRemovingPlayer = true;
                PlayerCursor leftPlayer = pcs[i];
                pcs[i].thisMSS.backgroundPanel.gameObject.SetActive(false);
                pcs[i].thisMSS.addBotButton.gameObject.SetActive(false);
                pcs.Remove(leftPlayer);
                joinText.SetActive(pcs.Count < 4);
                rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
                resetButton.interactable = pcs.Count > 0;
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
        if (!useBots || currentDie != null) return;
        isAddingPlayer = true;
        GameObject botCursor = Instantiate(botPrefab);
        botCursor.transform.SetParent(placePlayersHere);
        PlayerCursor joinedPlayer = botCursor.GetComponent<PlayerCursor>();
        MenuStatScript first = FirstUnusedMenuStatScript();
        int mssIndex = System.Array.IndexOf(playerPanels, first);
        // addBotButtons[mssIndex].gameObject.SetActive(false);
        joinedPlayer.Initialize(first, true);
        pcs.Add(joinedPlayer);
        joinText.SetActive(pcs.Count < 4);
        rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
        resetButton.interactable = pcs.Count > 0;
        ResetAllStats();
        Invoke("SetRemovingPlayerToFalse", 0.1f);
    }

    public bool PlayersAreReady()
    {
        if (pcs.Count < 1) return false;
        foreach(PlayerCursor pc in pcs)
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
        pcs.Remove(leftPlayer);
        int mssIndex = System.Array.IndexOf(playerPanels, leftPlayer.thisMSS);
        leftPlayer.thisMSS.backgroundPanel.gameObject.SetActive(false);
        leftPlayer.thisMSS.addBotButton.gameObject.SetActive(true);
        joinText.SetActive(pcs.Count < 4);
        rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
        resetButton.interactable = pcs.Count > 0;
        // addBotButtons[mssIndex].gameObject.SetActive(useBots);
        Destroy(leftPlayer.gameObject);
        Invoke("SetRemovingPlayerToFalse", 0.1f);
    }
}
