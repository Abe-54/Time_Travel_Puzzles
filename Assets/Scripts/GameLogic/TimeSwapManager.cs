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
    public Sprite presentBackground;

    [Header("Past State")]
    public GameObject[] pastObjects;

    public Sprite pastBackground;

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

#if UNITY_EDITOR
    public void SwapTimePeriodEditorOnly()
    {
        switch (currentTimePeriod)
        {
            case TimePeriod.Present:
                currentTimePeriod = TimePeriod.Past;
                EnterState(pastObjects, pastBackground, presentObjects);
                break;
            case TimePeriod.Past:
                currentTimePeriod = TimePeriod.Present;
                EnterState(presentObjects, presentBackground, pastObjects);
                break;
        }
    }
#endif

    public void SwapState()
    {
        switch (currentTimePeriod)
        {
            case TimePeriod.Present:
                currentTimePeriod = TimePeriod.Past;
                Debug.Log("Swapping to Past: " + currentTimePeriod);
                FadeOutAudio(pastAudioSource, presentAudioSource);
                EnterState(pastObjects, pastBackground, presentObjects);

                break;
            case TimePeriod.Past:
                currentTimePeriod = TimePeriod.Present;
                Debug.Log("Swapping to Present: " + currentTimePeriod);
                FadeOutAudio(presentAudioSource, pastAudioSource);
                EnterState(presentObjects, presentBackground, pastObjects);
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

    private void EnterState(GameObject[] objects, Sprite background, GameObject[] objectsToDeactivate)
    {
        Debug.Log("Entering State");

        // backgroundSprite.sprite = background;

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