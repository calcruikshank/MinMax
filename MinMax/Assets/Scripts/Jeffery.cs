using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jeffery : MonoBehaviour
{
    PlayerController controller;
    PlayerController target = null;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //controller.inputMovement = new Vector2(.1f,.1f);
        foreach(var p in GameManager.g.Players){
            if(p!=controller){
                target = p;
            }
        }
        if(target != null){
            var dir = transform.position - target.transform.position;
            controller.inputMovement = new Vector2(dir.x,dir.z);            
        }
    }
}
