using UnityEngine;
using Bronk;
using System.Collections.Generic;
using System;

public class Hud : MonoBehaviour
{
    private static Texture2D _staticRectTexture;
    private static GUIStyle _staticRectStyle;

    // Note that this function is only meant to be called from OnGUI() functions.
    public static void GUIDrawRect(Rect position, Color color) {
        if (_staticRectTexture == null) {
            _staticRectTexture = new Texture2D(1, 1);
        }
        if (_staticRectStyle == null) {
            _staticRectStyle = new GUIStyle();
        }
        _staticRectTexture.SetPixel(0, 0, color);
        _staticRectTexture.Apply();
        _staticRectStyle.normal.background = _staticRectTexture;
        GUI.Box(position, GUIContent.none, _staticRectStyle);
    }

	void Start ()
	{
	}

	void Update ()
	{
	    
	}

    void OnGUI() {

        var r = new Rect(0, Screen.height - 30, Screen.width, 30);
        GUIDrawRect(r, Color.black);

        var style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.yellow;
        GUI.Label(r, String.Format("Gold: {0}", Game.World.StockpileComponent.GoldCount), style);
    }
}