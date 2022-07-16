using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieScript : MonoBehaviour
{
    public int currentValue;
    public Rigidbody thisRB;
    public MeshRenderer thisMR;
    public DieRoller thisDR;

    // void Start()
    // {
    //     StartCoroutine(CheckValue());
    // }

    void Update()
    {
        Debug.Log(thisRB.velocity.magnitude);
    }

    public IEnumerator CheckValue()
    {
        while(thisRB.velocity.magnitude > 0) yield return new WaitForEndOfFrame();
        thisDR.valueText.text = "1";
    }
}
