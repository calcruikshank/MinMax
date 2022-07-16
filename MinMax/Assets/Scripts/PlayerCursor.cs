using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerCursor : MonoBehaviour
{
    public TMP_Text playerLabel;
    public Image playerImage;
    public RectTransform thisRT;
    public Vector2 moveVal;
    // public TMP_Text[] statTexts;
    public float speed = 100.0f;
    public bool usedCurrentDie = false;
    public MenuStatScript thisMSS;

    public void Initialize(MenuStatScript newMenuStats, bool bot = false)
    {
        thisMSS = newMenuStats;
        thisMSS.backgroundPanel.gameObject.SetActive(true);
        thisMSS.addBotButton.gameObject.SetActive(false);
        playerLabel.text = newMenuStats.playerName;
        playerImage.color = newMenuStats.playerColor;
        playerLabel.color = newMenuStats.playerColor;
        newMenuStats.thisPC = this;
        transform.position = newMenuStats.transform.position;
        newMenuStats.ResetTexts();
        // newMenuStats.isBot = bot;
        newMenuStats.botToggle.isOn = bot;
        StartCoroutine(newMenuStats.RollStats());
        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.zero;
    }

    public void OnMove(InputValue value)
    {
        if (thisMSS is null || !thisMSS.gameObject.activeSelf) return;
        if (thisMSS.waitPanel.activeSelf || thisMSS.isBot) return;
        moveVal = value.Get<Vector2>();
    }

    public void OnFire()
    {
        if (Vector3.Distance(transform.position, DieRoller.singleton.playButton.transform.position) < 0.12f && DieRoller.singleton.playButton.interactable && !DieRoller.singleton.xButton.gameObject.activeSelf && DieRoller.singleton.playButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.rollButton.transform.position) < 0.1f && DieRoller.singleton.rollButton.interactable && !DieRoller.singleton.xButton.gameObject.activeSelf)
        {
            DieRoller.singleton.rollButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.resetButton.transform.position) < 0.06f && DieRoller.singleton.resetButton.interactable && DieRoller.singleton.xButton.gameObject.activeSelf)
        {
            DieRoller.singleton.resetButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.optionsButton.transform.position) < 0.07f && DieRoller.singleton.optionsButton.interactable && !DieRoller.singleton.xButton.gameObject.activeSelf)
        {
            DieRoller.singleton.optionsButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.xButton.transform.position) < 0.12f && DieRoller.singleton.xButton.interactable && DieRoller.singleton.xButton.gameObject.activeSelf)
        {
            DieRoller.singleton.xButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.botsToggle.transform.position) < 0.075f && DieRoller.singleton.botsToggle.interactable && DieRoller.singleton.xButton.gameObject.activeSelf)
        {
            DieRoller.singleton.botsToggle.isOn = !DieRoller.singleton.botsToggle.isOn;
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.sameStatsToggle.transform.position) < 0.075f && DieRoller.singleton.sameStatsToggle.interactable && DieRoller.singleton.xButton.gameObject.activeSelf)
        {
            DieRoller.singleton.sameStatsToggle.isOn = !DieRoller.singleton.sameStatsToggle.isOn;
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[0].addBotButton.transform.position) < 0.07f && DieRoller.singleton.playerPanels[0].addBotButton.interactable && DieRoller.singleton.playerPanels[0].addBotButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playerPanels[0].addBotButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[1].addBotButton.transform.position) < 0.07f && DieRoller.singleton.playerPanels[1].addBotButton.interactable && DieRoller.singleton.playerPanels[1].addBotButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playerPanels[1].addBotButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[2].addBotButton.transform.position) < 0.07f && DieRoller.singleton.playerPanels[2].addBotButton.interactable && DieRoller.singleton.playerPanels[2].addBotButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playerPanels[2].addBotButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[3].addBotButton.transform.position) < 0.07f && DieRoller.singleton.playerPanels[3].addBotButton.interactable && DieRoller.singleton.playerPanels[3].addBotButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playerPanels[3].addBotButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[0].removeButton.transform.position) < 0.007f && DieRoller.singleton.playerPanels[0].removeButton.interactable && DieRoller.singleton.playerPanels[0].removeButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playerPanels[0].removeButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[1].removeButton.transform.position) < 0.007f && DieRoller.singleton.playerPanels[1].removeButton.interactable && DieRoller.singleton.playerPanels[1].removeButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playerPanels[1].removeButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[2].removeButton.transform.position) < 0.007f && DieRoller.singleton.playerPanels[2].removeButton.interactable && DieRoller.singleton.playerPanels[2].removeButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playerPanels[2].removeButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[3].removeButton.transform.position) < 0.007f && DieRoller.singleton.playerPanels[3].removeButton.interactable && DieRoller.singleton.playerPanels[3].removeButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playerPanels[3].removeButton.onClick.Invoke();
            return;
        }

        if (thisMSS is null || !thisMSS.gameObject.activeSelf) return;

        if (Vector3.Distance(transform.position, thisMSS.removeButton.transform.position) < 0.014f && !DieRoller.singleton.xButton.gameObject.activeSelf)
        {
            thisMSS.removeButton.onClick.Invoke();
            return;
        }

        if (usedCurrentDie || DieRoller.singleton.currentDie is null || string.IsNullOrEmpty(DieRoller.singleton.valueText.text)) return;

        float closestDistance = 0.014f;
        TMP_Text closestText = null;
        foreach(TMP_Text tmp in thisMSS.statTexts)
        {
            float thisDistance = Vector3.Distance(transform.position, tmp.transform.position);
            // Debug.Log(thisDistance);
            if (thisDistance < closestDistance)
            {
                closestDistance = thisDistance;
                closestText = tmp;
            }
        }
        if (closestText != null)
        {
            if (!string.IsNullOrEmpty(closestText.text)) return;
            closestText.text = DieRoller.singleton.valueText.text;
            usedCurrentDie = true;
            DieRoller.singleton.CheckForDieUsage();
            thisMSS.readyPanel.SetActive(true);
            DieRoller.singleton.Button_RollDie();
        }
    }

    void FixedUpdate()
    {
        thisRT.anchoredPosition += moveVal * speed * Time.deltaTime;
        thisRT.anchoredPosition = new Vector3(Mathf.Clamp(thisRT.anchoredPosition.x, (-Screen.width/2) + 40, (Screen.width/2) - 40), Mathf.Clamp(thisRT.anchoredPosition.y, (-Screen.height/2)+20, (Screen.height/2)-20));
    }

    void OnDisable()
    {
        if (thisMSS is null) return;
        if (thisMSS.thisPC is null) return;
        thisMSS.thisPC = null;
    }

    public void ChooseRandomStat(int rand)
    {
        List<TMP_Text> unusedStatTexts = new List<TMP_Text>();
        foreach(TMP_Text tt in thisMSS.statTexts)
        {
            if (string.IsNullOrEmpty(tt.text))
            {
                unusedStatTexts.Add(tt);
            }
        }
        Debug.Log(unusedStatTexts.Count);
        int randomText = Mathf.FloorToInt(Random.Range(0, unusedStatTexts.Count));
        Debug.Log(randomText);
        unusedStatTexts[randomText].text = rand.ToString();
        usedCurrentDie = true;
        DieRoller.singleton.CheckForDieUsage();
        thisMSS.readyPanel.SetActive(true);
    }
}
