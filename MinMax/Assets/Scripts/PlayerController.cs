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
        thisHPS.healthSlider.value = Utils.Remap(stats.HP, 0, stats.maxHP, 0, 1);
        thisHPS.playerHealthText.text = stats.HP.ToString() + "/" + stats.maxHP;
        //Lets calculate the max max hp, we're doing this 4x, sue me
        float maxMaxHP = 0;
        foreach(var player in GameManager.g.Players){
            if(player.GetComponent<Stats>().maxHP > maxMaxHP){
                maxMaxHP = player.GetComponent<Stats>().maxHP;
            }
        }
        //I hate UI
        float calc = (200-(stats.maxHP/maxMaxHP)*200) + 10;
        thisHPS.healthSlider.GetComponent<RectTransform>().offsetMax = new Vector2(-1*calc, thisHPS.healthSlider.GetComponent<RectTransform>().offsetMax.y);

        if (thisHPS.manaSlider is null) return;
        thisHPS.manaSlider.value = Utils.Remap(currentMana, 0, stats.manaPool, 0, 1);
    }

    void FindLowestPoint()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                this.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
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
                HandleFireInput();
                HandleDispelInput();
                HandleRollInput();
                HandleAnimationSpeeds();
                break;
            case State.Firing:
                FaceLookDirection();
                HandleMovement();
                HandleDispelInput();
                HandleRollInput();
                FireAnimation(armsAnim.GetCurrentAnimatorStateInfo(0));
                HandleAnimationSpeeds();
                break;
            case State.Dispelling:
                HandleDispelAnimation(armsAnim.GetCurrentAnimatorStateInfo(0));
                FaceLookDirection();
                HandleMovement();
                HandleRollInput();
                HandleAnimationSpeeds();
                break;
            case State.Rolling:
                HandleRollAnimation();
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
            case State.Rolling:
                FixedHandleRoll();
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
    float currentRollSpeed;
    private void FixedHandleRoll()
    {
        //rb.AddForce(movement.normalized * moveSpeed);
        rb.velocity = new Vector3(playerTorso.transform.forward.x * currentRollSpeed, rb.velocity.y, playerTorso.transform.forward.z * currentRollSpeed);

        if (Mathf.Abs(rb.velocity.magnitude) >= .02f)
        {
            legsAnim.SetBool("isMoving", true);
        }
        else
        {
            legsAnim.SetBool("isMoving", false);
        }
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

    bool rollDownPressed = false;
    void OnRollDown()
    {
        rollDownPressed = true;
    }
    void OnRollUp()
    {
        rollDownPressed = false;
    }

    public void TakeDamage(float damageSent)
    {
        stats.HP -= damageSent;
        if (stats.HP <= 0)
        {
            SoundManager.singleton.PlaySound(10, 0.8f, 0.2f, basePitch:1*stats.PlayerSize);
            Die();
        }
        else
        {
            SoundManager.singleton.PlaySound(9, 0.8f, 0.2f, basePitch:1*stats.PlayerSize);
        }
        if (thisHPS is null) return;
        if (thisHPS.healthSlider is null) return;
        thisHPS.healthSlider.value = Utils.Remap(stats.HP, 0, stats.maxHP, 0, 1);
        thisHPS.playerHealthText.text = Mathf.Clamp(stats.HP, 0, stats.maxHP).ToString() + "/" + stats.maxHP;
    }
    public void Die()
    {
        SpawnPlayerBody();
        GameManager.g.RemovePlayer(this);
        Destroy(this.gameObject);
    }
    void SpawnPlayerBody()
    {
        dieBody.transform.parent = null;
        dieBody.GetComponent<MeshCollider>().enabled = true;
        Rigidbody rb = dieBody.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(Vector3.up * 500f);
        rb.AddTorque(dieBody.transform.right * Random.Range(-500f, 500f));
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

    void HandleFireInput()
    {
        currentMana = Mathf.Clamp(currentMana + (Time.deltaTime * stats.manaRegenRate), 0, stats.manaPool);
        if (thisHPS != null)
        {
            thisHPS.manaSlider.value = Utils.Remap(currentMana, 0, stats.manaPool, 0, 1);
        }

        if (currentMana < stats.manaCost)
        {
            return;
        }
        if (fireDownPressed)
        {
            fireDownPressed = false;
            currentSpeed /= stats.SpeedReductionWhenFiring;
            ChangeStateToFire();
        }
    }
    void HandleDispelInput()
    {
        if (dispelDownPressed)
        {
            dispelDownPressed = false;
            currentSpeed /= stats.SpeedReductionWhenFiring;
            StartDispelAnimation();
        }
    }
    void HandleRollInput()
    {
        if (rollDownPressed)
        {
            dispelDownPressed = false;
            currentSpeed /= stats.SpeedReductionWhenFiring;
            StartRollAnimation();
        }
    }

    void StartDispelAnimation()
    {
        dispelEntryTime = Time.time;
        ChangeStateToDispel();
    }

    float rollEntryTime;
    void StartRollAnimation()
    {
        dispelEntryTime = Time.time;
        playerTorso.transform.forward = lastLookedPosition;
        currentRollSpeed = stats.initialRollSpeed;
        ChangeStateToRoll();
    }

    public float currentDispelPercentage;

    [SerializeField] Transform dispelPoint;
    GameObject newDispelObject;
    private void HandleDispelAnimation(AnimatorStateInfo stateInfo)
    {
        currentDispelPercentage = (Time.time - dispelEntryTime) / stateInfo.length;
        if (stateInfo.IsName("Dispel") && currentDispelPercentage > .5f && currentDispelPercentage <= .9f && newDispelObject == null)
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
    void HandleRollAnimation()
    {
        float currentRollSpeedMulti = 3f;
        currentRollSpeed -= currentRollSpeedMulti * Time.deltaTime;

    }
    private void FireAnimation(AnimatorStateInfo stateInfo)
    {
        currentPercentage = (Time.time - entryTime) / stateInfo.length;

        if (stateInfo.IsName("Arms_Cast 1") && currentPercentage > .4f && !gun.hasFiredForAnim)
        {
            gun.Fire();
        }
        // Debug.Log(stateInfo.length);
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
        if (state == State.Dispelling)
        {
            armsAnim.speed = stats.DispelSpeed;
        }
        if (state == State.Firing)
        {
            armsAnim.speed = stats.AttackCooldown;
        }
        if (state == State.Normal)
        {
            armsAnim.speed = 1;
        }
        if (state == State.Rolling)
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
        // Debug.Log(currentDispelPercentage);
        currentSpeed = stats.Speed;
        //wand.GetComponent<Collider>().enabled = false;
        armsAnim.SetBool("dispel", false);
        armsAnim.SetBool("cast", false);
        armsAnim.SetBool("roll", false);
        state = State.Normal;
    }
    void ChangeStateToFire()
    {
        gun.hasFiredForAnim = false;
        entryTime = Time.time;
        armsAnim.SetBool("cast", true);
        armsAnim.SetBool("dispel", false);
        armsAnim.SetBool("roll", false);
        state = State.Firing;
    }
    void ChangeStateToDispel()
    {
        armsAnim.SetBool("dispel", true);
        armsAnim.SetBool("cast", false);
        armsAnim.SetBool("roll", false);
        state = State.Dispelling;
    }
    void ChangeStateToRoll()
    {
        // Debug.Log(currentDispelPercentage);
        currentSpeed = stats.Speed;
        //wand.GetComponent<Collider>().enabled = false;
        armsAnim.SetBool("dispel", false);
        armsAnim.SetBool("cast", false);
        armsAnim.SetBool("roll", true);
        state = State.Rolling;
    }
    #endregion
}
