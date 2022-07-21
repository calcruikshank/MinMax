using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandDispel : MonoBehaviour
{
    PlayerController playerOwningWand;
    private void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>() != null)
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet.playerOwningBullet == this.playerOwningWand) return;
            bullet.playerOwningBullet = playerOwningWand;
            bullet.transform.rotation = transform.root.rotation;
            bullet.ShootAt(playerOwningWand.transform.root.position + playerOwningWand.gun.transform.forward * playerOwningWand.stats.ProjectileRange);
            bullet.velocity = bullet.velocity * 1.5f;
            bullet.attackDamage = bullet.attackDamage * 1.5f;
            Debug.Log("Trigger enter");
        }
    }

    public void Init(PlayerController playerSent)
    {
        playerOwningWand = playerSent;
    }
}
