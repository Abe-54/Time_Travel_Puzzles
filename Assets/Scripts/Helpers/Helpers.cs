using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static class Helpers
{
    public static float Map(float value, float originalMin, float originalMax, float newMin, float newMax, bool clamp)
    {
        float newValue = (value - originalMin) / (originalMax - originalMin) * (newMax - newMin) + newMin;
        if (clamp)
        {
            newValue = Mathf.Clamp(newValue, newMin, newMax);
        }
        return newValue;
    }

    public static Sequence Fade(Image image, float startAlpha, float endAlpha, float duration, bool disableOnComplete = false)
    {
        Sequence sequence = DOTween.Sequence();

        image.gameObject.SetActive(true);
        image.color = new Color(image.color.r, image.color.g, image.color.b, startAlpha);
        sequence.Append(image.DOFade(endAlpha, duration)).onComplete += () =>
        {
            if (disableOnComplete)
            {
                image.gameObject.SetActive(false);
            }
        };

        return sequence;
    }
}