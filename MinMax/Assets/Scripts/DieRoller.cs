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
    public TMP_Text valueText;
    // public int playerIndex = 0;
    public Button rollButton, resetButton;
    // public GameObject[] playerCursors;
    public MenuStatScript[] playerPanels;
    public List<PlayerCursor> pcs = new List<PlayerCursor>();
    public GameObject joinText;

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

    public void Button_RollDie()
    {
        // Debug.Log("rolling die");
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

    public void CheckForDieUsage()
    {
        if (currentDie is null || !EveryoneHasUsedCurrentDie()) return;
        
        Destroy(currentDie.gameObject);
        currentDie = null;
        valueText.text = "";
        rollButton.interactable = true;

        // currentDie.thisMR.material = EveryoneHasUsedCurrentDie() ? usedDie : unusedDie;
    }

    public void OnPlayerJoined(PlayerInput pi)
    {
        // playerCursors[playerIndex].SetActive(true);
        pi.transform.SetParent(placePlayersHere);
        PlayerCursor joinedPlayer = pi.GetComponent<PlayerCursor>();
        playerPanels[pcs.Count].gameObject.SetActive(true);
        joinedPlayer.Initialize(playerPanels[pcs.Count]);
        pcs.Add(joinedPlayer);
        // playerIndex++;
        joinText.SetActive(pcs.Count < 4);
        rollButton.interactable = pcs.Count > 0;
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
        rollButton.interactable = pcs.Count > 0;
        resetButton.interactable = pcs.Count > 0;
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
            mss.ResetTexts();
        }
        
        if (currentDie != null)
        {
            Destroy(currentDie.gameObject);
            currentDie = null;
        }
        valueText.text = "";
        rollButton.interactable = true;

        foreach(PlayerCursor pc in pcs)
        {
            pc.speed = 200;
            pc.usedCurrentDie = false;
        }
    }
}
