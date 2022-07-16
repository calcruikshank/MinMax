using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieScript : MonoBehaviour
{
    public int currentValue;
    public Rigidbody thisRB;
    public MeshRenderer thisMR;
    public DieRoller thisDR;
    public bool stopped = true;
    public DieCollider[] dieColliders;

    void OnCollisionEnter()
    {
        stopped = false;
    }

    void Update()
    {
        if (stopped) return;
        Debug.Log(thisRB.velocity.magnitude);
        if (Mathf.Approximately(thisRB.velocity.magnitude, 0))
        {
            stopped = true;
            thisDR.valueText.text = GetValue().ToString();
        }
    }

    int GetValue()
    {
        foreach(DieCollider dc in dieColliders)
        {
            if (dc.isContacted) return dc.value;
        }
        return 0;
    }
}
