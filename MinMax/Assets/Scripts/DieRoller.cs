using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

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
    public Button[] addBotButtons;
    public GameObject botPrefab;

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
        Debug.Log("rolling die");
        if (currentDie != null && !EveryoneHasUsedCurrentDie()) return;
        rollButton.interactable = false;
        GameObject rolledDie = Instantiate(diePrefab, placeDiceHere.position, Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
        currentDie = rolledDie.GetComponent<DieScript>();
        currentDie.thisRB.AddForce(Vector3.right * Random.Range(100, 300));
        currentDie.thisDR = this;
        // currentDie.stopped = false;
        // currentDie.thisRB.isKinematic = false;
        foreach(PlayerCursor pc in pcs)
        {
            if (!pc.gameObject.activeSelf) continue;
            // PlayerCursor pc = go.GetComponent<PlayerCursor>();
            pc.usedCurrentDie = false;
            pc.speed = 0;
            pc.thisMSS.readyPanel.SetActive(false);
        }
        // StartCoroutine(Timer());
        time = 6;
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
        // playerCursors[playerIndex].SetActive(true);
        pi.transform.SetParent(placePlayersHere);
        PlayerCursor joinedPlayer = pi.GetComponent<PlayerCursor>();
        playerPanels[pcs.Count].gameObject.SetActive(true);
        addBotButtons[pcs.Count].gameObject.SetActive(false);
        joinedPlayer.Initialize(playerPanels[pcs.Count]);
        pcs.Add(joinedPlayer);
        // playerIndex++;
        joinText.SetActive(pcs.Count < 4);
        rollButton.interactable = !PlayersAreRollingStats();
        resetButton.interactable = pcs.Count > 0;
        ResetAllStats();
    }

    public void OnPlayerLeft(PlayerInput pi)
    {
        // playerIndex--;
        // playerCursors[playerIndex].SetActive(false);
        PlayerCursor leftPlayer = pi.GetComponent<PlayerCursor>();
        pcs.Remove(leftPlayer);
        playerPanels[pcs.Count].gameObject.SetActive(false);
        joinText.SetActive(pcs.Count < 4);
        rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
        resetButton.interactable = pcs.Count > 0;
        addBotButtons[pcs.Count].gameObject.SetActive(useBots);
    }

    public void SetPlayersMovement()
    {
        foreach(PlayerCursor pc in pcs)
        {
            pc.speed = 200;
        }
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
            yield return new WaitForSeconds(1);
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
        rollButton.interactable = !PlayersAreReady();
    }

    public void Toggle_SameStats(bool s)
    {
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
                PlayerCursor leftPlayer = pcs[i];
                pcs.Remove(leftPlayer);
                playerPanels[pcs.Count].gameObject.SetActive(false);
                joinText.SetActive(pcs.Count < 4);
                rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
                resetButton.interactable = pcs.Count > 0;
                addBotButtons[pcs.Count].gameObject.SetActive(useBots);
                Destroy(leftPlayer.gameObject);
            }
        }
        for (int i = 0; i < addBotButtons.Length; i++)
        {
            // int buttonIndex = System.Array.IndexOf(AddBotButtons, bu);
            bool isPlayer = i < pcs.Count;
            addBotButtons[i].gameObject.SetActive(b && !isPlayer);
        }
    }

    public void AddBot()
    {
        if (!useBots || currentDie != null) return;
        GameObject botCursor = Instantiate(botPrefab);
        botCursor.transform.SetParent(placePlayersHere);
        PlayerCursor joinedPlayer = botCursor.GetComponent<PlayerCursor>();
        playerPanels[pcs.Count].gameObject.SetActive(true);
        addBotButtons[pcs.Count].gameObject.SetActive(false);
        joinedPlayer.Initialize(playerPanels[pcs.Count], true);
        pcs.Add(joinedPlayer);
        joinText.SetActive(pcs.Count < 4);
        rollButton.interactable = !PlayersAreRollingStats() && !PlayersAreReady();
        resetButton.interactable = pcs.Count > 0;
        ResetAllStats();
    }

    public bool PlayersAreReady()
    {
        foreach(PlayerCursor pc in pcs)
        {
            if (!pc.thisMSS.IsFull()) return false;
        }
        playButton.gameObject.SetActive(true);
        return true;
    }
}
