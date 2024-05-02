using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class JumpPadLogic : MonoBehaviour
{
    public float jumpForce = 10.0f;
    public Sprite defaultSprite;
    public Sprite pressedSprite;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer.sprite = defaultSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(other.GetComponent<Rigidbody2D>().velocity.x, jumpForce);
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                spriteRenderer.sprite = pressedSprite;
            });
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            audioSource.Play();
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(0.75f);
            sequence.AppendCallback(() =>
            {
                spriteRenderer.sprite = defaultSprite;
            });
        }
    }
}
