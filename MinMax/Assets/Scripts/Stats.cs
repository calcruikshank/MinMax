using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float Speed = 10f;
    public float HP = 10;
    public float maxHP = 10;
    public float AttackCooldown = 1f;
    public float AttackDamage = 1f;
    public float ProjectileSpeed = 30f;
    public float NumberOfProjectiles = 1f;
    public float ProjectileSize = 1f;
    public float PlayerSize = 1f;
    public float ProjectileRange = 30f;
    public float SpeedReductionWhenFiring = 2f;
    public float DispelSpeed = 2f;

    PlayerController player;

    public void Init(PlayerController playerSent)
    {
        player = playerSent;
    }

    public void SetSpeed(int numSent)
    {
        Speed = 5 + (2 * numSent);
    }
    public void SetHP(int numSent)
    {
        HP = 200 + numSent * 50;
        maxHP = HP;
    }
    public void SetAttackCooldown(int numSent)
    {
        AttackCooldown = ((float)numSent / 2f) * 1.0f;
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
        ProjectileSize = .2f + ((float)numSent)/4f;
    }
    public void SetPlayerSize(int numSent)
    {
        PlayerSize = numSent / 2.0f;
    }
    public void SetProjectileRange(int numSent)
    {
        ProjectileRange = 5 + (numSent * 4);
    }
    public void SetSpeedReductionWhenFiring(int numSent)
    {
        
    }
    public void SetDispelSpeed(int numSent)
    {
        DispelSpeed = ((float)numSent / 2f) * 1.0f;
    }
    public void CopyStatsToOtherComponent(Stats otherStats)
    {
        otherStats.Speed = Speed;
        otherStats.HP = HP;
        otherStats.maxHP = maxHP;
        otherStats.AttackCooldown = AttackCooldown;
        otherStats.AttackDamage = AttackDamage;
        otherStats.ProjectileSpeed = ProjectileSpeed;
        otherStats.NumberOfProjectiles = NumberOfProjectiles;
        otherStats.ProjectileSize = ProjectileSize;
        otherStats.PlayerSize = PlayerSize;
        otherStats.ProjectileRange = ProjectileRange;
        otherStats.SpeedReductionWhenFiring = SpeedReductionWhenFiring;
    }
}
