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
    public Image watchImage;

    public TMP_Text helpText;

    public bool isTransitioning = false;

    // Start is called before the first frame update
    void Start()
    {
        Helpers.Fade(transitionOverlay, 1f, 0f, 1f, true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowHelpText(String textToShow)
    {
        helpText.text = textToShow;
        helpText.gameObject.SetActive(true);
    }

    public void HideHelpText()
    {
        helpText.gameObject.SetActive(false);
    }

    public void UpdateInventory(GameObject item)
    {
        Debug.Log("Picked up " + item.name);
        inventoryImage.sprite = item.GetComponent<SpriteRenderer>().sprite;
        inventoryImage.color = item.GetComponent<SpriteRenderer>().color;
        inventoryImage.SetNativeSize();

        // Define the max size for the image to fit into the slot
        Vector2 maxSize = new Vector2(40, 40); // Example slot size, adjust as needed
        float margin = 0.95f; // Make the image slightly smaller than the slot

        // Calculate the scaling factor needed to ensure the image fits within the maxSize bounds
        float widthScale = maxSize.x / inventoryImage.rectTransform.sizeDelta.x;
        float heightScale = maxSize.y / inventoryImage.rectTransform.sizeDelta.y;
        float scale = Mathf.Min(widthScale, heightScale, 1) * margin; // Ensure we don't scale up, only down

        // Apply the calculated scale
        inventoryImage.rectTransform.localScale = new Vector3(scale, scale, 1);
    }

    public void ClearInventory()
    {
        inventoryImage.sprite = null;
        inventoryImage.color = new Color(0, 0, 0, 0);
    }

    public void TriggerTransition(float duration, Action onComplete)
    {
        // Debug.Log("Triggering transition");

        // isTransitioning = true;

        // transitionOverlay.gameObject.SetActive(true);

        // Helpers.Fade(transitionOverlay, 0f, 1.0f, duration).OnComplete(() =>
        // {
        //     onComplete(); // Call the onComplete action which will swap environments, update positions, etc.

        //     transitionOverlay.DOFade(0, duration / 2).OnComplete(() =>
        //     {
        //         transitionOverlay.gameObject.SetActive(false);
        //     });

        //     isTransitioning = false;
        // });

        Debug.LogWarning("OLD IMPLEMENTATION");
    }

}
