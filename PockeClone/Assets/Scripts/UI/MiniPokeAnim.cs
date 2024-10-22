using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MiniPoke : MonoBehaviour

{
    Image image;
    List<Sprite> frames;
    [SerializeField]float frameRate = 0.7f;
    Image spriteRenderer;
    SpriteAnimatorImage anim;
    bool animStart = false;
    bool animStart1 = false;
    bool animStart2 = false;

    public void Set(Pokemon pokemon)
    {
        frames = pokemon.Base.MiniPokeAnim;
        animStart1 = true;
        StartAnimation();
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<Image>();
        animStart2 = true;
    }
    void StartAnimation()
    {
        if (animStart1 && animStart2)
        {
            anim = new SpriteAnimatorImage(frames, spriteRenderer, frameRate);
            anim.Start();
            animStart = true;
        }
    }

    public void Update()
    {
        if(animStart)
        anim.HandleUpdate();
    }
}
