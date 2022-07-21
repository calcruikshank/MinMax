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

    public Vector3 startPosition;
    private float distMoved;
    public float attackDamage;
    private float projectileSize;
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

    internal void Init(PlayerController playerOwningGun, Vector3 endPosition, bool isCrit = false)
    {
        playerOwningBullet = playerOwningGun;
        var s = playerOwningBullet.stats;
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
        float percentMoved = distMoved/Vector3.Distance(targetPosition,startPosition);
        transform.localPosition = Vector3.Lerp(startPosition, targetPosition, percentMoved);

        if(transform.localPosition == targetPosition){
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
                Die();
            }
        } 
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);

    }

    public void Die(){
        Instantiate(puffParticles,transform.position,Quaternion.identity,null);
        GameManager.g.RemoveBullet(this);
        Destroy(this.gameObject);
    }
}
