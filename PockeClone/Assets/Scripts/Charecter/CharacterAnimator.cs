using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAnimator : MonoBehaviour
{
    public float moveX { get; set; }
    public float moveY { get; set; }
    public bool IsMove { get; set; }

    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] int StayFrame = 0;
    [SerializeField] FaceDirection defultDeraction = FaceDirection.Down;

    SpriteAnimator currentAnim;
    bool wasPrevMoving;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer, 0.15f);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer, 0.15f);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer, 0.15f);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer, 0.15f);
        SetFaceDirection(defultDeraction);

        switch (defultDeraction) 
        { 
            case FaceDirection.Down:
                currentAnim = walkDownAnim;
                break;
            case FaceDirection.Up:
                currentAnim = walkUpAnim;
                break;
            case FaceDirection.Left:
                currentAnim = walkLeftAnim;
                break;
            case FaceDirection.Right:
                currentAnim = walkRightAnim;
                break;

        }
    }

    private void Update()
    {
        var prevAnim = currentAnim;


        if (currentAnim != prevAnim || IsMove != wasPrevMoving)
            currentAnim.Start();

        if (moveX > 0)
            currentAnim = walkRightAnim;
        else if (moveX < 0 )
            currentAnim = walkLeftAnim;
        else if (moveY > 0)
            currentAnim = walkUpAnim;
        else if (moveY < 0)
            currentAnim = walkDownAnim;


        if (currentAnim != prevAnim || IsMove != wasPrevMoving)
            currentAnim.Start();
            
        if (IsMove )
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[StayFrame];



        wasPrevMoving = IsMove;
    }

    public void SetFaceDirection(FaceDirection direction)
    {
        switch (direction) {
            case FaceDirection.Left:
                moveX = -1;
                break;
            case FaceDirection.Right:
                moveX = 1;
                break;
            case FaceDirection.Up:
                moveY = 1;
                break;
            case FaceDirection.Down:
                moveY = -1;
                break;
                }
    }

    public FaceDirection Direction
    {
        get { return defultDeraction; }
    }
}

public enum FaceDirection { Up,Down,Left,Right}