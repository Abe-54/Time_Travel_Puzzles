using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Image inventoryImage;
    public Image transitionOverlay;

    public TMP_Text interactText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowInteractText()
    {
        interactText.gameObject.SetActive(true);
    }

    public void HideInteractText()
    {
        interactText.gameObject.SetActive(false);
    }

    public void UpdateInventory(GameObject item)
    {
        Debug.Log("Picked up " + item.name);
        inventoryImage.sprite = item.GetComponent<SpriteRenderer>().sprite;
        inventoryImage.color = item.GetComponent<SpriteRenderer>().color;
        inventoryImage.rectTransform.localScale = new Vector3(item.transform.localScale.x, item.transform.localScale.y, item.transform.localScale.z);
    }

    public void ClearInventory()
    {
        inventoryImage.sprite = null;
        inventoryImage.color = new Color(0, 0, 0, 0);
    }

    public void TriggerTransition(float duration, Action onComplete)
    {
        transitionOverlay.gameObject.SetActive(true);
        transitionOverlay.DOFade(1, duration / 2).OnComplete(() =>
        {
            onComplete(); // Call the onComplete action which will swap environments, update positions, etc.
            transitionOverlay.DOFade(0, duration / 2).OnComplete(() =>
            {
                transitionOverlay.gameObject.SetActive(false);
            });
        });
    }
}
