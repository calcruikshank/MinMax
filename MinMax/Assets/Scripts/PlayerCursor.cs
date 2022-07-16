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
    public TMP_Text[] statTexts;
    public float speed = 100.0f;
    public bool usedCurrentDie = false;
    public MenuStatScript thisMSS;

    public void Initialize(MenuStatScript newMenuStats)
    {
        thisMSS = newMenuStats;
        playerLabel.text = newMenuStats.playerName;
        playerImage.color = newMenuStats.playerColor;
        playerLabel.color = newMenuStats.playerColor;
        newMenuStats.thisPC = this;
    }

    public void OnMove(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }

    public void OnFire()
    {
        Debug.Log(gameObject.name + " fire");
        if (Vector3.Distance(transform.position, DieRoller.singleton.rollButton.transform.position) < 90)
        {
            DieRoller.singleton.rollButton.onClick.Invoke();
            return;
        }
        if (Vector3.Distance(transform.position, DieRoller.singleton.resetButton.transform.position) < 45)
        {
            DieRoller.singleton.resetButton.onClick.Invoke();
            return;
        }
        if (usedCurrentDie || DieRoller.singleton.currentDie is null || string.IsNullOrEmpty(DieRoller.singleton.valueText.text)) return;
        float closestDistance = 10.0f;
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
        }
    }

    void FixedUpdate()
    {
        thisRT.anchoredPosition += moveVal * speed * Time.deltaTime;
        thisRT.anchoredPosition = new Vector2(Mathf.Clamp(thisRT.anchoredPosition.x, (-Screen.width/2) + 40, (Screen.width/2) - 40), Mathf.Clamp(thisRT.anchoredPosition.y, (-Screen.height/2)+20, (Screen.height/2)-20));
    }
}
