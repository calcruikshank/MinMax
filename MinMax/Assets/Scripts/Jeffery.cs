using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Jeffery : MonoBehaviour
{
    PlayerController controller;
    PlayerController target = null;
    GameObject dodgeTarget = null;
    Vector2 moveDirection;
    float keepDistance = 10f;
    float keepRange = 2f;
    float keepFuzz = 0f;
    bool isFiring;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<PlayerController>();
        state = State.Chasing;
    }
    private enum State
    {
        Chasing,
        Dodge
    }
    State state;
    // Update is called once per frame
    void Update()
    {
        float speed = 1f;
        InvokeRepeating("SelectTarget", 1.0f, 1f);
        InvokeRepeating("Fuzz", 2.0f, 2f);
        LookAt();
        CheckDodge();
        switch(state){
            case State.Chasing:
                if(target != null){
                    var distance = Vector3.Distance(target.transform.position, transform.position);
                    var dir = target.transform.position - transform.position;
                    if( distance > keepDistance + (keepRange + keepFuzz)){
                        moveDirection = new Vector2(dir.x,dir.z);     
                    }else if(distance < keepDistance - (keepRange + keepFuzz)){
                        moveDirection = new Vector2(dir.x*-1,dir.z*-1);     
                    }else{
                        moveDirection = new Vector2(0,0);
                    }
                    speed = .2f;
                }else{
                    moveDirection = new Vector2(0,0);
                }
            break;
            case State.Dodge:
                if(dodgeTarget != null){
                    var bullet = dodgeTarget.GetComponent<Bullet>();
                    Vector3 vec;
                    var leftDistance = Vector3.Distance(bullet.transform.position + bullet.transform.TransformDirection(Vector3.left)*2, transform.position);
                    var rightDistance = Vector3.Distance(bullet.transform.position + bullet.transform.TransformDirection(Vector3.right)*2, transform.position);
                    if(leftDistance<rightDistance){
                        vec = bullet.transform.TransformDirection(Vector3.left);
                    }
                    else{
                        vec = bullet.transform.TransformDirection(Vector3.right);
                    }
                    moveDirection = new Vector2(vec.x, vec.z);
                    speed = 1f;
                    if(!RayCastBullet(bullet)){
                        state = State.Chasing;
                    }
                }else{
                    state = State.Chasing;
                }
                break;
        }
        controller.inputMovement = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
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
            var gun = controller.GetComponentInChildren<Gun>();
            var rb = target.GetComponent<Rigidbody>();
            var dist = Vector3.Distance(target.transform.position,gun.transform.position);
            var timeToTarget = dist / controller.stats.ProjectileSpeed;
            var predictedPos  = target.transform.position + rb.velocity * timeToTarget;
            var dir = predictedPos - gun.transform.position;
            controller.lookDirection = new Vector2(dir.x * .1f,dir.z *.1f);
            Fire();
        }else{
            StopFire();
        }

    }
    void StopFire(){
        if(isFiring){
            controller.OnFireUp();
            isFiring = false;
        }
    }

    void Fire(){
        if(state != State.Dodge){
            if(!isFiring){
                controller.OnFireDown();
                isFiring = true;
            }
        }
    }

    void CheckDodge(){
        var listOfClosest = OrderByClosest(GameManager.g.Bullets,transform.position);
        foreach(var obj in listOfClosest){
            var bullet = obj.GetComponent<Bullet>();
            if(bullet.playerOwningBullet != controller){
                var distance = Vector3.Distance(bullet.transform.position, transform.position);
                if(distance < 10000 && RayCastBullet(bullet)){
                    Debug.Log("DODGIN");
                    state = State.Dodge;
                    dodgeTarget = obj;
                    break;
                }
            }
        }
    }

    bool RayCastBullet(Bullet b){
        RaycastHit hitMid;
        RaycastHit hitRight;
        RaycastHit hitLeft;
        var collider = b.GetComponent<Collider>();
        //float speedCalc = b.velocity * Time.deltaTime * 100;
        float speedCalc = 10000;
        bool midHit = Physics.Raycast(b.transform.position, b.transform.TransformDirection(Vector3.forward), out hitMid, speedCalc);
        bool rightHit = Physics.Raycast(b.transform.position + b.transform.TransformDirection(Vector3.right) * collider.bounds.size.x, b.transform.TransformDirection(Vector3.forward), out hitRight, speedCalc);
        bool leftHit = Physics.Raycast(b.transform.position + b.transform.TransformDirection(Vector3.left) * collider.bounds.size.x, b.transform.TransformDirection(Vector3.forward), out hitLeft, speedCalc);
        if((midHit && hitMid.transform.gameObject == gameObject) ||(rightHit && hitRight.transform.gameObject == gameObject) || (leftHit &&  hitLeft.transform.gameObject == gameObject)){
            return true;
        }
        return false;
    }
    List<GameObject> OrderByClosest(List<GameObject> objs, Vector3 origin){
        return objs.OrderBy(x => Vector3.Distance(x.transform.position, origin)).ToList();
    }
}
