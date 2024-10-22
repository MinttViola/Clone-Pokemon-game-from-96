using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteAnimator 
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    public List<Sprite> Frames { get { return frames; } }
    float frameRate;
    bool oneAnimCycle;
    int currentFrame;
    float timer;
    bool stop = false;
    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate = 0.16f, bool oneAnimCycle = false)
    {
        this.frames = frames;
        this.spriteRenderer = spriteRenderer;  
        this.frameRate = frameRate;
        this.oneAnimCycle = oneAnimCycle;
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
        if (currentFrame == 0 && oneAnimCycle == true)
            currentFrame++;
        if (timer > frameRate&&stop == false)
            {
                currentFrame = (currentFrame + 1) % frames.Count;
                spriteRenderer.sprite = frames[currentFrame];
                timer -= frameRate;
            }
        if (currentFrame == 0 && oneAnimCycle == true)
        {
            stop = true;
            oneAnimCycle = false;
        }


    }
}
