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
    Vector2 inputFace;

    public State state;
    public Stats stats;

    float currentSpeed;
    //float acceleration;
    //float currentSpeed;

    public Gun gun;
  
    [SerializeField] private Animator legsAnim; 
    public Animator armsAnim;

    [SerializeField] Transform playerTorso;
    [SerializeField] Transform playerLegs;

    [SerializeField] Transform dieBody;
    [SerializeField] Animator capeAnim;
    [SerializeField] List<Vector3> faceRotations;
    [SerializeField] List<Color> faceColors;
    int currentFaceIndex = 0;
    Vector3 targetFaceRot;
    Color targetColor;

    public float entryTime;
    public float currentPercentage;

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

        targetColor = faceColors[0];
    }

    void Update()
    {
        switch (state)
        {
            case State.Normal:
                FaceLookDirection();
                HandleMovement();
                TrackAnimation(armsAnim.GetCurrentAnimatorStateInfo(0));
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
        if(movement.normalized != Vector3.zero)
        {
            playerLegs.transform.forward = Vector3.Lerp(playerLegs.transform.forward, movement.normalized, 20f * Time.deltaTime);
        }

        legsAnim.speed = currentSpeed / stats.Speed; 
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
        else
        {
            //if you aren't actively looking anywhere look where you are going. 
            lastLookedPosition = movement.normalized;
        }

        playerTorso.transform.forward = Vector3.Lerp(playerTorso.transform.forward, lastLookedPosition, 20f * Time.deltaTime);

        //player face spin
        if(dieBody.localRotation != Quaternion.Euler(targetFaceRot))
        {
            dieBody.localRotation = Quaternion.Lerp(dieBody.localRotation, Quaternion.Euler(targetFaceRot), 10f * Time.deltaTime);
        }
        //player face color
        Material mat = dieBody.GetComponent<MeshRenderer>().material;
        if(mat.GetColor("_Color") != targetColor)
        {
            mat.SetColor("_Color", Color.Lerp(mat.GetColor("_Color"), targetColor, 10f * Time.deltaTime));
        }
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
        entryTime = Time.time;
        gun.fire = true;
        currentSpeed /= stats.SpeedReductionWhenFiring;
        armsAnim.SetBool("cast", true);
        StartCoroutine("ResetCast");
    }
    void OnFireUp()
    {
        gun.fire = false;
        currentSpeed = stats.Speed;
        armsAnim.SetBool("cast", false);
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

    void OnFaceSelect(InputValue value)
    {
        inputFace = value.Get<Vector2>();
        ChangeFace();
    }

    void ChangeFace()
    {
        currentFaceIndex += (int)inputFace.normalized.x;
        if (currentFaceIndex < 0)
        {
            currentFaceIndex = faceRotations.Count - 1;
        }
        if(currentFaceIndex > faceRotations.Count - 1)
        {
            currentFaceIndex = 0;
        }

        capeAnim.Play("CapeAnim");
    
        targetFaceRot = faceRotations[currentFaceIndex];
        targetColor = faceColors[currentFaceIndex];
    }

    private void TrackAnimation(AnimatorStateInfo stateInfo)
    {
        //Debug.Log(currentPercentage % stateInfo.length * stateInfo.speed);
        currentPercentage = (Time.time - entryTime) / stateInfo.length;
        /*if(currentPercentage % stateInfo.length * stateInfo.speed <= .45f)
        {
            gun.hasFiredForAnim = false;
        }*/
        if(gun.fire == true)
        {
            /*if (!armsAnim.runtimeAnimatorController.animationClips[0].length * ("Arms_Cast 1"))
            {
                armsAnim.Play("Arms_Cast 1");
                gun.hasFiredForAnim = false;
                entryTime = Time.time;
            }
            */

        }



    }

    private IEnumerator ResetCast()
    {
        yield return new WaitForSeconds(0.1f);
        armsAnim.SetBool("cast", false);
    
    }
}
