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

    public void OnMove(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        thisRT.anchoredPosition += moveVal;
    }
}
