using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Jeffery : MonoBehaviour
{
    PlayerController controller;
    PlayerController target = null;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<PlayerController>();
        state = State.Chasing;
    }
    private enum State
    {
        Chasing,
    }
    State state;
    // Update is called once per frame
    void Update()
    {
        SelectTarget();
        
        switch(state){
            case State.Chasing:
                if(target != null){
                    var dir = target.transform.position - transform.position;
                    controller.inputMovement = new Vector2(dir.x * .1f,dir.z *.1f);            
                }
            break;
        }
    }

    void SelectTarget(){
        var listOfClosest = OrderByClosest(GameManager.g.Players,transform.position);
        foreach(var obj in listOfClosest){
            if(obj != gameObject){
                target = obj.GetComponent<PlayerController>();
            }
        }
    }

    void LookAt(){
                            var dir = target.transform.position - transform.position;
                    controller.inputMovement = new Vector2(dir.x * .1f,dir.z *.1f);       
    }

    List<GameObject> OrderByClosest(List<GameObject> objs, Vector3 origin){
        return objs.OrderBy(x => Vector3.Distance(x.transform.position, origin)).ToList();
    }
}
