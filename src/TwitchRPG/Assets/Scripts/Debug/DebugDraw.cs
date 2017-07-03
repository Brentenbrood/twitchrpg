using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using FireFly.Utilities;
using UnityEngine;

public class DebugDraw : MonoBehaviour
{
    private static DebugDraw debugDraw;
    public static DebugDraw Get { get { return debugDraw; } }

    void Start()
    {
        if (!debugDraw)
            debugDraw = this;
        else
            throw new Exception("Only one DebugDraw instance allowed!");


    }

    void LateUpdate () {
		
	}

    
}
