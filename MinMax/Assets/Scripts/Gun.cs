using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public bool fire = false;

    PlayerController playerOwningGun;
    //Timing stuff
    private float nextFire = 0;
    private float curTime = 0;
    
    public void Init(PlayerController playerSent)
    {
        playerOwningGun = playerSent;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if(curTime >= playerOwningGun.stats.AttackCooldown){
            curTime = 0;
            if(fire){
                Bullet newBullet = Instantiate(bullet, transform.position, transform.rotation).GetComponent<Bullet>();
                newBullet.Init(playerOwningGun,transform.position + transform.forward * playerOwningGun.stats.ProjectileRange);
            }

        }
    }
}
