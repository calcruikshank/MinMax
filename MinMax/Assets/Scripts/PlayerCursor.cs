using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerCursor : MonoBehaviour
{
    public TMP_Text playerLabel;
    public Image playerImage;
    public RectTransform thisRT;
    public Vector2 moveVal;
    public float speed = 100.0f;
    public bool usedCurrentDie = false;
    public MenuStatScript thisMSS;
    bool usedMovement = false, usedHealth = false, usedAttackSpeed = false, usedDamage = false, usedProjSpeed = false, usedProjSize = false, usedPlaSize = false, usedRange = false, usedReflect = false, usedMana = false, usedCrit = false;
    public PlayerInput playerInput;
    public bool isAnimating;
    public void Initialize(MenuStatScript newMenuStats, bool bot = false)
    {
        if (newMenuStats is null) 
        {
            Destroy(gameObject);
            return;
        }
        thisMSS = newMenuStats;
        RectTransform mssrt = thisMSS.backgroundPanel.GetComponent<RectTransform>();
        mssrt.anchoredPosition = new Vector2(mssrt.anchoredPosition.x , -230);
        thisMSS.backgroundPanel.gameObject.SetActive(true);
        thisMSS.addBotButton.gameObject.SetActive(false);
        isAnimating = true;
        mssrt.DOAnchorPosY(10, 0.2f).OnComplete(() => {
            isAnimating = false;
        });
        playerLabel.text = newMenuStats.playerName;
        playerImage.color = bot ? new Color(newMenuStats.playerColor.r,newMenuStats.playerColor.g,newMenuStats.playerColor.b,0) : newMenuStats.playerColor;
        playerLabel.color = bot ? new Color(newMenuStats.playerColor.r,newMenuStats.playerColor.g,newMenuStats.playerColor.b,0) : newMenuStats.playerColor;
        newMenuStats.thisPC = this;
        newMenuStats.thisStats = GetComponent<Stats>();
        transform.position = newMenuStats.transform.position;
        newMenuStats.ResetTexts();
        // newMenuStats.isBot = bot;
        newMenuStats.botToggle.isOn = bot;
        newMenuStats.botToggle.gameObject.SetActive(bot);
        StartCoroutine(newMenuStats.RollStats());
        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.zero;
        playerInput = this.GetComponent<PlayerInput>();
        Cursor.visible = true;
        foreach(GameObject g in newMenuStats.stars)
        {
            g.SetActive(!bot);
        }
    }

    public void OnMove(InputValue value)
    {
        if (DieRoller.singleton is null || isAnimating) return;
        if (thisMSS is null || !thisMSS.gameObject.activeSelf) return;
        if (thisMSS.waitPanel.activeSelf || thisMSS.isBot) return;
        
            moveVal = value.Get<Vector2>();
    }

    void OnConfirm()
    {
        if (DieRoller.singleton is null || isAnimating) return;
        OnFire();
    }

    public void OnFire()
    {
        if (DieRoller.singleton is null || isAnimating) return;
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

        if (Vector3.Distance(transform.position, DieRoller.singleton.botsToggle.transform.position) < 0.02f && DieRoller.singleton.botsToggle.interactable && DieRoller.singleton.xButton.gameObject.activeSelf)
        {
            DieRoller.singleton.botsToggle.isOn = !DieRoller.singleton.botsToggle.isOn;
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.sameStatsToggle.transform.position) < 0.02f && DieRoller.singleton.sameStatsToggle.interactable && DieRoller.singleton.xButton.gameObject.activeSelf)
        {
            DieRoller.singleton.sameStatsToggle.isOn = !DieRoller.singleton.sameStatsToggle.isOn;
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[0].addBotButton.transform.position) < 0.057f && DieRoller.singleton.playerPanels[0].addBotButton.interactable && DieRoller.singleton.playerPanels[0].addBotButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playerPanels[0].addBotButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[1].addBotButton.transform.position) < 0.057f && DieRoller.singleton.playerPanels[1].addBotButton.interactable && DieRoller.singleton.playerPanels[1].addBotButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playerPanels[1].addBotButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[2].addBotButton.transform.position) < 0.057f && DieRoller.singleton.playerPanels[2].addBotButton.interactable && DieRoller.singleton.playerPanels[2].addBotButton.gameObject.activeSelf)
        {
            DieRoller.singleton.playerPanels[2].addBotButton.onClick.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, DieRoller.singleton.playerPanels[3].addBotButton.transform.position) < 0.057f && DieRoller.singleton.playerPanels[3].addBotButton.interactable && DieRoller.singleton.playerPanels[3].addBotButton.gameObject.activeSelf)
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

        float closestDistance = 0.02f;
        TMP_Text closestText = null;
        foreach(TMP_Text tmp in thisMSS.statTexts)
        {
            float thisDistance = Vector3.Distance(transform.position, tmp.transform.position);
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
            StatComponent sc = closestText.GetComponent<StatComponent>();
            SoundManager.singleton.PlaySound(0);
            if (thisMSS.thisStats != null)
            {
                // Set selected stat to selected die roll
                if (sc.statName.text.Contains("Movement"))
                {
                    // Debug.Log("Setting speed of " + thisMSS.thisStats.gameObject.name + " to " + int.Parse(closestText.text));
                    thisMSS.thisStats.SetSpeed(int.Parse(closestText.text));
                    usedMovement = true;
                }
                else if (sc.statName.text.Contains("Health"))
                {
                    // Debug.Log("Setting health of " + thisMSS.thisStats.gameObject.name + " to " + int.Parse(closestText.text));
                    thisMSS.thisStats.SetHP(int.Parse(closestText.text));
                    usedHealth = true;
                }
                else if (sc.statName.text.Contains("Attack Speed"))
                {
                    // Debug.Log("Setting attack speed of " + thisMSS.thisStats.gameObject.name + " to " + int.Parse(closestText.text));
                    thisMSS.thisStats.SetAttackCooldown(int.Parse(closestText.text));
                    usedAttackSpeed = true;
                }
                else if (sc.statName.text.Contains("Damage"))
                {
                    // Debug.Log("Setting damage of " + thisMSS.thisStats.gameObject.name + " to " + int.Parse(closestText.text));
                    thisMSS.thisStats.SetAttackDamage(int.Parse(closestText.text));
                    usedDamage = true;
                }
                else if (sc.statName.text.Contains("Projectile Speed"))
                {
                    // Debug.Log("Setting projectile speed of " + thisMSS.thisStats.gameObject.name + " to " + int.Parse(closestText.text));
                    thisMSS.thisStats.SetProjectileSpeed(int.Parse(closestText.text));
                    usedProjSpeed = true;
                }
                else if (sc.statName.text.Contains("Projectile Size"))
                {
                    // Debug.Log("Setting projectile size of " + thisMSS.thisStats.gameObject.name + " to " + int.Parse(closestText.text));
                    thisMSS.thisStats.SetProjectileSize(int.Parse(closestText.text));
                    usedProjSize = true;
                }
                else if (sc.statName.text.Contains("Shrinkage"))
                {
                    // Debug.Log("Setting player size of " + thisMSS.thisStats.gameObject.name + " to " + int.Parse(closestText.text));
                    thisMSS.thisStats.SetPlayerSize(int.Parse(closestText.text));
                    usedPlaSize = true;
                }
                else if (sc.statName.text.Contains("Range"))
                {
                    // Debug.Log("Setting range of " + thisMSS.thisStats.gameObject.name + " to " + int.Parse(closestText.text));
                    thisMSS.thisStats.SetProjectileRange(int.Parse(closestText.text));
                    usedRange = true;
                }
                else if (sc.statName.text.Contains("Reflect Speed"))
                {
                    thisMSS.thisStats.SetDispelSpeed(int.Parse(closestText.text));
                    usedReflect = true;
                }
                else if (sc.statName.text.Contains("Collection"))
                {
                    thisMSS.thisStats.SetManaRegenRate(int.Parse(closestText.text));
                    usedMana = true;
                }
                else if (sc.statName.text.Contains("Crit Chance"))
                {
                    thisMSS.thisStats.SetCritChance(int.Parse(closestText.text));
                    usedCrit = true;
                }
            }

            usedCurrentDie = true;
            DieRoller.singleton.CheckForDieUsage();
            thisMSS.readyPanel.SetActive(true);
            DieRoller.singleton.Button_RollDie();
        }
    }

    float Remap (float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    void FixedUpdate()
    {

        // transform.position = new Vector3(Mathf.Clamp(transform.position.x + moveVal.x, -480, 480), 0, Mathf.Clamp(transform.position.z + moveVal.y, -360, 360));
        // transform.position += (new Vector3(moveVal.x, moveVal.y, 0) * speed * Time.deltaTime);
        // transform.position = new Vector3(Mathf.Clamp(transform.position.x, -470, 470), Mathf.Clamp(transform.position.y, -350, 350));

        if (playerInput != null && playerInput.enabled && playerInput.currentControlScheme == "Keyboard&Mouse" && thisMSS != null && !thisMSS.isBot)
        {
            Cursor.visible = false;
            Vector2 mousePos = Mouse.current.position.ReadValue();
            thisRT.anchoredPosition = new Vector2(Remap(mousePos.x, 0, Screen.width, -450, 450), Remap(mousePos.y, 0, Screen.height, -260, 260));
            // thisRT.anchoredPosition = new Vector2(Mathf.Clamp(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).x, -450, 450);
        }
        else
        {
            thisRT.anchoredPosition = new Vector2(Mathf.Clamp(thisRT.anchoredPosition.x + moveVal.x * speed, -450, 450), Mathf.Clamp(thisRT.anchoredPosition.y + moveVal.y * speed, -245, 245));
        }

        // thisRT.anchoredPosition = new Vector2(Mathf.Clamp(thisRT.anchoredPosition.x + moveVal.x * speed, -450, 450), Mathf.Clamp(thisRT.anchoredPosition.y + moveVal.y * speed, -245, 245));

        // thisRT.anchoredPosition += moveVal * speed * Time.deltaTime;
        // thisRT.anchoredPosition = new Vector3(Mathf.Clamp(thisRT.anchoredPosition.x, (-Screen.width/2) + 40, (Screen.width/2) - 40), Mathf.Clamp(thisRT.anchoredPosition.y, (-Screen.height/2) + 40, (Screen.height/2) - 40));
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
        int randomText = Mathf.FloorToInt(Random.Range(0, unusedStatTexts.Count));
        unusedStatTexts[randomText].text = rand.ToString();
        StatComponent sc = unusedStatTexts[randomText].GetComponent<StatComponent>();
        if (thisMSS.thisStats != null)
        {
            // Set selected stat to selected die roll
            if (sc.statName.text.Contains("Movement"))
            {
                // Debug.Log("Setting speed of " + thisMSS.thisStats.gameObject.name + " to " + rand);
                thisMSS.thisStats.SetSpeed(rand);
                usedMovement = true;
            }
            else if (sc.statName.text.Contains("Health"))
            {
                // Debug.Log("Setting health of " + thisMSS.thisStats.gameObject.name + " to " + rand);
                thisMSS.thisStats.SetHP(rand);
                usedHealth = true;
            }
            else if (sc.statName.text.Contains("Attack Speed"))
            {
                // Debug.Log("Setting attack speed of " + thisMSS.thisStats.gameObject.name + " to " + rand);
                thisMSS.thisStats.SetAttackCooldown(rand);
                usedAttackSpeed = true;
            }
            else if (sc.statName.text.Contains("Damage"))
            {
                // Debug.Log("Setting damage of " + thisMSS.thisStats.gameObject.name + " to " + rand);
                thisMSS.thisStats.SetAttackDamage(rand);
                usedDamage = true;
            }
            else if (sc.statName.text.Contains("Projectile Speed"))
            {
                // Debug.Log("Setting projectile speed of " + thisMSS.thisStats.gameObject.name + " to " + rand);
                thisMSS.thisStats.SetProjectileSpeed(rand);
                usedProjSpeed = true;
            }
            else if (sc.statName.text.Contains("Projectile Size"))
            {
                // Debug.Log("Setting projectile size of " + thisMSS.thisStats.gameObject.name + " to " + rand);
                thisMSS.thisStats.SetProjectileSize(rand);
                usedProjSize = true;
            }
            else if (sc.statName.text.Contains("Shrinkage"))
            {
                // Debug.Log("Setting player size of " + thisMSS.thisStats.gameObject.name + " to " + rand);
                thisMSS.thisStats.SetPlayerSize(rand);
                usedPlaSize = true;
            }
            else if (sc.statName.text.Contains("Range"))
            {
                // Debug.Log("Setting range of " + thisMSS.thisStats.gameObject.name + " to " + rand);
                thisMSS.thisStats.SetProjectileRange(rand);
                usedRange = true;
            }
            else if (sc.statName.text.Contains("Reflect Speed"))
            {
                thisMSS.thisStats.SetDispelSpeed(rand);
                usedReflect = true;
            }
            else if (sc.statName.text.Contains("Collection"))
            {
                thisMSS.thisStats.SetManaRegenRate(rand);
                usedMana = true;
            }
            else if (sc.statName.text.Contains("Crit Chance"))
            {
                thisMSS.thisStats.SetCritChance(rand);
                usedCrit = true;
            }
        }
        usedCurrentDie = true;
        DieRoller.singleton.CheckForDieUsage();
        thisMSS.readyPanel.SetActive(true);
    }

    public void SetUnusedStatsToThree()
    {
        foreach(string statName in DieRoller.singleton.statStrings)
        {
            if (statName.Contains("Movement") && !usedMovement)
            {
                // Debug.Log("setting movement of " + gameObject.name + " to 3");
                thisMSS.thisStats.SetSpeed(2);
            }
            if (statName.Contains("Health") && !usedHealth)
            {
                // Debug.Log("setting health of " + gameObject.name + " to 3");
                thisMSS.thisStats.SetHP(2);
            }
            if (statName.Contains("Attack Speed") && !usedAttackSpeed)
            {
                // Debug.Log("setting attack speed of " + gameObject.name + " to 3");
                thisMSS.thisStats.SetAttackCooldown(2);
            }
            if (statName.Contains("Damage") && !usedDamage)
            {
                // Debug.Log("setting damage of " + gameObject.name + " to 3");
                thisMSS.thisStats.SetAttackDamage(2);
            }
            if (statName.Contains("Projectile Speed") && !usedProjSpeed)
            {
                // Debug.Log("setting proj speed of " + gameObject.name + " to 3");
                thisMSS.thisStats.SetProjectileSpeed(2);
            }
            if (statName.Contains("Projectile Size") && !usedProjSize)
            {
                // Debug.Log("setting projectile size of " + gameObject.name + " to 3");
                thisMSS.thisStats.SetProjectileSize(2);
            }
            if (statName.Contains("Shrinkage") && !usedPlaSize)
            {
                // Debug.Log("setting size of " + gameObject.name + " to 3");
                thisMSS.thisStats.SetPlayerSize(2);
            }
            if (statName.Contains("Range") && !usedRange)
            {
                // Debug.Log("setting range of " + gameObject.name + " to 3");
                thisMSS.thisStats.SetProjectileRange(2);
            }
            if (statName.Contains("Reflect Speed") && !usedReflect)
            {
                thisMSS.thisStats.SetDispelSpeed(2);
            }
            if (statName.Contains("Collection") && !usedMana)
            {
                thisMSS.thisStats.SetManaRegenRate(2);
            }
            if (statName.Contains("Crit Chance") && !usedCrit)
            {
                thisMSS.thisStats.SetCritChance(2);
            }
        }
        usedMovement = false;
        usedHealth = false;
        usedAttackSpeed = false;
        usedDamage = false;
        usedProjSpeed = false;
        usedProjSize = false;
        usedPlaSize = false;
        usedRange = false;
        usedReflect = false;
        usedMana = false;
        usedCrit = false;
    }
}
