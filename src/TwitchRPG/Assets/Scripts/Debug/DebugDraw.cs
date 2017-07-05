using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using FireFly.Utilities;
using UnityEngine;
using Color = UnityEngine.Color;

public class DebugDraw : MonoBehaviour
{
    private static DebugDraw debugDraw;
    public static DebugDraw Get { get { return debugDraw; } }

    private Texture2D tex;
    private Color32[] emptyscreen;

    void Start()
    {
        if (!debugDraw)
            debugDraw = this;
        else
            throw new Exception("Only one DebugDraw instance allowed!");

        emptyscreen = new Color32[Screen.width * Screen.height];
        for (int i = 0; i < emptyscreen.Length; i++)
            emptyscreen[i] = new Color32(255, 255, 255, 0);

        tex = new Texture2D(Screen.width, Screen.height);
        ClearScreen();

        StartCoroutine(UpdateLoop());
    }

    IEnumerator UpdateLoop()
    {
        while (Application.isPlaying)
        {
            yield return new WaitForEndOfFrame();
            OnEndOfFrame();
        }
    }

    void OnEndOfFrame()
    {
        
    }

    void LateUpdate()
    {
        tex.Apply(false);
    }

    public void ClearScreen()
    {
        tex.SetPixels32(emptyscreen);
        tex.Apply(false);
    }

    public void DrawLine(Vector2 p1, Vector2 p2, Color col)
    {
        Vector2 t = p1;
        float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
        float ctr = 0;

        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
        {
            t = Vector2.Lerp(p1, p2, ctr);
            ctr += frac;
            tex.SetPixel((int)t.x, (int)t.y, col);
        }
    }

    public void DrawRect(Rect rect, Color col)
    {
        DrawLine(rect.position, rect.position + new Vector2(rect.width, 0f), col); //top
        DrawLine(rect.position, rect.position + new Vector2(0f, rect.height), col); //left
        DrawLine(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax), col); //bottom
        DrawLine(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax), col);
    }
}
