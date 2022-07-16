using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float Speed = 10f;
    public float HP;
    public float AttackCooldown = 1f;
    public float AttackDamage = 1f;
    public float ProjectileSpeed = 30f;
    public int NumberOfProjectiles = 2;
    public float ProjectileSize = 1f;
    public float PlayerSize = 1f;
    public float ProjectileRange = 30f;
    public float SpeedReductionWhenFiring = 2f;

    PlayerController player;

    public void Init(PlayerController playerSent)
    {
        player = playerSent;
    }

}
