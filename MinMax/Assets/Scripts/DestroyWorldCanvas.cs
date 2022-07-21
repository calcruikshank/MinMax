using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DestroyWorldCanvas : MonoBehaviour
{
    [SerializeField] CanvasGroup cg;
    void Start()
    {
        cg.alpha = 1.0f;
        transform.DOMove(new Vector3(transform.position.x, transform.position.y+2, transform.position.z), 1.9f);
        cg.DOFade(0, 2).OnComplete(() => {
            Destroy(gameObject);
        });
    }
}
