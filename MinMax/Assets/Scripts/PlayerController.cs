using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 lastLookedPosition;
    public Vector2 lookDirection;
    Vector3 movement;
    public Vector2 inputMovement;
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

    [SerializeField] WandDispel wand;
    int currentFaceIndex = 0;
    Vector3 targetFaceRot;
    Color targetColor;

    public float entryTime;
    public float currentPercentage;
    public HealthPanelScript thisHPS;
    public Image circleImage;

    bool fireAnimationIsPlaying = false;
    bool dispelAnimationIsPlaying = false;

    bool pressedFireWhileDispelling = false;

    public GameObject dispellMesh;

    public float currentMana;

    PlayerInput playerInput;
    InputDevice currentDevice;
    public enum State
    {
        Normal,
        Firing,
        Rolling,
        Dispelling
    }
    void Awake()
    {
        stats = this.GetComponent<Stats>();
        stats.Init(this);
    }
    void Start()
    {
        GameManager.g.AddPlayer(this);

        rb = this.GetComponent<Rigidbody>();

        state = State.Normal;
        gun = GetComponentInChildren<Gun>();
        gun.Init(this);
        this.transform.localScale /= (stats.PlayerSize);
        currentSpeed = stats.Speed;

        targetColor = faceColors[0];
        currentMana = stats.manaPool;
        FindLowestPoint();

        playerInput = this.GetComponent<PlayerInput>();
        // Debug.Log(playerInput.currentControlScheme);
        if (thisHPS is null || thisHPS.healthSlider is null) return;
        thisHPS.healthSlider.value = GameManager.g.Remap(stats.HP, 0, stats.maxHP, 0, 1);
        thisHPS.playerHealthText.text = stats.HP.ToString() + "/" + stats.maxHP;

        if (thisHPS.manaSlider is null) return;
        thisHPS.manaSlider.value = GameManager.g.Remap(currentMana, 0, stats.manaPool, 0, 1);
    }

    void FindLowestPoint()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                this.transform.position = new Vector3(transform.position.x, 0 + this.transform.localScale.y, transform.position.z);
                Debug.Log(hit.transform);
            }
        }

    }
    void Update()
    {
        switch (state)
        {
            case State.Normal:
                FaceLookDirection();
                HandleMovement();
                HandleInput();
                HandleAnimationSpeeds();
                break;
            case State.Firing:
                FaceLookDirection();
                HandleMovement();
                HandleInput();
                FireAnimation(armsAnim.GetCurrentAnimatorStateInfo(0));
                HandleAnimationSpeeds();
                break;
            case State.Dispelling:
                HandleDispelAnimation(armsAnim.GetCurrentAnimatorStateInfo(0));
                FaceLookDirection();
                HandleMovement();
                HandleInput();
                HandleAnimationSpeeds();
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
            case State.Firing:
                FixedHandleMovement();
                break;
            case State.Dispelling:
                FixedHandleMovement();
                break;
        }
    }

    private void HandleMovement()
    {
        movement.x = inputMovement.x;
        movement.y = 0;
        movement.z = inputMovement.y;
        if (movement.normalized != Vector3.zero)
        {
            playerLegs.transform.forward = Vector3.Lerp(playerLegs.transform.forward, movement, 20f * Time.deltaTime);
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
        rb.velocity = new Vector3(movement.x * currentSpeed, rb.velocity.y, movement.z * currentSpeed);

        if (Mathf.Abs(rb.velocity.magnitude) >= .02f)
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
        if (dieBody.localRotation != Quaternion.Euler(targetFaceRot))
        {
            dieBody.localRotation = Quaternion.Lerp(dieBody.localRotation, Quaternion.Euler(targetFaceRot), 10f * Time.deltaTime);
        }
        //player face color
        Material mat = dieBody.GetComponent<MeshRenderer>().material;
        if (mat.GetColor("_Color") != targetColor)
        {
            mat.SetColor("_Color", Color.Lerp(mat.GetColor("_Color"), targetColor, 10f * Time.deltaTime));
        }
    }

    void OnMove(InputValue value)
    {
        inputMovement = value.Get<Vector2>();
    }

    [SerializeField] LayerMask groundLayer;
    void OnLook(InputValue value)
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            lookDirection = value.Get<Vector2>();
        }
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            Vector2 mousePosition;
            mousePosition = value.Get<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                lookDirection = new Vector2(hit.point.x - transform.position.x, hit.point.z - transform.position.z);
            }
        }
    }

    bool fireDownPressed = false;
    bool fireStillPressed = false;
    public void OnFireDown()
    {
        fireDownPressed = true;
        fireStillPressed = true;
    }

    public void OnFireUp()
    {
        fireDownPressed = false;
        fireStillPressed = false;
    }
    public bool dispelDownPressed = false;
    void OnDispelDown()
    {
        dispelDownPressed = true;
    }
    void OnDispelUp()
    {
        dispelDownPressed = false;
    }
    public void TakeDamage(float damageSent)
    {
        stats.HP -= damageSent;
        if (stats.HP <= 0)
        {
            SoundManager.singleton.PlaySound(10, 0.8f, 0.4f);
            Die();
        }
        else
        {
            SoundManager.singleton.PlaySound(9, 0.4f, 0.4f);
        }
        if (thisHPS is null) return;
        if (thisHPS.healthSlider is null) return;
        thisHPS.healthSlider.value = GameManager.g.Remap(stats.HP, 0, stats.maxHP, 0, 1);
        thisHPS.playerHealthText.text = Mathf.Clamp(stats.HP, 0, stats.maxHP).ToString() + "/" + stats.maxHP;
    }
    public void Die()
    {
        GameManager.g.RemovePlayer(this);
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
        if (currentFaceIndex > faceRotations.Count - 1)
        {
            currentFaceIndex = 0;
        }

        capeAnim.Play("CapeAnim");

        targetFaceRot = faceRotations[currentFaceIndex];
        targetColor = faceColors[currentFaceIndex];
    }

    public void PickRandomFace()
    {
        currentFaceIndex = Random.Range(0, faceRotations.Count);
        ChangeFace();
    }

    void HandleInput()
    {
        if (dispelDownPressed)
        {
            if (dispelAnimationIsPlaying) return;
            dispelDownPressed = false;
            currentSpeed /= stats.SpeedReductionWhenFiring;
            StartDispelAnimation();
        }


        //anything past this point requires mana to cast
        currentMana = Mathf.Clamp(currentMana + (Time.deltaTime * stats.manaRegenRate), 0, stats.manaPool);
        if (thisHPS != null)
        {
            thisHPS.manaSlider.value = GameManager.g.Remap(currentMana, 0, stats.manaPool, 0, 1);
        }

        if (currentMana < stats.manaCost)
        {
            return;
        }
        if (fireDownPressed)
        {
            if (fireAnimationIsPlaying) return;
            if (dispelAnimationIsPlaying == true) return;
            if (dispelDownPressed) return;
            fireDownPressed = false;
            currentSpeed /= stats.SpeedReductionWhenFiring;
            ChangeStateToFire();
        }
    }

    void StartDispelAnimation()
    {
        if (SoundManager.singleton != null)
        {
            SoundManager.singleton.PlaySound(6, 0.4f, 0.4f);
        }
        dispelEntryTime = Time.time;
        ChangeStateToDispel();

    }

    public float currentDispelPercentage;

    [SerializeField] Transform dispelPoint;
    GameObject newDispelObject;
    private void HandleDispelAnimation(AnimatorStateInfo stateInfo)
    {
        currentDispelPercentage = (Time.time - dispelEntryTime) / stateInfo.length;
        if (stateInfo.IsName("Dispel") && currentDispelPercentage > .4f && currentDispelPercentage <= 1.2f && newDispelObject == null)
        {
            newDispelObject = Instantiate(dispellMesh, dispelPoint.position, dispelPoint.rotation);
            newDispelObject.GetComponent<WandDispel>().Init(this);
            //newDispelObject.transform.parent = gun.transform;
            newDispelObject.SetActive(true);
        }
        if (newDispelObject != null)
        {
            newDispelObject.transform.position = dispelPoint.position;
            newDispelObject.transform.rotation = dispelPoint.rotation;
        }
        if (stateInfo.IsName("Dispel") && currentDispelPercentage > 1.2f)
        {
            Destroy(newDispelObject);
        }
        if (!stateInfo.IsName("Dispel") && currentDispelPercentage > .4f)
        {
            ChangeStateToNormal();
        }
    }
    private void FireAnimation(AnimatorStateInfo stateInfo)
    {
        currentPercentage = (Time.time - entryTime) / stateInfo.length;

        if (stateInfo.IsName("Arms_Cast 1") && currentPercentage > .4f && !gun.hasFiredForAnim)
        {
            gun.Fire();
        }
        Debug.Log(stateInfo.length);
        if (!stateInfo.IsName("Arms_Cast 1") && currentPercentage > .4f)
        {
            
            
            if (fireStillPressed == true && gun.hasFiredForAnim && currentMana >= stats.manaCost)
            {
                ChangeStateToFire();
            }

            if (!fireStillPressed == true && gun.hasFiredForAnim || currentMana < stats.manaCost)
            {
                ChangeStateToNormal();
            }
        }
    }
    
    float dispelEntryTime;


    void HandleAnimationSpeeds()
    {
        if (dispelAnimationIsPlaying)
        {
            armsAnim.speed = stats.DispelSpeed;
        }
        if (fireAnimationIsPlaying)
        {
            armsAnim.speed = stats.AttackCooldown;
        }
        if (!dispelAnimationIsPlaying && !fireAnimationIsPlaying)
        {
            armsAnim.speed = 1;
        }
    }
    public MeshRenderer hatRenderer, cloakRenderer;

    public void ChangeColor(Color c)
    {
        Material hatMat = new Material(hatRenderer.material);
        hatMat.color = c;
        hatRenderer.material = hatMat;
        Material cloakMat = new Material(cloakRenderer.material);
        cloakMat.color = c;
        cloakRenderer.material = cloakMat;
        circleImage.color = new Color(c.r, c.g, c.b, 190.0f);
    }


    #region changingOfStates
    void ChangeStateToNormal()
    {
        Debug.Log(currentDispelPercentage);
        currentSpeed = stats.Speed;
        dispelAnimationIsPlaying = false;
        //wand.GetComponent<Collider>().enabled = false;
        armsAnim.SetBool("dispel", false);
        armsAnim.SetBool("cast", false);
        fireAnimationIsPlaying = false;
        state = State.Normal;
    }
    void ChangeStateToFire()
    {
        gun.hasFiredForAnim = false;
        entryTime = Time.time;
        dispelAnimationIsPlaying = false;
        fireAnimationIsPlaying = true;
        armsAnim.SetBool("cast", true);
        armsAnim.SetBool("dispel", false);
        state = State.Firing;
    }
    void ChangeStateToDispel()
    {
        fireAnimationIsPlaying = false;
        dispelAnimationIsPlaying = true;
        armsAnim.SetBool("dispel", true);
        armsAnim.SetBool("cast", false);
        state = State.Dispelling;
    }
    #endregion
}
