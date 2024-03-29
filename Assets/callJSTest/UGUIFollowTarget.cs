﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// 挂在UI上
/// </summary>

public class UGUIFollowTarget : MonoBehaviour
{
    public Camera Cam;//3d摄像机
    public GameObject Target;//跟随的目标
    private Canvas canvas;
    CanvasScaler canvasScaler;
    private RectTransform rectTransform;
    public Vector3 currentPos;//当前位置

    CanvasGroup canvasGroup;//控制显示隐藏
    public bool IsRayCheckCollision = false;//是否开启检测碰撞
    public float DisCamera;//到摄像机的距离
    public bool isup;//凸显当前界面，把当前界面顶到最上层


    private bool isUseCanvasScaler = true;

    public bool IsCheckDistance = false;//是否检测到摄像头的距离
    public bool IsOnCameraBack;//Taget是否在摄像机背面
    private float maxDistance;//界面可显示的最远距离

    private bool setTargetChildActive = false;//是否设置目标子物体的状态
    /// <summary>
    /// 是否设置目标子物体的状态
    /// </summary>
    public bool SetTargetChildActive
    {
        get { return setTargetChildActive; }
        set { setTargetChildActive = value; }
    }
    /// <summary>
    /// 是否忽略Canvas的CanvasScaler组件
    /// </summary>
    public bool IsUseCanvasScaler
    {
        get
        {
            return isUseCanvasScaler;
        }

        set
        {
            isUseCanvasScaler = value;
        }
    }

    LayerMask mask;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (canvas == null)
        {
            canvas = gameObject.GetComponentInParent<Canvas>();
            if (canvas)
            {
                canvasScaler = canvas.GetComponent<CanvasScaler>();
            }
        }
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        currentPos = rectTransform.localPosition;//不赋值，当物体到屏幕位置为（0，0）时，会和vector3的默认值相同。导致位置更新不了
    }

    private void OnEnable()
    {
        Init();
        FollowTarget();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Target == null)
        //{
        //    DestroyImmediate(gameObject);
        //    return;
        //}

        //FollowTarget();
        //GetDisTargetandCam();
        //if (IsRayCheckCollision)
        //{
        //    bool b = RayCheckCollision();
        //    SetShowOrHide(!b);
        //}
    }

    void LateUpdate()
    {
        if (EnableFollow == false) return;

        if (Target == null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        SetChildrenActive(Target.activeInHierarchy);

        //if (Target.activeInHierarchy == false)
        //{
        //    gameObject.SetActive(false);
        //}
        FollowTarget();
        GetDisTargetandCam();
        if (IsRayCheckCollision && !IsOnCameraBack)//检测射线，并且物体得在摄像机前方才行
        {
            bool b = RayCheckCollision();
            b = CheckDistance(b);
            SetShowOrHide(!b);
        }
    }

    public bool EnableFollow = true;

    public void StartFollow()
    {
        EnableFollow = true;
    }

    public void StopFollow()
    {
        EnableFollow = false;
    }

    IEnumerator CoroutineLateUpdate()
    {
        yield return null;
        if (Target == null)
        {
            DestroyImmediate(gameObject);
            yield break;
        }
        //if (Target.activeInHierarchy == false)
        //{
        //    gameObject.SetActive(false);
        //}
        FollowTarget();
        GetDisTargetandCam();
        if (IsRayCheckCollision && !IsOnCameraBack)//检测射线，并且物体得在摄像机前方才行
        {
            bool b = RayCheckCollision();
            b = CheckDistance(b);
            SetShowOrHide(!b);
        }

    }

    void OnGUI()
    {
        //if (Target == null)
        //{
        //    DestroyImmediate(gameObject);
        //    return;
        //}
        ////if (Target.activeInHierarchy == false)
        ////{
        ////    gameObject.SetActive(false);
        ////}
        //FollowTarget();
        //GetDisTargetandCam();
        //if (IsRayCheckCollision && !IsOnCameraBack)//检测射线，并且物体得在摄像机前方才行
        //{
        //    bool b = RayCheckCollision();
        //    b = CheckDistance(b);
        //    SetShowOrHide(!b);
        //}
    }

    public void SetLayerMask(int layerint = -1)
    {
        if (layerint == -1)
        {
            mask = 0 << LayerMask.NameToLayer("IgnoreRaycast");
        }
        else
        {
            mask = layerint;
        }
    }

    /// <summary>
    /// 设置距离检测
    /// </summary>
    /// <param name="isDistanceCheckOn"></param>
    /// <param name="minDis"></param>
    /// <param name="maxDis"></param>
    public void SetEnableDistace(bool isDistanceCheckOn, float maxDis)
    {
        IsCheckDistance = isDistanceCheckOn;
        maxDistance = maxDis;
    }
    private bool CheckDistance(bool isRayOtherCollider)
    {
        //只有开启距离检测才启用，目前仅用于开启射线检测的
        if (IsCheckDistance && !isRayOtherCollider)
        {
            return DisCamera > maxDistance ? true : false;
        }
        return isRayOtherCollider;
    }
    //设置UI的显示隐藏
    private void SetShowOrHide(bool b)
    {
        if (b)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }


    }

    private void SetChildrenActive(bool isActive)
    {
        if (!setTargetChildActive) return;
        for (int i = 0; i < transform.childCount; i++)//物体隐藏掉
        {
            var child = transform.GetChild(i);
            child.gameObject.SetActive(isActive);
        }
    }


    /// <summary>
    /// UI跟随目标
    /// </summary>
    public void FollowTarget()
    {
        if (EnableFollow == false) return;
        if (Target == null || Cam == null) return;
        //Vector3 p = Vector3.zero;
        Vector3 p = Cam.WorldToScreenPoint(Target.transform.position);

        //如果物体在摄像机的后面，摄像机看不到，就不显示跟随UI
        if (p.z < 0)
        {
            IsOnCameraBack = true;
            SetShowOrHide(false);
            return;
        }
        else
        {
            IsOnCameraBack = false;
            SetShowOrHide(true);
        }


        Vector3 p1;
        if (canvas == null)
        {
            p1 = new Vector3(p.x - Screen.width / 2, p.y - Screen.height / 2, 0);
        }
        else
        {
            if (IsUseCanvasScaler)
            {
                //CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
                p1 = WorldToUI(Cam, canvasScaler, Target.transform.position);
            }
            else
            {
                p1 = WorldToUIWithIgnoreCanvasScaler(Cam, canvas, Target.transform.position);
            }
        }


        if (Math.Round(currentPos.x, 2) != Math.Round(p1.x, 2) || Math.Round(currentPos.y, 2) != Math.Round(p1.y, 2))
        {
            rectTransform.localPosition = p1;
            currentPos = rectTransform.localPosition;
        }

    }


    /// <summary>
    /// 世界物体位置转换为UI位置的方式
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="canvasT"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector3 WorldToUI(Camera camera, CanvasScaler canvasScalerT, Vector3 pos)
    {
        //if (canvasT == null)
        //{
        //    Debug.LogError("UGUIFollowTarget no Canvas");
        //    return Vector3.zero;
        //}

        //canvasScalerT = canvasT.GetComponent<CanvasScaler>();

        Vector3 screenPos = camera.WorldToScreenPoint(pos);

        return ScreenToUI(camera, canvasScalerT, screenPos);
    }

    /// <summary>
    /// 屏幕转换到UGUI
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="canvasScalerT"></param>
    /// <param name="screenPos"></param>
    /// <returns></returns>
    public static Vector3 ScreenToUI(Camera camera, CanvasScaler canvasScalerT, Vector3 screenPos)
    {
        float yOffset = 0;
        float xOffset = 0;
        Rect rec = camera.rect;
        yOffset = Screen.height * (rec.height / 2 + rec.y - 0.5f);
        xOffset = Screen.width * (rec.width / 2 + rec.x - 0.5f);

        Vector2 uiPos2 = new Vector2(screenPos.x - (Screen.width / 2) - xOffset, screenPos.y - (Screen.height / 2) - yOffset);
        //uiPos2 = uiPos2 / scaler.scaleFactor;

        float scaleFactor = 1;

        if (canvasScalerT.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
        {
            uiPos2 = uiPos2 / canvasScalerT.scaleFactor;
        }
        else if (canvasScalerT.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            //uiPos2 = new Vector2(uiPos2.x * canvasScalerT.referenceResolution.x / Screen.width, uiPos2.y * canvasScalerT.referenceResolution.y / Screen.height);


            if (canvasScalerT.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
            {
                // We take the log of the relative width and height before taking the average.
                // Then we transform it back in the original space.
                // the reason to transform in and out of logarithmic space is to have better behavior.
                // If one axis has twice resolution and the other has half, it should even out if widthOrHeight value is at 0.5.
                // In normal space the average would be (0.5 + 2) / 2 = 1.25
                // In logarithmic space the average is (-1 + 1) / 2 = 0
                float kLogBase = 2;
                float logWidth = Mathf.Log(Screen.width / canvasScalerT.referenceResolution.x, kLogBase);
                float logHeight = Mathf.Log(Screen.height / canvasScalerT.referenceResolution.y, kLogBase);
                float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, canvasScalerT.matchWidthOrHeight);
                scaleFactor = Mathf.Pow(kLogBase, logWeightedAverage);

            }
            else if (canvasScalerT.screenMatchMode == CanvasScaler.ScreenMatchMode.Expand)
            {
                scaleFactor = Mathf.Min(Screen.width / canvasScalerT.referenceResolution.x, Screen.height / canvasScalerT.referenceResolution.y);

            }
            else if (canvasScalerT.screenMatchMode == CanvasScaler.ScreenMatchMode.Shrink)
            {
                scaleFactor = Mathf.Max(Screen.width / canvasScalerT.referenceResolution.x, Screen.height / canvasScalerT.referenceResolution.y);

            }
        }

        //Debug.Log("ScreenToUI:"+ scaleFactor);
        Vector3 p = new Vector3(uiPos2.x, uiPos2.y, 0) / scaleFactor;
        return p;
    }

    /// <summary>
    /// 世界物体位置转换为UI位置的方式,此方法不受CanvasScaler约束(就是不计算CanvasScaler对UI的影响)
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="canvasT"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector3 WorldToUIWithIgnoreCanvasScaler(Camera camera, Canvas canvasT, Vector3 pos)
    {
        if (canvasT == null)
        {
            Debug.LogError("UGUIFollowTarget no Canvas");
            return Vector3.zero;
        }
        //CanvasScaler scaler = canvasT.GetComponent<CanvasScaler>();

        Vector3 screenPos = camera.WorldToScreenPoint(pos);

        //float yOffset = 0;
        //float xOffset = 0;
        //Rect rec = camera.rect;
        //yOffset = Screen.height * (rec.height / 2 + rec.y - 0.5f);
        //xOffset = Screen.width * (rec.width / 2 + rec.x - 0.5f);

        //Vector2 uiPos2 = new Vector2(screenPos.x - (Screen.width / 2) - xOffset, screenPos.y - (Screen.height / 2) - yOffset);
        Vector2 uiPos2 = new Vector2(screenPos.x - (Screen.width / 2), screenPos.y - (Screen.height / 2));

        Vector3 p = new Vector3(uiPos2.x, uiPos2.y, 0);
        return p;

        //return p;
    }

    /// <summary>
    /// 射线检测，是否被物体遮挡
    /// </summary>
    private bool RayCheckCollision()
    {
        if (Target == null) return false;
        bool b = false;

        if (Target.transform.name == "TitleTag")
        {
            b = RayCheck(Target.transform.parent.gameObject, Cam, mask);
        }
        else
        {
            b = RayCheck(Target, Cam, mask);
        }

        return b;
    }

    /// <summary>
    /// 射线检测，是否被物体遮挡
    /// </summary>
    public static bool RayCheck(GameObject targetT, Camera cameraT, LayerMask maskT)
    {
        Ray ray = new Ray(targetT.transform.position, cameraT.transform.position - targetT.transform.position);
        RaycastHit hit;
        float dis = Vector3.Distance(targetT.transform.position, cameraT.transform.position);
        if (Physics.Raycast(ray, out hit, dis, maskT))
        {
            if (hit.collider.gameObject != targetT)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 获取跟随目标与Camera的距离
    /// </summary>
    public void GetDisTargetandCam()
    {
        DisCamera = Vector3.Distance(Target.transform.position, Cam.transform.position);
    }

    /// <summary>
    /// 是否开启凸显当前界面，把当前界面顶到最上层（在当前组里面）
    /// </summary>
    /// <param name="isActive"></param>
    public void SetIsUp(bool isActive)
    {
        isup = isActive;
    }

    /// <summary>
    /// 设置是否检测射线碰撞
    /// </summary>
    public void SetIsRayCheckCollision(bool b)
    {
        IsRayCheckCollision = b;
    }
}
