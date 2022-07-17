using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandDispel : MonoBehaviour
{
    PlayerController playerOwningWand;
    private void Start()
    {
        playerOwningWand = this.transform.root.GetComponent<PlayerController>();   
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>() != null)
        {
            Bullet bullet = other.GetComponent<Bullet>();
            bullet.playerOwningBullet = playerOwningWand;
            bullet.ShootAt(this.transform.root.position + playerOwningWand.gun.transform.forward * playerOwningWand.stats.ProjectileRange);
            Debug.Log("Trigger enter");
        }
    }
}