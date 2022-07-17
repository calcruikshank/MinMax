using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Jeffery : MonoBehaviour
{
    PlayerController controller;
    PlayerController target = null;
    GameObject dodgeTarget = null;
    GameObject grabTarget = null;
    Vector2 moveDirection;
    float keepDistance = 15f;
    float keepRange = 5f;
    float keepFuzz = 2f;
    bool isFiring;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<PlayerController>();
        state = State.Chasing;
        InvokeRepeating("SelectTarget", .5f, 1f);
        InvokeRepeating("Fuzz", 2.0f, 2f);
        InvokeRepeating("Face", 10f, 10f);
        InvokeRepeating("LookForPower", 1f, 1f);
        keepDistance = controller.stats.ProjectileRange * .75f;
        keepRange = controller.stats.ProjectileRange * .10f;
    }
    private enum State
    {
        Chasing,
        Dodge,
        Grabbing
    }
    State state;
    // Update is called once per frame
    void Update()
    {
        float speed = 1f;
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
                        var strafe = Quaternion.AngleAxis(90, Vector3.left)*dir;
                        moveDirection = new Vector2(strafe.x,strafe.z);
                        Fire();
                    }
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
            case State.Grabbing:
                if(grabTarget != null && Vector3.Distance(grabTarget.transform.position,transform.position)<=60){

                    var dir = grabTarget.transform.position - transform.position;
                    moveDirection = new Vector2(dir.x,dir.z); 
                }else{
                    state = State.Chasing;
                }
                break;

        }
        speed = 1f;
        moveDirection.Normalize();
        controller.inputMovement = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }
    void SelectTarget(){
        var listOfClosest = OrderByClosest(GameManager.g.Players,transform.position);
        foreach(var obj in listOfClosest){
            if(obj != gameObject){
                if(target != obj)
                    StopFire();
                    target = obj.GetComponent<PlayerController>();
                    break;                    
            }
        }
    }

    void Fuzz(){
        keepFuzz = Random.Range(-1,1);
    }

    void Face(){
        controller.PickRandomFace();
    }

    void LookForPower(){
        var listOfClosest = OrderByClosest(GameManager.g.Powers,transform.position);
        foreach(var obj in listOfClosest){
            if(Vector3.Distance(obj.transform.position,transform.position)>60 || obj.transform.position.y<.2 || obj.transform.position.y>1){
                break;
            }
            grabTarget = obj;
            StopFire();
            state = State.Grabbing;
            break;
        }
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
