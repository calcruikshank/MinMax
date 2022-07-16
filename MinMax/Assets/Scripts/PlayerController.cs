using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 lastLookedPosition;
    Vector2 lookDirection;
    Vector3 movement;
    Vector2 inputMovement;

    public State state;
    public Stats stats;

    float currentSpeed;
    //float acceleration;
    //float currentSpeed;

    Gun gun;

    [SerializeField]
    private Animator legsAnim;
    [SerializeField]
    private Animator armsAnim;

    public enum State
    {
        Normal,
        Knockback,
        Diving,
        WithPuck
    }

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        stats = this.GetComponent<Stats>();
        stats.Init(this);
        state = State.Normal;
        gun = GetComponentInChildren<Gun>();
        gun.Init(this);
        this.transform.localScale /= (stats.PlayerSize);
        currentSpeed = stats.Speed;
    }

    void Update()
    {
        switch (state)
        {
            case State.Normal:
                FaceLookDirection();
                HandleMovement();
                break;
        }
    }
    void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                FixedHandleMovement();
                break;
        }
    }

    private void HandleMovement()
    {
        movement.x = inputMovement.x;
        movement.y = 0;
        movement.z = inputMovement.y;
        /*if (movement.magnitude != 0)
        {
            currentSpeed += 165 * Time.deltaTime;
        }
        if (movement.magnitude == 0)
        {
            currentSpeed = 0f;
        }*/

    }
    private void FixedHandleMovement()
    {
        //rb.AddForce(movement.normalized * moveSpeed);
        rb.velocity = movement * currentSpeed;
        Debug.Log(rb.velocity.magnitude);

        if (rb.velocity.magnitude != 0f)
        {
            legsAnim.SetBool("isMoving", true);
        } 
        else
        {
           legsAnim.SetBool("isMoving", false);
        }
        /*if (rb.velocity.magnitude > stats.Speed)
        {
            rb.velocity = rb.velocity.normalized * stats.Speed;
        }
        if (movement.magnitude == 0)
        {
            rb.velocity *= .97f;
        }*/
    }
    void FaceLookDirection()
    {
        Vector3 lookTowards = new Vector3(lookDirection.x, 0, lookDirection.y);
        if (lookTowards.magnitude != 0f)
        {
            lastLookedPosition = lookTowards;
        }

        transform.right = lastLookedPosition;

    }

    void OnMove(InputValue value)
    {
        inputMovement = value.Get<Vector2>();
    }
    void OnLook(InputValue value)
    {
        lookDirection = value.Get<Vector2>();
    }
    void OnFireDown()
    {
        gun.fire = true;
        currentSpeed /= stats.SpeedReductionWhenFiring;
    }
    void OnFireUp()
    {
        gun.fire = false;
        currentSpeed = stats.Speed;
    }

    public void TakeDamage(float damageSent)
    {
        stats.HP -= damageSent;
        if (stats.HP < 0)
        {
           Die();
        }
    }
    public void Die()
    {
        Destroy(this.gameObject);
    }

}
