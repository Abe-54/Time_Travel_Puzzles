using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

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

        uiManager.TriggerTransition(1.0f, () =>
        {
            SwapState();

            OnTimePeriodChanged?.Invoke();

            player.machine.Set(player.idleState);
        });
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
                //TODO: ADD TIME TRAVEL UI ANIMATION using dotween

                /* STEPS:
                    1. Spawn a new ui image at the player watch position
                    2. Scale the image up to fill the screen while also playing the watch animation
                    3. after the animation is done, swap the state
                    4. scale the image back down to 0
                    5. destroy the image
                */

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