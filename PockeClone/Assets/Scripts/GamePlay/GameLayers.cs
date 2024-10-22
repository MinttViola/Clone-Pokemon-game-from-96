using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidobjMask;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;


    public static GameLayers i { get; set; }
    private void Awake()
    {
        i = this;
    }
    public LayerMask SolidobjMask { get { return solidobjMask; } }
    public LayerMask InteractLayer { get { return interactLayer; } }
    public LayerMask GrassLayer { get { return grassLayer; } }
    public LayerMask PlayerLayer { get { return playerLayer; } }
    public LayerMask FovLayer { get { return fovLayer; } }

    public LayerMask PortalLayer => portalLayer;

    public LayerMask TriggerableLayers
    {
        get => grassLayer|fovLayer|portalLayer;
    }


}
