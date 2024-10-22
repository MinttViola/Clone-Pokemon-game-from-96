using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimatorImage 
{
    Image spriteRenderer;
    List<Sprite> frames;
    public List<Sprite> Frames { get { return frames; } }
    float frameRate;

    int currentFrame;
    float timer;
    public SpriteAnimatorImage(List<Sprite> frames, Image spriteRenderer, float frameRate = 0.16f)
    {
        this.frames = frames;
        this.spriteRenderer = spriteRenderer;  
        this.frameRate = frameRate;
    }

    public void Start()
    {
        currentFrame = 0;
        timer = 0;
        spriteRenderer.sprite = frames[0];
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }
}
