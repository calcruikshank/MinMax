using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    void OnTriggerEnter()
    {
        SoundManager.singleton.PlayRandomStepSound();
    }
}
