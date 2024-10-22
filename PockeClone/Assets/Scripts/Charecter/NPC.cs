
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movePattern;
    [SerializeField] float time;

    NPCState state;
    float idleTimer = 0;
    int currentPattern = 0;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }
    public void Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            character.LookTowards(initiator.position);
            state = NPCState.Dialog;
            StartCoroutine(DialogManagerScript.Instance.ShowDialog(dialog, () => { state = NPCState.Idle; idleTimer = 0; }));
        }
    }

    private void Update()
    {
        if(state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > time) 
            {
                idleTimer = 0;
                if (movePattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();
    }
    IEnumerator Walk()
    {
        state = NPCState.Walking;
        var oldPosition = transform.position;
        yield return character.Move(movePattern[currentPattern]);
        if(transform.position != oldPosition)
           currentPattern = (currentPattern + 1)% movePattern.Count;
        state = NPCState.Idle;
    }
}

public enum NPCState
{
    Idle,
    Walking,
    Dialog
}
