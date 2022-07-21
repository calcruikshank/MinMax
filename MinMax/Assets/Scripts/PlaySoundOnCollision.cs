using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    void OnTriggerEnter()
    {
        if (SoundManager.singleton == null) return;
        SoundManager.singleton.PlayRandomDieHitSound();
    }
}
