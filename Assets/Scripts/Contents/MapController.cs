using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public GridController Grid { get; private set; }

    private void Awake()
    {
        Grid = GetComponent<GridController>();
    }
}
