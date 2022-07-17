using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float Speed = 10f;
    public float HP = 10;
    public float AttackCooldown = 1f;
    public float AttackDamage = 1f;
    public float ProjectileSpeed = 30f;
    public float NumberOfProjectiles = 1f;
    public float ProjectileSize = 1f;
    public float PlayerSize = 1f;
    public float ProjectileRange = 30f;
    public float SpeedReductionWhenFiring = 2f;

    PlayerController player;

    public void Init(PlayerController playerSent)
    {
        player = playerSent;
    }

    public void SetSpeed(int numSent)
    {
        Speed = 6 + (2 * numSent);
    }
    public void SetHP(int numSent)
    {
        HP = numSent * 30;
    }
    public void SetAttackCooldown(int numSent)
    {
        AttackCooldown = numSent;
    }
    public void SetAttackDamage(int numSent)
    {
        AttackDamage = numSent * 10;
    }
    public void SetProjectileSpeed(int numSent)
    {
        ProjectileSpeed = 15 + (numSent * 5);
    }
    public void SetNumberOfProjectiles(int numSent)
    {
        NumberOfProjectiles = numSent;
    }
    public void SetProjectileSize(int numSent)
    {
        ProjectileSize = numSent;
    }
    public void SetPlayerSize(int numSent)
    {
        PlayerSize = numSent / 2;
    }
    public void SetProjectileRange(int numSent)
    {
        ProjectileRange = numSent;
    }
    public void SetSpeedReductionWhenFiring(int numSent)
    {
        
    }
}
