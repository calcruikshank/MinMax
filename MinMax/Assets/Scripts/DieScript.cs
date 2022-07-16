using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieScript : MonoBehaviour
{
    public Rigidbody thisRB;
    public MeshRenderer thisMR;
    public DieRoller thisDR;
    public bool stopped = true;
    public DieCollider[] dieColliders;
    public GameObject starParticles;

    void Start()
    {
        stopped = true;
        Invoke("SetStoppedFalse", 0.1f);
    }

    void Update()
    {
        if (stopped) return;
        // Debug.Log(thisRB.velocity.magnitude);
        if (Mathf.Approximately(thisRB.velocity.magnitude, 0))
        {
            stopped = true;
            thisDR.valueText.text = CurrentValue().ToString();
            // thisRB.isKinematic = true;
            thisDR.SetPlayersMovement();
            StartCoroutine(thisDR.Timer());
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

    void SetStoppedFalse()
    {
        stopped = false;
    }

    void OnDisable()
    {
        thisDR.time = 0;
        thisDR.timerText.text = "";
        StopCoroutine(thisDR.Timer());
        Instantiate(starParticles, transform.position, Quaternion.identity);
    }
}
