using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class TimeSwapManager : MonoBehaviour
{
    public enum TimePeriod
    {
        Present,
        Past
    }

    public TimePeriod currentTimePeriod { get; private set; }


    [Header("Present State")]
    public GameObject[] presentObjects;
    public AudioSource presentAudioSource;

    [Header("Past State")]
    public GameObject[] pastObjects;
    public AudioSource pastAudioSource;

    [Header("Other References")]
    // public SpriteRenderer backgroundSprite;
    public float maxWatchScale = 2.5f;
    public SettingsSO settings;
    public event Action OnTimePeriodChanged;

    private PlayerMovement player;
    private UIManager uiManager;

    public float originalGravity;

    void Awake()
    {
        currentTimePeriod = TimePeriod.Present; // Default start time period
        player = FindObjectOfType<PlayerMovement>();
        uiManager = FindObjectOfType<UIManager>();
        originalGravity = player.GetComponent<Rigidbody2D>().gravityScale;
    }

    public void SwapTimePeriod()
    {
        if (player.machine.state == player.timeSwapState || uiManager.isTransitioning)
        {
            return;
        }
        player.machine.Set(player.timeSwapState);

        //TODO: ADD TIME TRAVEL UI ANIMATION using dotween

        /* STEPS:
            1. Spawn a new ui image at the player watch position
            2. Scale the image up to fill the screen while also playing the watch animation
            3. after the animation is done, swap the state
            4. scale the image back down to 0
            5. destroy the image
        */

        // Convert the watch position to screen space
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(player.timeSwapState.watchPosition.position);

        Image watchImage = Instantiate(uiManager.watchImage, screenPosition, Quaternion.identity, uiManager.canvas.transform);
        watchImage.transform.localScale = Vector3.one * 0.1f;

        Sequence sequence = DOTween.Sequence();
        // spawn the watch image, scale it up and play the animation, while also shaking the image and fade to black with the watch image on top
        sequence.Append(watchImage.transform.DOMove(new Vector2(Screen.width / 2, Screen.height / 2), 1.0f).SetEase(Ease.InOutQuint));
        sequence.Join(watchImage.transform.DOScale(maxWatchScale, 1.0f));
        watchImage.GetComponent<Animator>().Play(currentTimePeriod == TimePeriod.Present ? "WatchToPast" : "WatchToPresent");
        sequence.Join(uiManager.transitionOverlay.DOFade(1.0f, 1.0f)).onComplete += () =>
        {
            SwapState();
        };
        sequence.Join(watchImage.DOFade(1.0f, 1.0f));
        sequence.onComplete += () =>
        {
            // On Complete of the animation, swap the state, scale the image back down, fade in to gameplay, and destroy the watch image
            Sequence completeSequence = DOTween.Sequence();
            completeSequence.Append(watchImage.transform.DOScale(0, 1.0f));

            Vector2 newScreenPosition = Camera.main.WorldToScreenPoint(player.timeSwapState.watchPosition.position);

            completeSequence.Join(watchImage.transform.DOMove(newScreenPosition, 1.0f).SetEase(Ease.InOutQuint));
            completeSequence.Join(uiManager.transitionOverlay.DOFade(0, 1.0f));
            completeSequence.Join(watchImage.DOFade(0, 1.0f));
            completeSequence.onComplete += () =>
            {
                Destroy(watchImage.gameObject);
                OnTimePeriodChanged?.Invoke();
                player.machine.Set(player.idleState);
            };
        };




        // uiManager.TriggerTransition(1.0f, () =>
        // {
        //     SwapState();

        //     OnTimePeriodChanged?.Invoke();

        //     player.machine.Set(player.idleState);
        // });
    }

    public void SwapTimePeriodEditorOnly()
    {
        switch (currentTimePeriod)
        {
            case TimePeriod.Present:
                currentTimePeriod = TimePeriod.Past;
                EnterState(pastObjects, presentObjects);
                break;
            case TimePeriod.Past:
                currentTimePeriod = TimePeriod.Present;
                EnterState(presentObjects, pastObjects);
                break;
        }
    }

    public void SwapState()
    {
        switch (currentTimePeriod)
        {
            case TimePeriod.Present:
                currentTimePeriod = TimePeriod.Past;
                Debug.Log("Swapping to Past: " + currentTimePeriod);
                FadeOutAudio(pastAudioSource, presentAudioSource);
                EnterState(pastObjects, presentObjects);


                break;
            case TimePeriod.Past:
                currentTimePeriod = TimePeriod.Present;
                Debug.Log("Swapping to Present: " + currentTimePeriod);
                FadeOutAudio(presentAudioSource, pastAudioSource);
                EnterState(presentObjects, pastObjects);
                //TODO: ADD TIME TRAVEL UI ANIMATION using dotween

                break;
        }
    }

    private void FadeOutAudio(AudioSource currentAudioSource, AudioSource newAudioSource)
    {
        currentAudioSource.DOFade(0, 1.0f).onComplete += () =>
        {
            currentAudioSource.Pause();
            newAudioSource.Play();
            newAudioSource.DOFade(settings.volume, 1.0f);
        };
    }

    private void EnterState(GameObject[] objects, GameObject[] objectsToDeactivate)
    {
        Debug.Log("Entering State");

        foreach (GameObject obj in objects)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }
    }
}