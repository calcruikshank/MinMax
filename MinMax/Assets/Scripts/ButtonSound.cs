using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public void SoundOnClick()
    {
        SoundManager.singleton.PlaySound(0);
    }
}
