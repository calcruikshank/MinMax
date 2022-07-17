using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnTerrainHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "object")
        {
            transform.parent.GetComponent<Bullet>().Die();
        }
    }
}
