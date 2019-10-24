using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ShowHtmlElement : MonoBehaviour
{
    public InputField InputHtmlName;

    public RectTransform Rect;

    public void StartFollow()
    {
        Following = true;
    }

    public void StopFollow()
    {
        Following = false;
        HideElement();
    }

    public bool Following = false;



    public void FixedUpdate()
    {
        if (Following)
        {
            ShowElement();
        }
    }

    public void HideElement()
    {
        Debug.Log(">>>> ShowHtmlElement.HideElement");

        var htmlName = InputHtmlName.text;
        Debug.Log("htmlName:" + htmlName);
        UnityTool_HideHtmlElement(htmlName);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(">>>> ShowHtmlElement.Start");
        Debug.Log("Screen.width:" + Screen.width);
        Debug.Log("Screen.height:" + Screen.height);
        Debug.Log("Screen.safeArea:" + Screen.safeArea);
        UnityTool_SetScreenInfo(Screen.width, Screen.height);
    }

    public void ShowElement()
    {
        Debug.Log(">>>> ShowHtmlElement.ShowElement");
        var pos = Rect.anchoredPosition;
        Debug.Log("pos:" + pos);
        var rect = Rect.rect;
        Debug.Log("rect:" + rect);
        var size = new Vector2(rect.width, rect.height);
        Debug.Log("size:" + size);
        var htmlName = InputHtmlName.text;
        Debug.Log("htmlName:" + htmlName);
        UnityTool_ShowHtmlElement(htmlName, pos.x, pos.y, rect.width, rect.height);
    }

    [DllImport("__Internal")]
    private static extern void UnityTool_SetScreenInfo(float sizeX, float sizeY);

    [DllImport("__Internal")]
    private static extern void UnityTool_ShowHtmlElement(string id, float posX, float posY, float sizeX, float sizeY);

    [DllImport("__Internal")]
    private static extern void UnityTool_HideHtmlElement(string id);
}
