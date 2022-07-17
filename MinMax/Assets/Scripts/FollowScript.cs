using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, followTarget.position, 50f * Time.deltaTime);
    }
}
