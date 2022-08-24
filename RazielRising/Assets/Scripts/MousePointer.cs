using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    //THIS FILE NAME CANNOT BE "CURSOR" !!

    //make cursor file (this one)
    //add script to object
    //link sprite to script
    //obj must have boxcollider (boxcollider 2d seems to work, might give collision though) 

    //tutorial vid is mainly for changing cursor on hover for certain object (https://www.youtube.com/watch?v=W4SE0_cfAqc)
    //think Start() method is pretty much all we need


    public Texture2D cursorArrow;   //default cursor
    //public Texture2D cursorEnemy;   //cursor shown when hovering over specific thing


    void Start()
    {
        //Cursor.visible=false; //hides cursor
        Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
    }

    // void OnMouseEnter()
    // {
    //     Cursor.SetCursor(cursorEnemy, Vector2.zero, CursorMode.ForceSoftware);
    // }

    // void OnMouseExit()
    // {
    //     Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
    // }
}
