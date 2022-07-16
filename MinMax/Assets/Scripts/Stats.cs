using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float Speed = 10f;
    public float HP;
    public float AttackSpeed;
    public float AttackDamage;
    public float ProjectileSpeed;
    public float NumberOfProjectiles;
    public float ProjectileSize;
    public float PlayerSize;
    public float Range;

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
