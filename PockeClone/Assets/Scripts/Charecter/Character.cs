using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Character : MonoBehaviour
{
    public float moveSpeed;
     bool isMove;
    public bool IsMove { get; private set; }
    public float OffsetY { get; private set; } = 0.3f;

    CharacterAnimator anim;
    private void Awake()
    {
        anim = GetComponent<CharacterAnimator>();
        SetPositionAndSnapTile(transform.position);
    }

    public void SetPositionAndSnapTile(Vector2 pos)
    {

        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;

        transform.position = pos;
    }

    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {
        anim.moveX = Mathf.Clamp(moveVec.x, -1, 1);
        //anim.moveX = moveVec.normalized.x;
        anim.moveY = Mathf.Clamp(moveVec.y, -1, 1);
        //anim.moveY = moveVec.normalized.y;

        var targetPos = transform.position;
            targetPos.x += moveVec.x;
            targetPos.y += moveVec.y;

        if(!IsPathClear(targetPos))
            yield break;

        IsMove = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        IsMove = false;
        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        anim.IsMove = IsMove;
    }
    
    public bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;
        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.i.SolidobjMask | GameLayers.i.InteractLayer|GameLayers.i.PlayerLayer))
            return false;
        return true;
    }
    
    public void LookTowards(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {
            anim.moveY = ydiff;
            anim.moveX = xdiff;

        }
        else Debug.LogError("Cant ask Character look diogonaly");
    }

    private bool IsWalkible(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidobjMask | GameLayers.i.InteractLayer) != null)
        {
            return false;
        }
        return true;
    }

    public CharacterAnimator Anim { get { return anim; } }
}
