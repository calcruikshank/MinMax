using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DieRoller : MonoBehaviour
{
    public static DieRoller singleton;
    public GameObject diePrefab;
    public Transform placeDiceHere;
    public DieScript currentDie;
    public TMP_Text valueText;
    public int playerIndex = 0;
    public Button rollButton;
    public GameObject[] playerCursors;
    public GameObject[] playerPanels;

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
        Debug.Log("rolling die");
        if (currentDie != null && !EveryoneHasUsedCurrentDie()) return;
        GameObject rolledDie = Instantiate(diePrefab, placeDiceHere.position, Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
        currentDie = rolledDie.GetComponent<DieScript>();
        currentDie.thisRB.AddForce(Vector3.right * Random.Range(100, 300));
        currentDie.thisDR = this;
        currentDie.stopped = false;
        currentDie.thisRB.isKinematic = false;
        foreach(GameObject go in playerCursors)
        {
            if (!go.activeSelf) continue;
            PlayerCursor pc = go.GetComponent<PlayerCursor>();
            pc.usedCurrentDie = false;
        }
    }

    public bool EveryoneHasUsedCurrentDie()
    {
        foreach(GameObject go in playerCursors)
        {
            if (!go.activeSelf) continue;
            PlayerCursor pc = go.GetComponent<PlayerCursor>();
            if (!pc.usedCurrentDie) return false;
        }
        return true;
    }

    public void CheckForDieUsage()
    {
        if (currentDie is null) return;
        // currentDie.thisMR.material = EveryoneHasUsedCurrentDie() ? usedDie : unusedDie;
    }

    public void OnPlayerJoined()
    {
        playerCursors[playerIndex].SetActive(true);
        playerPanels[playerIndex].SetActive(true);
        playerIndex++;
    }

    public void OnPlayerLeft()
    {
        playerIndex--;
        playerCursors[playerIndex].SetActive(false);
        playerPanels[playerIndex].SetActive(false);
    }

}
