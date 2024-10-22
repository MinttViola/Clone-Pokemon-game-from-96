using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator FadeAnim(float time, bool on)
    {
        if(on)
            yield return image.DOFade(1, time).WaitForCompletion();
        else
            yield return image.DOFade(0, time).WaitForCompletion();

    }
}
