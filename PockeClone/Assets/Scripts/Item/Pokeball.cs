using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Pokeball : MonoBehaviour
{
    [SerializeField] List<Sprite> pokeballSprite;
    [SerializeField] float frameRate = 0.16f;
    SpriteRenderer spriteRenderer;
    SpriteAnimator anim;
    bool startAnim = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = new SpriteAnimator(pokeballSprite, spriteRenderer, frameRate, true);
        anim.Start();
    }
    public IEnumerator AnimationOpen()
    {
        startAnim = true;
        yield return null;
    }

    private void Update()
    {
        if(startAnim = true)
            anim.HandleUpdate();
        
    }

}
