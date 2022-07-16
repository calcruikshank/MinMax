using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float velocity;
    private Vector3 targetPosition;

    private Vector3 startPosition;
    private float distMoved;
    private float attackDamage;
    private float projectileSize;
    public PlayerController playerOwningBullet;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
    }

    internal void Init(PlayerController playerOwningGun, Vector3 endPosition)
    {
        GameManager.g.AddBullet(this);
        targetPosition = endPosition;
        playerOwningBullet = playerOwningGun;
        var s = playerOwningBullet.stats;
        velocity = s.ProjectileSpeed;
        attackDamage = s.AttackDamage;
        projectileSize = s.ProjectileSize;
    }

    // Update is called once per frame
    void Update()
    {
        distMoved += Time.deltaTime * velocity ;
        float percentMoved = distMoved/Vector3.Distance(targetPosition,startPosition);
        transform.localPosition = Vector3.Lerp(startPosition, targetPosition, percentMoved);

        if(transform.localPosition == targetPosition){
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            var objectHit = other.GetComponent<PlayerController>();
            if(objectHit != playerOwningBullet){
                objectHit.TakeDamage(attackDamage);
                Die();
            }
        }
    }
    private void Die(){
        GameManager.g.RemoveBullet(this);
        Destroy(this.gameObject);
    }
}
