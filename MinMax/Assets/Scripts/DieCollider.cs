using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieCollider : MonoBehaviour
{
    public int value;
    public bool isContacted;
    List<Collider> collided = new List<Collider>();

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer != LayerMask.NameToLayer("Floor")) return;
        isContacted = true;
        if (!collided.Contains(collider)) collided.Add(collider);
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer != LayerMask.NameToLayer("Floor")) return;
        if (collided.Contains(collider)) collided.Remove(collider);
        if (collided.Count < 1) isContacted = false;
    }
}
