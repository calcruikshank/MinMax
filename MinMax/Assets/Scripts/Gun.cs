using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public float cooldown = 1;
    public bool fire = true;
    public float startingVelocity = 100;
    public float range = 10;

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
        if(curTime >= cooldown){
            curTime = 0;
            Bullet newBullet = Instantiate(bullet, transform.position, transform.rotation).GetComponent<Bullet>();
            newBullet.velocity = startingVelocity;
            newBullet.targetPosition = transform.position + transform.forward * range;
            newBullet.Init(playerOwningGun);
        }
    }
}
