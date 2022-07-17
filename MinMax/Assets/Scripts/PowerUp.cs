using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public GameObject icePrefab;
    [SerializeField] List<Color> colors = new List<Color>();

    private enum PowerT{
        projectiles = 0,
        hp,
        ice,
        lightning
    }

    PowerT type;
    // Start is called before the first frame update
    void Start()
    {
        type = (PowerT) Random.Range(0,4);
        SetColor(type);
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    stats.ProjectileSpeed -= 30;
                    if(stats.ProjectileSpeed < 10f)
                        stats.ProjectileSpeed = 10;
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
                break;
                    
            }
            GameManager.g.RemovePower(this);
            Destroy(this.gameObject);
        }
    }
}
