using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float velocity;
    public Vector3 targetPosition;

    private Vector3 startPosition;
    private float distMoved = 0;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
    }

    
    // Update is called once per frame
    void Update()
    {
        distMoved += Time.deltaTime * velocity ;
        float percentMoved = distMoved/Vector3.Distance(targetPosition,startPosition);
        transform.localPosition = Vector3.Lerp(startPosition, targetPosition, percentMoved);

        if(transform.localPosition == targetPosition){
            Destroy(this.gameObject);
        }
    }
}
