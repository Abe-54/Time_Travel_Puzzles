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
    public List<GameObject> presentObjects;
    public AudioSource presentAudioSource;

    [Header("Past State")]
    public List<GameObject> pastObjects;
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

        DoWatchSequence();
    }

    private void DoWatchSequence(float duration = 1.0f)
    {
        // Convert the watch position to screen space
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(player.timeSwapState.watchPosition.position);

        Image watchImage = Instantiate(uiManager.watchImage, screenPosition, Quaternion.identity, uiManager.transform.parent);
        watchImage.transform.localScale = Vector3.one * 0.1f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(watchImage.transform.DOMove(new Vector2(Screen.width / 2, Screen.height / 2), duration).SetEase(Ease.InOutQuint));
        sequence.Join(watchImage.transform.DOScale(maxWatchScale, duration));

        string animationName = currentTimePeriod == TimePeriod.Present ? "WatchToPast" : "WatchToPresent";
        watchImage.GetComponent<Animator>().Play(animationName);

        sequence.Join(Helpers.Fade(uiManager.transitionOverlay, 0.0f, 1.0f, duration))
            .onComplete += () =>
                {
                    SwapState();
                    OnTimePeriodChanged?.Invoke();
                };
        sequence.Join(Helpers.Fade(watchImage, 0.0f, 1.0f, duration));

        // End of Sequence, back to gameplay
        sequence.onComplete += () =>
        {
            Sequence completeSequence = DOTween.Sequence();
            completeSequence.Append(watchImage.transform.DOScale(0, duration));

            Vector2 newScreenPosition = Camera.main.WorldToScreenPoint(player.timeSwapState.watchPosition.position);

            completeSequence.Join(watchImage.transform.DOMove(newScreenPosition, duration).SetEase(Ease.InOutQuint));
            completeSequence.Join(Helpers.Fade(uiManager.transitionOverlay, 1.0f, 0f, duration, true));
            completeSequence.Join(Helpers.Fade(watchImage, 1.0f, 0f, duration, true));
            completeSequence.onComplete += () =>
            {
                Destroy(watchImage.gameObject);
                player.machine.Set(player.idleState);
            };
        };
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

    private void EnterState(List<GameObject> objects, List<GameObject> objectsToDeactivate)
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