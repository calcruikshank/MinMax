using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float Speed = 10f;
    public float HP = 10;
    public float AttackSpeed = 1;
    public float AttackDamage = 1;
    public float ProjectileSpeed = 1;
    public float NumberOfProjectiles = 1;
    public float ProjectileSize = 1;
    public float PlayerSize = 6;
    public float Range = 1;

    PlayerController player;

    public void Init(PlayerController playerSent)
    {
        player = playerSent;
    }
    public void TakeDamage(float damageSent)
    {
        HP -= damageSent;
        if (HP < 0)
        {
            player.Die();
        }
    }

}
