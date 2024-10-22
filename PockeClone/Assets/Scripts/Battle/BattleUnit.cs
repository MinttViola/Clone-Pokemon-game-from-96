using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{

    [SerializeField] bool isPlayer;
    [SerializeField] BattleHub hub;
    //заполнение спрайта покемона
    public bool IsPlayer { get { return isPlayer; } }
    public Pokemon Pokemon { get; set; }
    public BattleHub Hub { get { return hub; } }

    Image image;
    Vector3 originalPos;
    Color origColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        origColor = image.color;
    }
    public void Setup(Pokemon poke)
    {
        Pokemon = poke;
        if (isPlayer)
            image.sprite = Pokemon.Base.BackSprite;
        else
            image.sprite = Pokemon.Base.FrontSprite;
        PlayEnterAnimation();

        transform.localScale = new Vector3(1, 1, 1);
        hub.SetData(poke);
        image.color = origColor;
    }

    public void PlayEnterAnimation()
    {
        if (isPlayer)
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPos.y);

        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void AttackAnim()
    {
        var sequence = DOTween.Sequence();
        if (isPlayer)
            sequence.Append( image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void HitAnim()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(origColor, 0.1f));
    }

    public void FaintAnim()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        yield return sequence.Append(image.DOFade(1, 0.5f));
        yield return sequence.Join(transform.DOLocalMoveY(originalPos.y , 0.5f));
        yield return sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
}
