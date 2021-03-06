using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public GameObject icePrefab;
    public GameObject lightningPrefab;
    public GameObject homingPrefab;
    [SerializeField] List<Color> colors = new List<Color>();

    private enum PowerT{
        projectiles = 0,
        hp,
        ice,
        lightning,
        homing
    }

    PowerT type;
    // Start is called before the first frame update
    void Start()
    {
        type = (PowerT) Random.Range(0,5);
        SetColor(type);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -10f){
            Destroy(this);
        }
    }

    private void SetColor(PowerT type)
    {
        transform.GetComponent<MeshRenderer>().material.SetColor("_Color", colors[(int)type]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            var playerHit = other.GetComponent<PlayerController>();
            var stats = playerHit.stats;
            switch(type){
                case PowerT.projectiles:
                    if(stats.NumberOfProjectiles<7){
                        stats.NumberOfProjectiles++;
                    }
                    break;
                case PowerT.hp:
                    stats.HP = stats.maxHP;
                    break;
                case PowerT.ice:
                    stats.ProjectileRange += 10;
                    stats.AttackDamage += 30;
                    stats.ProjectileSize += 0.5f;
                    stats.ProjectileSpeed -= 40;
                    if(stats.ProjectileSpeed < 5f)
                        stats.ProjectileSpeed = 5;
                    playerHit.gun.bullet = icePrefab;
                break;
                case PowerT.lightning:
                    stats.ProjectileRange -= 10;
                    if(stats.ProjectileRange < 10f){
                        stats.ProjectileRange = 10f;
                    }
                    stats.AttackDamage += 10f;
                    stats.ProjectileSize -= .5f;
                    stats.ProjectileSpeed += 20f;
                    stats.AttackCooldown += 2f;
                    playerHit.gun.bullet = lightningPrefab;
                    break;
                case PowerT.homing:
                    stats.Homing = true;
                    playerHit.gun.bullet = homingPrefab;
                    break;

            }
            GameManager.g.RemovePower(this);
            Destroy(this.gameObject);
        }
    }
}
