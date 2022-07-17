using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public bool fire = false;
    public bool hasFiredForAnim = false;

    PlayerController playerOwningGun;

    public void Init(PlayerController playerSent)
    {
        playerOwningGun = playerSent;
        SetAnimationSpeedOnWand();
    }

    void SetAnimationSpeedOnWand()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            //Debug.Log(playerOwningGun.armsAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            if (playerOwningGun.currentPercentage >= 0.3f && !hasFiredForAnim)
            {
                hasFiredForAnim = true;
                switch (playerOwningGun.stats.NumberOfProjectiles)
                {
                    case 1:
                        boom(0);
                        break;
                    case 2:
                        boom(15);
                        boom(-15);
                        break;
                    case 6:
                        boom(160);
                        boom(180);
                        boom(200);
                        boom(20);
                        boom(0);
                        boom(-25);
                        break;
                    case 3:
                        boom(20);
                        boom(0);
                        boom(-25);
                        break;
                    case 4:
                        boom(15);
                        boom(25);
                        boom(-15);
                        boom(-25);
                        break;
                    case 5:
                        boom(0);
                        boom(90);
                        boom(45);
                        boom(-45);
                        boom(-90);
                        break;
                    case 7:
                        boom(0);
                        boom(90);
                        boom(45);
                        boom(-45);
                        boom(-90);
                        boom(160);
                        boom(200);
                        break;
                }
        }
        
    }

    void boom(float angleOffset)
    {
        Bullet b;
        b = Instantiate(bullet, transform.position, transform.rotation).GetComponent<Bullet>();
        b.transform.localEulerAngles = new Vector3(b.transform.localEulerAngles.x, b.transform.localEulerAngles.y + angleOffset, b.transform.localEulerAngles.z);
        b.Init(playerOwningGun, transform.position + b.transform.forward * playerOwningGun.stats.ProjectileRange);
    }
}
