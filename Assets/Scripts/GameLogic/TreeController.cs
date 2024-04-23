using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TreeController : MonoBehaviour
{
    public GameObject topTree;
    public Transform pivotTree;

    public float zRotation = 0f;
    public float rotationSpeed = 1f;

    public bool isKnocked;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.name.Contains("Axe") && !isKnocked)
        {
            Sequence treeFallingSequence = DOTween.Sequence();

            treeFallingSequence.Append(pivotTree.DORotate(new Vector3(0, 0, zRotation), rotationSpeed, RotateMode.FastBeyond360))
                .OnComplete(() =>
                {
                    isKnocked = true;
                    topTree.layer = LayerMask.NameToLayer("Ground");
                    topTree.GetComponent<Collider2D>().isTrigger = false;
                    gameObject.GetComponent<Collider2D>().isTrigger = true;
                });
        }
    }
}
