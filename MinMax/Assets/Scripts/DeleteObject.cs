using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    public void DestroyObject()
    {
        StartCoroutine("Die");
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(.05f);
        Destroy(this.gameObject);
    }
}
