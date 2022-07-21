using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public bool hasFiredForAnim = false;
    public GameObject critCanvas;

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

    public void Fire()
    {
        //Debug.Log(playerOwningGun.armsAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);

        playerOwningGun.currentMana -= playerOwningGun.stats.manaCost;
        hasFiredForAnim = true;
        if (SoundManager.singleton != null)
        {
            SoundManager.singleton.PlaySound(5, 0.4f, 0.4f);
        }
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
    // Update is called once per frame
    void Update()
    {


    }

    void boom(float angleOffset)
    {
        Bullet b;
        b = Instantiate(bullet, transform.position, transform.rotation).GetComponent<Bullet>();
        b.transform.localEulerAngles = new Vector3(b.transform.localEulerAngles.x, b.transform.localEulerAngles.y + angleOffset, b.transform.localEulerAngles.z);
        b.Init(playerOwningGun, transform.position + b.transform.forward * playerOwningGun.stats.ProjectileRange);
        int didCrit = Random.Range(0, 100);
        if (didCrit <= playerOwningGun.stats.critChance)
        {
            Debug.Log("crit!");
            if (critCanvas != null)
            {
                GameObject c = Instantiate(critCanvas, transform.position, Camera.main.transform.rotation);
                Destroy(c, 2);
            }
            b.attackDamage = b.attackDamage * 2;
            b.transform.localScale = new Vector3(b.transform.localScale.x * 1.5f, b.transform.localScale.y * 1.5f, b.transform.localScale.z * 1.5f);
        }
    }
}
