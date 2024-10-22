using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectLoader : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectPrehab;
    private void Awake()
    {
        var exObj = FindObjectsOfType<EssentialObjects>();
        if (exObj.Length == 0)
        {
            var spawPos = new Vector3 (0, 0, 0);
            var grid = FindObjectOfType<Grid>();
            if(grid != null)
            {
                spawPos = grid.transform.position;
            }
            Instantiate(essentialObjectPrehab, spawPos, Quaternion.identity);
        }
    }
}
