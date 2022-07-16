using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Jeffery : MonoBehaviour
{
    PlayerController controller;
    PlayerController target = null;

    float keepDistance = 10f;
    float keepRange = 2f;
    float keepFuzz = 0f;

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
        InvokeRepeating("SelectTarget", 2.0f, 0.3f);
        InvokeRepeating("Fuzz", 2.0f, 2f);
        LookAt();
        switch(state){
            case State.Chasing:
                if(target != null){
                    var distance = Vector3.Distance(target.transform.position, transform.position);
                    var dir = target.transform.position - transform.position;
                    if( distance > keepDistance + (keepRange + keepFuzz)){
                        controller.inputMovement = new Vector2(dir.x * .1f,dir.z *.1f);     
                    }else if(distance < keepDistance - (keepRange + keepFuzz)){
                        controller.inputMovement = new Vector2(dir.x * .1f * -1,dir.z *.1f * -1);     
                    }else{
                        controller.inputMovement = new Vector2(0,0);
                    }
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

    void Fuzz(){
        //keepFuzz = Random.Range(-1,1);
    }

    void LookAt(){
        if(target != null){
            var dir = target.transform.position - transform.position;
            controller.lookDirection = new Vector2(dir.x * .1f,dir.z *.1f);       
        }

    }

    List<GameObject> OrderByClosest(List<GameObject> objs, Vector3 origin){
        return objs.OrderBy(x => Vector3.Distance(x.transform.position, origin)).ToList();
    }
}
