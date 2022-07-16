using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector3 inputMovement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void OnMove(InputValue value)
    {
        inputMovement = value.Get<Vector2>();
    }

    void OnLook()
    {
    }

    void OnFire()
    {
    }

}
