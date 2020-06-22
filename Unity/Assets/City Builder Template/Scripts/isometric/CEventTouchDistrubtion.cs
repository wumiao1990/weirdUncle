//==============================================================
//  Copyright (C) 2017 
//  Create by ChengBo at 2017/6/19 10:42:01.
//  Version 1.0
//  Administrator 
//==============================================================

using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.EventSystems;
using BaseTouchSystem;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(EasyTouch))]
public class CEventTouchDistrubtion : MonoBehaviour
{
    private List<Camera> _activeCameras = new List<Camera>();
    private GameObject _current;
    private static BaseTouchSystem.Gesture _curTouch;
    public static BaseTouchSystem.Gesture CurTouch
    {
        get
        {
            return _curTouch;
        }
    }

    private static Dictionary<GameObject, TouchEventDistrubtionCall> _clickListeners = new Dictionary<GameObject, TouchEventDistrubtionCall>();
    private static Dictionary<GameObject, TouchEventDistrubtionCall> _touchStartListeners = new Dictionary<GameObject, TouchEventDistrubtionCall>();
    private static Dictionary<GameObject, TouchEventDistrubtionCall> _touchDownListeners = new Dictionary<GameObject, TouchEventDistrubtionCall>();
    private static Dictionary<GameObject, TouchEventDistrubtionCall> _touchUpListeners = new Dictionary<GameObject, TouchEventDistrubtionCall>();

    //! _isSpecialStatus 为true时仅响应对 _specialObjs 的事件
    private static bool _isSpecialStatus = false;
    private static GameObject _specialObj = null;

    public delegate void TouchEventDistrubtionCall(GameObject go);

    private void Awake()
    {
        _isSpecialStatus = false;
        _specialObj = null;
    }

    public static void SetEnabled(bool v)
    {
        EasyTouch.SetEnabled(v);
    }

    public static bool IsEnable
    {
        get
        {
            return EasyTouch.instance.enable;
        }
    }

    private void Start()
    {
        EasyTouch.On_TouchStart += On_TouchStart;
        EasyTouch.On_TouchDown += On_TouchDown;
        EasyTouch.On_TouchUp += On_TouchUp;
    }

    public void AddCamera(Camera newCamera)
    {
        if (newCamera != null)
        {
            _activeCameras.Add(newCamera);
        }
    }

    public void RemoveCamera(Camera removeCamera)
    {
        _activeCameras.Remove(removeCamera);
    }

    public static bool RegisterClickEventAtHead(GameObject go, TouchEventDistrubtionCall call)
    {
        if (go != null && _clickListeners.ContainsKey(go))
        {
            _clickListeners[go] = call + _clickListeners[go];
            return true;
        }
        else return false;
    }

    public static void RegisterClickEvent(GameObject go, TouchEventDistrubtionCall call)
    {
        if (go != null)
        {
            if (!_clickListeners.ContainsKey(go))
            {
                _clickListeners.Add(go, call);
            }
            else
            {
                _clickListeners[go] += call;
            }
        }
    }

    public static void UnRegisterClickEvent(GameObject go)
    {
        if (go != null && _clickListeners.ContainsKey(go))
        {
            _clickListeners.Remove(go);
        }
    }

    public static void UnRegisterClickEvent(GameObject go, TouchEventDistrubtionCall call)
    {
        if (go != null && _clickListeners.ContainsKey(go))
        {
            _clickListeners[go] -= call;
            if (_clickListeners[go] == null || _clickListeners[go].GetInvocationList().Length == 0)
            {
                _clickListeners.Remove(go);
            }
        }
    }

    public static void RegisterStartEvent(EasyTouch.TouchStartHandler call)
    {
        EasyTouch.On_TouchStart += call;
    }

    public static void RegisterStartEvent(GameObject go, TouchEventDistrubtionCall call)
    {
        if (go != null)
        {
            if (!_touchStartListeners.ContainsKey(go))
            {
                _touchStartListeners.Add(go, call);
            }
            else
            {
                _touchStartListeners[go] += call;
            }
        }
    }

    public static void UnRegisterStartEvent(GameObject go)
    {
        if (go != null && _touchStartListeners.ContainsKey(go))
        {
            _touchStartListeners.Remove(go);
        }
    }

    public static void UnRegisterStartEvent(EasyTouch.TouchStartHandler call)
    {
        EasyTouch.On_TouchStart -= call;
    }

    public static void UnRegisterStartEvent(GameObject go, TouchEventDistrubtionCall call)
    {
        if (go == null)
            return;

        if (_touchStartListeners.ContainsKey(go))
        {
            _touchStartListeners[go] -= call;
            if (_touchStartListeners[go] == null ||
                _touchStartListeners[go].GetInvocationList().Length == 0)
            {
                _touchStartListeners.Remove(go);
            }
        }
    }

    public static void RegisterDownEvent(EasyTouch.TouchDownHandler call)
    {
        EasyTouch.On_TouchDown += call;
    }

    public static void UnRegisterDownEvent(EasyTouch.TouchDownHandler call)
    {
        EasyTouch.On_TouchDown -= call;
    }

    public static void RegisterDownEvent(GameObject go, TouchEventDistrubtionCall call)
    {
        if (go != null)
        {
            if (!_touchDownListeners.ContainsKey(go))
            {
                _touchDownListeners.Add(go, call);
            }
            else
            {
                _touchDownListeners[go] += call;
            }
        }
    }

    public static void UnRegisterDownEvent(GameObject go)
    {
        if (go != null && _touchDownListeners.ContainsKey(go))
        {
            _touchDownListeners.Remove(go);
        }
    }

    public static void UnRegisterDownEvent(GameObject go, TouchEventDistrubtionCall call)
    {
        if (go == null)
            return;

        if (_touchDownListeners.ContainsKey(go))
        {
            _touchDownListeners[go] -= call;
            if (_touchDownListeners[go] == null ||
                _touchDownListeners[go].GetInvocationList().Length == 0)
            {
                _touchDownListeners.Remove(go);
            }
        }
    }

    public static void RegisterUpEvent(EasyTouch.TouchUpHandler call)
    {
        EasyTouch.On_TouchUp += call;
    }

    public static void UnRegisterUpEvent(EasyTouch.TouchUpHandler call)
    {
        EasyTouch.On_TouchUp -= call;
    }

    public static void RegisterUpEvent(GameObject go, TouchEventDistrubtionCall call)
    {
        if (go != null)
        {
            if (!_touchUpListeners.ContainsKey(go))
            {
                _touchUpListeners.Add(go, call);
            }
            else
            {
                _touchUpListeners[go] += call;
            }
        }
    }

    public static void UnRegisterUpEvent(GameObject go)
    {
        if (go != null && _touchUpListeners.ContainsKey(go))
        {
            _touchUpListeners.Remove(go);
        }
    }

    public static void UnRegisterUpEvent(GameObject go, TouchEventDistrubtionCall call)
    {
        if (go != null && _touchUpListeners.ContainsKey(go))
        {
            _touchUpListeners[go] -= call;
            if (_touchUpListeners[go] == null ||
                _touchUpListeners[go].GetInvocationList().Length == 0)
            {
                _touchUpListeners.Remove(go);
            }
        }
    }

    private void On_TouchStart(BaseTouchSystem.Gesture gesture)
    {
        if (gesture.fingerIndex > 0) return;
        _curTouch = gesture;
        float dis = 1000000;
        float depth = -1;
        GameObject go = null;
        foreach (Camera cm in _activeCameras)
        {
            if (cm == null) continue;
            if (!cm.gameObject.activeSelf) continue;
            if (!cm.gameObject.activeInHierarchy) continue;
            if (!cm.enabled) continue;

            Ray r = cm.ScreenPointToRay(gesture.position);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000, cm.cullingMask))
            {
                if (cm.depth > depth && hit.distance < dis)
                {
                    go = hit.collider.gameObject;
                    dis = hit.distance;
                    depth = cm.depth;
                }
            }
        }
        if (CanHandleTouch(go))
        {
            _current = go;
            if (_touchStartListeners.ContainsKey(_current))
            {
                _touchStartListeners[_current].Invoke(_current);

                // Debug.LogError("_touchStartListeners " + _current.name);
            }
        }
    }

    private void On_TouchDown(BaseTouchSystem.Gesture gesture)
    {
        if (gesture.fingerIndex > 0) return;
        _curTouch = gesture;
        float dis = 1000000;
        float depth = -1;
        GameObject go = null;
        foreach (Camera cm in _activeCameras)
        {
            if (cm == null) continue;
            if (!cm.gameObject.activeSelf) continue;
            if (!cm.gameObject.activeInHierarchy) continue;
            if (!cm.enabled) continue;

            Ray r = cm.ScreenPointToRay(gesture.position);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000, cm.cullingMask))
            {
                if (cm.depth > depth && hit.distance < dis)
                {
                    go = hit.collider.gameObject;
                    dis = hit.distance;
                    depth = cm.depth;
                }
            }
        }
        if (CanHandleTouch(go))
        {
            _current = go;
            if (_touchDownListeners.ContainsKey(_current))
            {
                _touchDownListeners[_current].Invoke(_current);

                // Debug.LogError("_touchDownListeners " + _current.name);
            }
        }
    }

    private void On_TouchUp(BaseTouchSystem.Gesture gesture)
    {
        if (gesture.fingerIndex > 0) return;
        _curTouch = gesture;
        float dis = 1000000;
        float depth = -1;
        GameObject go = null;

        foreach (Camera cm in _activeCameras)
        {
            if (cm == null) continue;
            if (!cm.gameObject.activeSelf) continue;
            if (!cm.gameObject.activeInHierarchy) continue;
            if (!cm.enabled) continue;

            Ray r = cm.ScreenPointToRay(gesture.position);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, dis, cm.cullingMask))
            {
                Debug.DrawRay(r.origin, Vector3.forward * hit.distance, Color.green, 5);
                if (cm.depth > depth && hit.distance < dis)
                {
                    go = hit.collider.gameObject;
                    dis = hit.distance;
                    depth = cm.depth;
                }
            }
            else
            {
                Debug.DrawRay(r.origin, Vector3.forward * 1000, Color.black, 5);
            }
        }

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Button _ClickButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (_ClickButton != null)
            {
                

                // Debug.LogError("EventConfig.CLICK_BUTTON " + path);
            }
            else
            {
                Toggle _Tog = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
                if (_Tog != null)
                {
                    
                }
            }

        }

        if (CanHandleTouch(go))
        {
            if (go == _current)
            {
                if (_clickListeners.ContainsKey(go) && _curTouch.actionTime < 0.2f)
                {
                    _clickListeners[go].Invoke(go);

                    // Debug.LogError("_clickListeners " + _current.name);
                }
            }
            if (_touchUpListeners.ContainsKey(go))
            {
                _touchUpListeners[go].Invoke(go);

                // Debug.LogError("_touchUpListeners " + _current.name);
            }
        }
        /*
        {
            if (Ngame.ABSystem.AssetBundleManager.Instance.Inited)
            {
                CBEffectInstance eff = CBEffectPool.Instance.PlayEffect("Effect/Prefabe/uieffect_dianji", 1f);
                Vector3 pos = CBEffectPool.Instance.EffectCamera.ScreenToWorldPoint(new Vector3(_curTouch.position.x, _curTouch.position.y, 10));
                eff.Effect.transform.position = pos;
            }
        }
        */
    }

    private const string DisableTouchEffectTag = "DiableTouchEffect";
    private const string UntaggedTag = "Untagged";
    public static void DisableTouchEffect(GameObject target)
    {
        if (target != null)
        {
            if (target.tag == UntaggedTag || target.tag == DisableTouchEffectTag)
            {
                target.tag = DisableTouchEffectTag;
            }
            else
            {
                Debug.LogErrorFormat("CEventTouchDistribution.DisableTouchEffect, the gameObject {0} has tagged {1}", target.name, target.tag);
            }
        }
    }

    public static void EnableTouchEffect(GameObject target)
    {
        if (target != null && target.tag == DisableTouchEffectTag)
        {
            target.tag = UntaggedTag;
        }
    }

    private static bool IsTouchEffectEnabled(GameObject target)
    {
        return (target == null || target.tag != DisableTouchEffectTag);
    }

    private void OnDestroy()
    {
        EasyTouch.On_TouchStart -= On_TouchStart;
        EasyTouch.On_TouchDown -= On_TouchDown;
        EasyTouch.On_TouchUp -= On_TouchUp;
        //_clickListeners.Clear();
        //_touchDownListeners.Clear();
        //_touchStartListeners.Clear();
        //_touchUpListeners.Clear();
        //if (_activeCameras != null)
        //{
        //    _activeCameras.Clear();
        //    _activeCameras = null;
        //}
        //_instance = null;
    }

    private bool HasAnyTouchUp()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return Input.GetMouseButtonUp(0);
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            return (touch.phase == TouchPhase.Ended);
        }
        else
        {
            return false;
        }
#else
        return false;
#endif
    }
    
    private bool GetTouchPos(out Vector2 pos)
    {
        pos = Vector2.zero;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        pos = Input.mousePosition;
        return true;
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            pos = touch.position;
            return true;
        }
        else
        {
            return false;
        }
#else
        return false;
#endif

    }


    private void Update()
    {
        if (!EasyTouch.instance.enable)
            return;

        if (HasAnyTouchUp())
        {
            Vector2 touchPos;
            var valid = GetTouchPos(out touchPos);
            if (valid)
            {
                TouchSfx();
            }
        }
    }

    private void TouchSfx()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Button _ClickButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (_ClickButton != null)
            {
                _ClickButton = null;
            }
            else
            {
                Toggle _Tog = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
                if (_Tog != null)
                {
                }
            }
        }
    }

    public static bool StartSpecialStatus(GameObject specialObj)
    {
        bool hasSpeicalObj = false;
        if (!specialObj.IsNull())
        {
            _isSpecialStatus = true;
            if (_clickListeners.ContainsKey(specialObj)
                || _touchStartListeners.ContainsKey(specialObj)
                || _touchDownListeners.ContainsKey(specialObj)
                || _touchUpListeners.ContainsKey(specialObj))
            {
                _specialObj = specialObj;
                hasSpeicalObj = true;
            }
        }
        else
        {
            Debug.LogErrorFormat("CEventTouchDistrubtion.StartSpecialStatus, specialObj is NULL");
        }
        return hasSpeicalObj;
    }

    public static bool EndSpecialStatus()
    {
        bool hasSpeicalObj = (_specialObj != null);
        _isSpecialStatus = false;
        _specialObj = null;
        return hasSpeicalObj;
    }

    private bool CanHandleTouch(GameObject obj)
    {
        if (obj != null)
        {
            return (!_isSpecialStatus || obj == _specialObj);
        }
        else
        {
            return false;
        }
    }
}

