using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieScript : MonoBehaviour
{
    public Rigidbody thisRB;
    public MeshRenderer thisMR;
    public DieRoller thisDR;
    public bool stopped;
    public DieCollider[] dieColliders;

    void Update()
    {
        if (stopped) return;
        Debug.Log(thisRB.velocity.magnitude);
        if (Mathf.Approximately(thisRB.velocity.magnitude, 0))
        {
            stopped = true;
            thisDR.valueText.text = CurrentValue().ToString();
            thisRB.isKinematic = true;
        }
    }

    public int CurrentValue()
    {
        foreach(DieCollider dc in dieColliders)
        {
            if (dc.isContacted) return dc.value;
        }
        return 0;
    }

    void OnCollisionEnter()
    {
        stopped = false;
    }
}
