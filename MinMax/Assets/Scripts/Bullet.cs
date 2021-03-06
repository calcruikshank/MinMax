using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bullet : MonoBehaviour
{
    public bool isCrit = false;
    public float velocity;
    public Vector3 targetPosition;
    public GameObject critCanvas;

    public Vector3 startPosition;
    private float distMoved;
    public float attackDamage;
    private float projectileSize;
    private float projectileRange;
    private GameObject homingTarget = null;
    public PlayerController playerOwningBullet;
    public bool isHoming;
    bool hasHitPlayer = false;
    public GameObject puffParticles;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.g.AddBullet(this);
    }

    public void ShootAt(Vector3 endPosition)
    {
        if(isHoming){
            SetHomingTarget();
        }
        startPosition = transform.localPosition;
        targetPosition = endPosition;
        distMoved = 0;
    }

    public void SetHomingTarget(){
        var players = GameManager.g.Players.OrderBy(x => Vector3.Distance(x.transform.position, transform.position + transform.forward * 10)).ToList();
        homingTarget = null;
        foreach(var player in players){
            if(player.GetComponent<PlayerController>() != playerOwningBullet && Vector3.Distance(transform.position + transform.forward * 10f,player.transform.position) < 15f){
                homingTarget = player;
                break;
            }
        }
    }

    internal void Init(PlayerController playerOwningGun, Vector3 target, bool isCrit = false)
    {
        
        playerOwningBullet = playerOwningGun;
        var s = playerOwningBullet.stats;
        projectileRange = s.ProjectileRange;
        Vector3 endPosition = target +transform.forward * projectileRange;
        isHoming = s.Homing;
        ShootAt(endPosition);
        velocity = s.ProjectileSpeed;
        attackDamage = s.AttackDamage;
        projectileSize = s.ProjectileSize;
        this.isCrit = isCrit;
        if(isCrit){
            attackDamage = attackDamage * 2;
            projectileSize = projectileSize * 1.5f;
        }
        transform.localScale = new Vector3(projectileSize,projectileSize,projectileSize);
    }

    // Update is called once per frame
    void Update()
    {
        if(homingTarget != null){
            targetPosition = homingTarget.transform.position;
        }
        distMoved += Time.deltaTime * velocity ;
        //float percentMoved = distMoved/Vector3.Distance(targetPosition,startPosition);
        //transform.localPosition = Vector3.Lerp(startPosition, targetPosition, percentMoved);
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, Time.deltaTime * velocity);
        if(Vector3.Distance(transform.localPosition, targetPosition) < 0.001f || distMoved >= projectileRange){
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitPlayer) return;
        if (other.GetComponent<PlayerController>() != null)
        {
            hasHitPlayer = true;
            var objectHit = other.GetComponent<PlayerController>();
            if(objectHit != playerOwningBullet){
                objectHit.TakeDamage(attackDamage);
                if (critCanvas != null)
                {
                    GameObject oof = Instantiate(critCanvas, transform.position, Camera.main.transform.rotation);
                    if (isCrit)
                    {
                        oof.GetComponent<DestroyWorldCanvas>().oofText.text = "critical oof!";
                    }
                    else
                    {
                        oof.GetComponent<DestroyWorldCanvas>().oofText.text = "oof x" + Mathf.RoundToInt(attackDamage).ToString();
                    }
                }
                Die();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    public void Die(){
        Instantiate(puffParticles,transform.position,Quaternion.identity,null);
        GameManager.g.RemoveBullet(this);
        Destroy(this.gameObject);
    }
}
