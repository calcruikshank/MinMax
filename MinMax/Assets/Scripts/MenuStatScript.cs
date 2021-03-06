using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuStatScript : MonoBehaviour
{
    public string playerName;
    public Color playerColor;
    public Stats thisStats;
    public PlayerCursor thisPC;
    public TMP_Text[] statTexts;
    public GameObject[] statPanels;
    public GameObject[] stars;
    // public List<string> statStrings = new List<string>();
    public GameObject readyPanel, waitPanel, backgroundPanel;
    public bool isBot = true;
    public Toggle botToggle;
    public Button removeButton, addBotButton;

    public void ResetTexts()
    {
        foreach(TMP_Text t in statTexts)
        {
            t.text = "";
        }
        readyPanel.SetActive(true);
        waitPanel.SetActive(false);
        addBotButton.gameObject.SetActive(!backgroundPanel.activeSelf);
    }

    public IEnumerator RollStats()
    {
        DieRoller.singleton.botsToggle.interactable = false;
        DieRoller.singleton.sameStatsToggle.interactable = false;
        foreach(GameObject go in statPanels)
        {
            go.SetActive(false);
        }
        waitPanel.SetActive(true);
        readyPanel.SetActive(false);
        DieRoller.singleton.rollButton.interactable = !DieRoller.singleton.PlayersAreRollingStats();
        yield return new WaitForEndOfFrame();
        if (DieRoller.singleton.useSameStats)
        {
            for(int i = 0; i < DieRoller.singleton.sameStats.Count; i++)
            {
                GameObject iteratedPanel = statPanels[i];
                iteratedPanel.SetActive(true);
                iteratedPanel.GetComponentInChildren<TMP_Text>().text = DieRoller.singleton.sameStats[i];
            }
        }
        else
        {
            List<string> statsCopy = new List<string>(DieRoller.singleton.statStrings);
            for (int i = 0; i < 6; i++)
            {
                int randomStat = Mathf.FloorToInt(Random.Range(0, statsCopy.Count));
                GameObject randPanel = statPanels[i];
                randPanel.SetActive(true);
                randPanel.GetComponentInChildren<TMP_Text>().text = statsCopy[randomStat] + "......................................";
                statsCopy.RemoveAt(randomStat);
                yield return new WaitForSeconds(1);
            }
        }
        waitPanel.SetActive(false);
        readyPanel.SetActive(true);
        DieRoller.singleton.rollButton.interactable = !DieRoller.singleton.PlayersAreRollingStats();
        DieRoller.singleton.botsToggle.interactable = !DieRoller.singleton.PlayersAreRollingStats();
        DieRoller.singleton.sameStatsToggle.interactable = !DieRoller.singleton.PlayersAreRollingStats();
    }

    public void Toggle_IsBot(bool b)
    {
        // Debug.Log("setting is bot to " + b + " on " + gameObject.name);
        isBot = b;
        thisPC.playerImage.enabled = !b;
        thisPC.playerLabel.enabled = !b;
        foreach(GameObject g in stars)
        {
            g.SetActive(!b);
        }
    }

    public bool IsFull()
    {
        foreach(TMP_Text tt in statTexts)
        {
            if (string.IsNullOrEmpty(tt.text)) return false;
        }
        return true;
    }
}
