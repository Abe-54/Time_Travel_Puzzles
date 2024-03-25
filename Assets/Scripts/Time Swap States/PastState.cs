using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PastState : TimeSwapBaseState
{
    public GameObject[] pastObjects;
    public Sprite pastBackground;
    public AudioSource pastAudioSource;

    public override void EnterState()
    {
        backgroundSprite.sprite = pastBackground;

        if (Application.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            pastAudioSource.Play();
            pastAudioSource.DOFade(gameManager.settings.volume, 1.0f);
        }

        foreach (GameObject gameObject in pastObjects)
        {
            gameObject.SetActive(true);
        }
    }

    public override void Do()
    {
    }

    public override void ExitState()
    {
        if (Application.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) pastAudioSource.DOFade(0, 1.0f).onComplete += () => pastAudioSource.Pause();

        foreach (GameObject gameObject in pastObjects)
        {
            gameObject.SetActive(false);
        }
    }
}
