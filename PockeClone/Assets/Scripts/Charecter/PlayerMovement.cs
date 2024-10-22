
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, ISavable
{

    private Vector2 input;
    private Character character;

    [SerializeField] Sprite sprite;
    [SerializeField] string name;
    public string Name { get { return name; } }

    public Sprite Sprite { get { return sprite; } }

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    public void HundleUpdate()
    {
        //движение по осям х у
        if (!character.IsMove)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                StartCoroutine( character.Move(input, OnMoveOver)); 
            }
        }

        character.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    public void Interact()
    {
        var faceDir = new Vector3(character.Anim.moveX,character.Anim.moveY);
        var interactPos = transform.position + faceDir;
        
        var collider = Physics2D.OverlapCircle(interactPos, 0.5f, GameLayers.i.InteractLayer);
        if(collider != null) 
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    public void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY, 0), 0.2f, GameLayers.i.TriggerableLayers);

        foreach(var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable != null)
            {
                character.Anim.IsMove = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    public object CaptureState()
    {
        float[] position = new float[] { transform.position.x, transform.position.y };
        return position;
    }

    public void RestoreState(object state)
    {
        var position = (float[])state;
        transform.position = new Vector3(position[0], position[1]);
    }

    public Character Character => character;
}


