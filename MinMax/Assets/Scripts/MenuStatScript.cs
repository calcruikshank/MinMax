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

    public void ResetTexts()
    {
        foreach(TMP_Text t in statTexts)
        {
            t.text = "";
        }
    }
}
