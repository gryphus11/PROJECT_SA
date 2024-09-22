using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{

    public ObjectType ObjectType { get; protected set; } = ObjectType.Unknown;

    bool _init = false;

    public virtual bool Init()
    {
        if (_init)
            return false;


        _init = true;
        return true;
    }

    private void Awake()
    {
        Init();
    }

    protected virtual void UpdateController()
    {

    }

    private void Update()
    {
        UpdateController();
    }
}
