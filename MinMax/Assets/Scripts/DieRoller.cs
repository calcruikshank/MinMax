using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DieRoller : MonoBehaviour
{
    public GameObject diePrefab;
    public Transform placeDiceHere;
    public DieScript currentDie;
    public TMP_Text valueText;
    public Material unusedDie, usedDie;

    public void Button_RollDie()
    {
        GameObject rolledDie = Instantiate(diePrefab, placeDiceHere.position, Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
        currentDie = rolledDie.GetComponent<DieScript>();
        currentDie.thisRB.AddForce(Vector3.right * Random.Range(100, 300));
        currentDie.thisDR = this;
        currentDie.stopped = false;
        currentDie.thisRB.isKinematic = false;
    }

}
