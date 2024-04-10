using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateLogic : MonoBehaviour, IInteractable
{
    public GameObject itemPrefab;
    public Transform itemSpawnPosition;
    public SpriteRenderer displaySprite;
    public AnimationClip openAnimation;
    private bool isItemSpawned = false;
    private GameObject spawnedItem = null;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public void Interact(PlayerMovement player)
    {
        if (!isItemSpawned && itemPrefab != null)
        {
            isItemSpawned = true;
            displaySprite.color = new Color(1, 1, 1, 0.25f);
            animator.Play(openAnimation.name);
            spawnedItem = Instantiate(itemPrefab, itemSpawnPosition.position, Quaternion.identity);
        }
    }

    void Update()
    {
        if (isItemSpawned && spawnedItem == null)
        {
            isItemSpawned = false;
            // Additional logic for re-enabling the crate visual cue or interaction capability
        }
    }
}
