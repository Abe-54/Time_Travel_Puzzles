using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TreeController : MonoBehaviour
{
    public GameObject tree;
    public GameObject knockedTree;

    public bool isKnocked;

    private AudioSource audioSource;

    public bool isGrown;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        tree = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.CompareTo("Axe") == 0)
        {
            Rigidbody2D axeBody = other.GetComponent<Rigidbody2D>();
            axeBody.bodyType = RigidbodyType2D.Static;
            axeBody.gravityScale = 0;
            axeBody.constraints = RigidbodyConstraints2D.FreezeAll;
            axeBody.velocity = Vector2.zero;
            axeBody.transform.SetParent(tree.transform);


            // audioSource.volume = FindObjectOfType<GameManager>().settings.volume;

            audioSource.PlayOneShot(audioSource.clip);

            Sequence treeFallingSequence = DOTween.Sequence();

            treeFallingSequence
                .Append(tree.transform.DOMove(knockedTree.transform.position, 2f))
                .Insert(0, tree.transform.DORotateQuaternion(knockedTree.transform.rotation, treeFallingSequence.Duration()))
                .OnComplete(() =>
                {
                    isKnocked = true;

                    other.isTrigger = true;
                    tree.GetComponent<Collider2D>().isTrigger = false;
                });
        }
    }
}
