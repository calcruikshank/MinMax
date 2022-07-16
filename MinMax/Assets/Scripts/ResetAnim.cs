using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnim : MonoBehaviour
{
    PlayerController player;
    void Start()
    {
        player = transform.root.GetComponent<PlayerController>();
    }

    public void OnaAnimStart()
    {
        player.gun.hasFiredForAnim = false;
        player.entryTime = Time.time;
    }
}
