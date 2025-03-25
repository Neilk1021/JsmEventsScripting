using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ExampleJSME : MonoBehaviour
{
    private Animator _animator;
    public void PrintThing(string val)
    {
        Debug.Log(val);
    }

    public void UpdatePosition(int x, int y)
    {
        transform.position = new Vector3(x, y);
    }
}
