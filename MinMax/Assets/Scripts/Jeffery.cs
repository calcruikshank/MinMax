using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Jeffery : MonoBehaviour
{
    PlayerController controller;
    PlayerController target = null;
    GameObject dodgeTarget = null;
    GameObject grabTarget = null;
    Vector3 moveTarget;
    Vector3 pathTarget;
    Vector2 moveDirection;
    float keepDistance = 15f;
    float keepRange = 5f;
    float keepFuzz = 2f;
    float moveFuzzX;
    float moveFuzzZ;
    float dispelCoolDown;
    bool isFiring;

    private Vector3 lastPos = new Vector3();
    private float distMoved = 0;
    private float timeStamp = 0;
    NavMeshPath path;
    int pathIndex = 0;

    void Start()
    {
        controller = gameObject.GetComponent<PlayerController>();
        state = State.Chasing;
        InvokeRepeating("SelectTarget", Random.Range(.3f,.7f), 1f);
        InvokeRepeating("Fuzz", Random.Range(.3f,.7f), Random.Range(2f,3f));
        InvokeRepeating("Face", Random.Range(1f,10f), Random.Range(11f,20f));
        InvokeRepeating("LookForPower", Random.Range(2f,.4f), Random.Range(1f,3f));
        InvokeRepeating("CalculatePath", 0f, .25f);
        //InvokeRepeating("FindDirection", 1f, 1f);
        keepDistance = controller.stats.ProjectileRange * .75f;
        keepRange = controller.stats.ProjectileRange * .10f;
        path = new NavMeshPath();
    }
    private enum State
    {
        Chasing,
        Dodge,
        Grabbing,
        Goto
    }
    int move = 0;
    State state;
    // Update is called once per frame
    void Update()
    {
        if(timeStamp == 0){
            timeStamp = Time.time;
        }
        else{
            distMoved += Vector3.Distance(lastPos, transform.position);
            lastPos = transform.position;
        }

        if(Time.time >= timeStamp + 1f){
            if(state == State.Goto){
                state = State.Chasing;
            }
            //We haven't moved in awhile
            if(distMoved < 2f){
                move = 0;
                FindDirection();
            }
            distMoved = 0;
            timeStamp = Time.time;
        }

        float speed = 1f;
        LookAt();
        CheckDodge();
        Vector3 dir;
        switch(state){
            case State.Chasing:
                if(target != null){
                    var distance = Vector3.Distance(target.transform.position, transform.position);
                    dir = target.transform.position - transform.position;

                    if(distance > keepDistance + (keepRange + keepFuzz)){
                        pathTarget = target.transform.position;
                        dir = GetNextPathDir();
                        moveDirection = new Vector2(dir.x,dir.z);
                        StopFire();
                    }else if(distance < keepDistance - (keepRange + keepFuzz)){
                        moveDirection = new Vector2(dir.x*-1,dir.z*-1);
                        Fire();     
                    }else{
                        moveDirection = new Vector2(0,0);
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
                    
                    if(Time.time >= dispelCoolDown){
                        if(leftDistance<15f || rightDistance<15f){
                            Dispell();
                            dispelCoolDown = Time.time + 10f;
                        }else{
                            StopDispell();
                        }
                    }else{
                        StopDispell();
                    }
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
                        StopDispell();
                    }
                }else{
                    state = State.Chasing;
                    StopDispell();
                }
                break;
            case State.Grabbing:
                if(grabTarget != null && Vector3.Distance(grabTarget.transform.position,transform.position)<=60 && !(grabTarget.transform.position.y<.2) && !(grabTarget.transform.position.y>.5)){
                    dir = GetNextPathDir();
                    moveDirection = new Vector2(dir.x,dir.z);
                }else{
                    state = State.Chasing;
                }
                break;
            case State.Goto:
                
                if(Vector3.Distance(moveTarget, transform.position) <= 2f){
                    moveDirection = new Vector2(0,0);
                    if(move == 0){
                        move++;
                        FindDirection();
                    }
                    else{
                        state = State.Chasing;
                    }
                }
                break;
        }
        speed = 1f;
        //moveDirection.x = moveDirection.x + moveFuzzX;
        //moveDirection.y = moveDirection.y + moveFuzzZ;
        moveDirection.Normalize();
        controller.inputMovement = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
        DebugPath();
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

    bool CalculatePath(){
        pathIndex = 0;
        
        return NavMesh.CalculatePath(transform.position, pathTarget, NavMesh.AllAreas, path);
    }

    void DebugPath(){
        if(path!=null && path.corners.Length>0){
            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
    }
    Vector3 GetNextPathDir(){
        if(Vector3.Distance(transform.position, path.corners[pathIndex])<1f){
            pathIndex++;
        }
        return path.corners[pathIndex] - transform.position;
    }
    
    void Fuzz(){
        keepFuzz = Random.Range(-1f,1f);
        moveFuzzX = Random.Range(-1f,1f);
        moveFuzzZ = Random.Range(-1f,1f);
    }

    void Face(){
        controller.PickRandomFace();
    }

    void LookForPower(){
        var listOfClosest = OrderByClosest(GameManager.g.Powers,transform.position);
        foreach(var obj in listOfClosest){
            if(Vector3.Distance(obj.transform.position,transform.position)>60){
                break;
            }
            
            grabTarget = obj;
            pathTarget = obj.transform.position;
            if(CalculatePath()){
                StopFire();
                state = State.Grabbing;
            }
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
            controller.lookDirection = new Vector2(dir.x,dir.z);
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
    bool isDispelling = false;
    void Dispell(){
        if(state == State.Dodge){
            if(!isDispelling){
                controller.dispelDownPressed = true;
                isDispelling = true;
            }
        }
    }

    void StopDispell(){
        if(isDispelling){
            controller.dispelDownPressed = false;
            isDispelling = false;
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
    void FindDirection(){
        RaycastHit hit;
        state = State.Chasing;
        for(int i = 0; i < 20; i++){
            float angle = Random.Range(0f,360f);
            Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            if(!Physics.Raycast(transform.position, dir, out hit, 7)){
                moveTarget = transform.position + dir * 7;
                state = State.Goto;
                moveDirection = moveTarget - transform.position;
                break;
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
