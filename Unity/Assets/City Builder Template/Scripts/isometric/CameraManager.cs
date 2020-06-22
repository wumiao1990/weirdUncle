using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
	private Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

	public class CameraEvent
	{
		public Vector3 point;
		public BaseItemScript baseItem;
	}

	public enum RaycastTarget
	{
		BASE_ITEM,
		GROUND
	}

	public static CameraManager instance;

	/* object refs */
	public Camera MainCamera;
	public Text debugText;

	public EventSystem EventSystem;

	//public Transform CameraClampTopLeft;
	//public Transform CameraClampBottomRight;

	/* public variables */
	public UnityAction<CameraEvent> OnItemTap;
	public UnityAction<CameraEvent> OnItemDragStart;
	public UnityAction<CameraEvent> OnItemDrag;
	public UnityAction<CameraEvent> OnItemDragStop;
	public UnityAction<CameraEvent> OnTapGround;


	/* private vars */

	private int _layerMaskBaseItemCollider;
	private int _layerMaskGroundCollider;

	private float screenRatio = Screen.width / Screen.height;
	private Vector2 _defaultTouchPos = new Vector2(9999, 9999);
	private float _minimumMoveDistanceForItemMove = 0.2f;
	private float _maxZoomFactor = 30;
	private float _minZoomFactor = 3;
	private float _clampZoomOffset = 2.0f;

	private Vector3 _tapItemStartPos;
	private Vector3 _tapGroundStartPosition;

	private bool _isTappedBaseItem;
	private bool _isDraggingBaseItem;
	private bool _isPanningScene;

	private BaseItemScript _selectedBaseItem;

	void Awake()
	{
		instance = this;
		this._layerMaskBaseItemCollider = LayerMask.GetMask("BaseItemCollider");
		this._layerMaskGroundCollider = LayerMask.GetMask("GroundCollider");
		
		CEventTouchDistrubtion.RegisterDownEvent(onBackInputDown);
		CEventTouchDistrubtion.RegisterUpEvent(onBackInputUp);
	}
	
	bool _isDown = false;
    Vector2 _v2Down = Vector2.zero;
    float _DragDis = 0;
    bool _isFull = false;
    private float thredhold = 2;

    void onBackInputDown(BaseTouchSystem.Gesture _selection)
    {
        if (!_isDown)
        {
            _isDown = true;
            _v2Down = _selection.startPosition;
            _DragDis = 0;

        }
        else
        {
	        if(_tapStartRaycastedItem != null)
	        {
		        return;
	        }
            _v2Down += CEventTouchDistrubtion.CurTouch.deltaPosition;            
            _DragDis += Vector2.SqrMagnitude(CEventTouchDistrubtion.CurTouch.deltaPosition);

            if (_DragDis > thredhold)
            {
                Vector3 oldpos = transform.position;
                transform.position += new Vector3(-CEventTouchDistrubtion.CurTouch.deltaPosition.x / 100.0f,
                                                            CEventTouchDistrubtion.CurTouch.deltaPosition.y / 100.0f,
                                                            0);
                Vector3 pos = transform.position;
                pos.x = Mathf.Clamp(pos.x, 7, 20);
                transform.position = new Vector3(pos.x, 3.95f, transform.position.z);
            }
        }
    }

    void onBackInputUp(BaseTouchSystem.Gesture _selection)
    {
        if (_isDown)
        {
            //  点击
            if (_DragDis < thredhold)
            {
            }
            else
            {
                //  拖动
            }

            _isDown = false;
        }
    }

	void Update()
	{
		if (this.IsUsingUI())
		{
			return;
		}

		this.UpdateBaseItemTap();
		this.UpdateBaseItemMove();
		this.UpdateGroundTap();
		this.UpdateScenePan();
		//this.UpdateSceneZoom();
	}

	public bool IsUsingUI()
	{
		if (this._isDraggingBaseItem)
		{
			return false;
		}

		if (_isPanningSceneStarted)
		{
			return false;
		}

		return (EventSystem.IsPointerOverGameObject() || EventSystem.IsPointerOverGameObject(0));
	}


	private void _GetTouches(out int touchCount, out Vector2 touch0, out Vector2 touch1)
	{
		touchCount = 0;
		touch0 = _defaultTouchPos;
		touch1 = _defaultTouchPos;

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			touchCount = Input.touchCount;
			touch0 = Input.GetTouch(0).position;
		}
		else
		{
			if (Input.GetMouseButton(0))
			{
				touchCount = 1;
				touch0 = Input.mousePosition;
			}
			else
			{
				touchCount = 0;
				touch0 = _defaultTouchPos;
			}
		}
	}

	private BaseItemScript _TryGetRaycastHitBaseItem(Vector2 touch)
	{
		RaycastHit hit;
		if (_TryGetRaycastHit(touch, out hit, RaycastTarget.BASE_ITEM))
		{
			return hit.collider.gameObject.GetComponent<BaseItemScript>();
		}

		return null;
	}

	private Vector3 _TryGetRaycastHitBaseGround(Vector2 touch)
	{
		RaycastHit hit;
		if (_TryGetRaycastHit(touch, out hit, RaycastTarget.GROUND))
		{
			return hit.point;
		}
		return positiveInfinityVector;
	}

	private bool _TryGetRaycastHit(Vector2 touch, out RaycastHit hit, RaycastTarget target)
	{
		Ray ray = MainCamera.ScreenPointToRay(touch);
		return (Physics.Raycast(ray, out hit, 1000, (target == RaycastTarget.BASE_ITEM) ? _layerMaskBaseItemCollider : _layerMaskGroundCollider));
	}

	public void UpdateBaseItemTap()
	{
		if (!Input.GetMouseButtonUp(0))
		{
			return;
		}


		if (this._isPanningSceneStarted)
		{
			return;
		}

		if (this._isDraggingBaseItem)
		{
			return;
		}

		if (this.IsUsingUI())
		{
			return;
		}

		BaseItemScript baseItemTapped = this._TryGetRaycastHitBaseItem(Input.mousePosition);
		if (baseItemTapped != null)
		{
			this._isTappedBaseItem = true;

			this._selectedBaseItem = baseItemTapped;

			CameraEvent evt = new CameraEvent()
			{
				baseItem = baseItemTapped
			};
			if (this.OnItemTap != null)
			{
				this.OnItemTap.Invoke(evt);
			}


		}
		else
		{
			this._isTappedBaseItem = false;
			this._selectedBaseItem = null;
		}
	}

	public void UpdateGroundTap()
	{
		if (this._isTappedBaseItem)
		{
			return;
		}

		if (this._isDraggingBaseItem)
		{
			return;
		}

		if (this._isPanningScene)
		{
			return;
		}

		if (this._isPanningSceneStarted)
		{
			return;
		}

		if (!Input.GetMouseButtonUp(0))
		{
			return;
		}

		Vector3 tapPosition = this._TryGetRaycastHitBaseGround(Input.mousePosition);
		if (tapPosition != positiveInfinityVector)
		{
			//			Debug.Log ("GroundTap");
			CameraEvent evt = new CameraEvent()
			{
				point = tapPosition
			};
			if (this.OnTapGround != null)
			{
				this.OnTapGround.Invoke(evt);
			}
		}
	}

	private BaseItemScript _tapStartRaycastedItem = null;
	private bool _isDragItemStarted;
	private bool _baseItemMoved;
	public void UpdateBaseItemMove()
	{

		if (Input.GetMouseButtonDown(0))
		{
			this._tapItemStartPos = this._TryGetRaycastHitBaseGround(Input.mousePosition);
			this._tapStartRaycastedItem = this._TryGetRaycastHitBaseItem(Input.mousePosition);
			this._isDraggingBaseItem = false;
			this._isDragItemStarted = false;
		}


		if (Input.GetMouseButton(0) && this._tapItemStartPos != positiveInfinityVector)
		{
			if (this._isTappedBaseItem && this._selectedBaseItem == this._tapStartRaycastedItem)
			{
				Vector3 currentTapPosition = this._TryGetRaycastHitBaseGround(Input.mousePosition);
				if (Vector3.Distance(this._tapItemStartPos, currentTapPosition) >= _minimumMoveDistanceForItemMove)
				{

					CameraEvent evt = new CameraEvent()
					{
						point = currentTapPosition,
						baseItem = this._selectedBaseItem
					};

					if (!this._isDragItemStarted)
					{
						//						Debug.Log ("BaseItemDragStart");
						this._isDragItemStarted = true;
						if (this.OnItemDragStart != null)
						{
							this.OnItemDragStart.Invoke(evt);
						}
					}

					//					Debug.Log ("BaseItemDrag");
					this._isDraggingBaseItem = true;
					if (this.OnItemDrag != null)
					{
						this.OnItemDrag.Invoke(evt);
					}
				}
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			this._tapItemStartPos = positiveInfinityVector;
			if (_isDragItemStarted)
			{
				//				Debug.Log ("BaseItemDragStop");
				this._isDragItemStarted = false;
				this._isDraggingBaseItem = false;
				if (this.OnItemDragStop != null)
				{
					this.OnItemDragStop.Invoke(null);
				}
			}
		}
	}

	private int _previousTouchCount = 0;
	private bool _touchCountChanged;
	private Vector2 _touchPosition;
	private bool _canPan;

	private void _RefreshTouchValues()
	{
		this._touchCountChanged = false;
		int touchCount = 0;
		bool isInEditor = false;


		if (Input.touchCount == 0)
		{
			if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)))
			{
				//editor
				touchCount = 1;
				isInEditor = true;
			}
			else
			{
				touchCount = 0;
			}

		}
		else
		{
			if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				touchCount = 0;
			}
			else
			{
				touchCount = Input.touchCount;
			}
		}

		if (touchCount != this._previousTouchCount)
		{
			if (touchCount != 0)
			{
				this._touchCountChanged = true;
			}
		}

		if (isInEditor)
		{
			this._touchPosition = (Vector2)Input.mousePosition;
		}
		else
		{
			if (touchCount == 1)
			{
				this._touchPosition = Input.GetTouch(0).position;
			}
			else if (touchCount >= 2)
			{
				this._touchPosition = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2.0f;
			}
		}

		this._canPan = (touchCount > 0);

		this._previousTouchCount = touchCount;
	}

	private bool _isPanningSceneStarted;
	public void UpdateScenePan()
	{
		_RefreshTouchValues();
		if (this._isDraggingBaseItem)
		{
			return;
		}
		
		//OnScenePan();
	}

	private Vector3 _touchPoint1;
	private Vector3 _touchPoint2;
	private bool _isZoomingStarted;
	private float _previousPinchDistance;
	private float _oldZoom = -1;
	


	//panning
	private Vector3 _previousPanPoint;
	private Vector3 _panVelocity = Vector3.zero;
	public void OnChangeTouchCountScenePan(CameraEvent evt)
	{
		this._previousPanPoint = evt.point;
	}
	
	private float sensitivityAmt = 0.5f;
	public void OnScenePan()
	{
		if (Input.GetMouseButton(0) || Input.touchCount > 0)
		{
			Vector3 p0 = transform.position;
			Vector3 p01 = p0 - transform.right * Input.GetAxisRaw("Mouse X") * sensitivityAmt * Time.timeScale;
			Vector3 p03 = p01 - transform.up * Input.GetAxisRaw("Mouse Y") * sensitivityAmt * Time.timeScale;
			float x = p03.x;
			if (x <= 9)
			{
				x = 9;
			}
			if (x >= 22)
			{
				x = 22;
			}
			transform.position = new Vector3(x, 3.95f, 0);
		}
	}


	//clamps the camera within the scene limits, the limits can adjusted with '_CameraClampLeft' and 
	//'_CameraClampRight' components


    
    /* SHAKE SCRIPT */

    private float _shakeAmount = 5.0f;
	private float _shakeDuration = 1.0f;
    private bool _isShaking = false;

	private Vector3 _originalPos;
    
	public void ShakeCamera()
	{
		ShakeCamera(0.5f, 0.5f);
    }

    public void ShakeCamera(float amount, float duration)
    {
		_originalPos = transform.localPosition;
		_shakeAmount = amount;
		_shakeDuration = duration;

        if (!_isShaking) 
			StartCoroutine(_Shake());
    }


    private IEnumerator _Shake()
    {
		_isShaking = true;
		while (_shakeDuration > 0.01f)
		{
			this.transform.localPosition = _originalPos + Random.insideUnitSphere * _shakeAmount;
            _shakeDuration -= Time.deltaTime;
			yield return null;
		}

		_shakeDuration = 0f;
        this.transform.localPosition = _originalPos;

        _isShaking = false;
    }

}
