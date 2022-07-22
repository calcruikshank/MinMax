using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DestroyWorldCanvas : MonoBehaviour
{
    [SerializeField] CanvasGroup cg;
    public TMP_Text oofText;
    void Start()
    {
        cg.alpha = 1.0f;
        transform.DOMove(new Vector3(transform.position.x, transform.position.y+1, transform.position.z), 1.9f);
        cg.DOFade(0, 2).OnComplete(() => {
            Destroy(gameObject);
        });
    }
}
