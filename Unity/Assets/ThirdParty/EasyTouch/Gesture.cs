// EasyTouch v3.0 (October 2013)
// EasyTouch library is copyright (c) of Hedgehog Team
// Please send feedback or bug reports to the.hedgehog.team@gmail.com
namespace BaseTouchSystem
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    public class TouchObjectPool<T> where T : new()
    {
        private readonly Stack<T> m_stack = new Stack<T>();
        private readonly Action<T> m_getAction;
        private readonly Action<T> m_releaseAction;

        public TouchObjectPool(Action<T> getAction, Action<T> releaseAction)
        {
            m_getAction = getAction;
            m_releaseAction = releaseAction;
        }

        public T Get()
        {
            T element;
            if (m_stack.Count > 0)
            {
                element = m_stack.Pop();
            }
            else
            {
                element = new T();
            }
            if (m_getAction != null)
                m_getAction(element);
            return element;
        }

        public void Recycle(T element)
        {
            if (element == null)
                return;

            //理论上应该全部检查是否重复回收，但是为了效率，只检查最近的一次回收
            if (m_stack.Count > 0 && object.ReferenceEquals(m_stack.Peek(), element))
            {
                Debug.LogError("是不是重复回收了，刚刚已经回收了");
                return;
            }

            if (m_releaseAction != null)
                m_releaseAction(element);
            m_stack.Push(element);
        }

        public void Clean()
        {
            m_stack.Clear();
        }
    }

    /// <summary>
    /// This is the class passed as parameter by EasyTouch events, that containing all informations about the touch that raise the event,
    /// or by the tow fingers gesture that raise the event.
    /// </summary>
    public class Gesture
    {
        //优化：使用池来放Gesture，避免每次new
        private static readonly TouchObjectPool<Gesture> s_listPool = new TouchObjectPool<Gesture>(null, (g) =>
        {
            g.pickObject = null;
            g.otherReceiver = null;
        });

        /// <summary>
        /// 获得Gesture，从池中取，优化掉new的开销
        /// </summary>
        /// <returns></returns>
        public static Gesture Get()
        {
            return s_listPool.Get();
        }

        /// <summary>
        /// 回收Gesture
        /// </summary>
        /// <param name="g"></param>
        public static void Recycle(Gesture g)
        {
            s_listPool.Recycle(g);
        }

        /// <summary>
        /// The index of the finger that raise the event (Starts at 0), or -1 for a two fingers gesture.
        /// </summary>
        public int fingerIndex;
        /// <summary>
        /// The touches count.
        /// </summary>
        public int touchCount;
        /// <summary>
        /// The start position of the current gesture, or the center position between the two touches for a two fingers gesture.
        /// </summary>
        public Vector2 startPosition;
        /// <summary>
        /// The current position of the touch that raise the event,  or the center position between the two touches for a two fingers gesture.
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// The position delta since last change.
        /// </summary>
        public Vector2 deltaPosition;
        /// <summary>
        /// Time since the beginning of the gesture.
        /// </summary>
        public float actionTime;
        /// <summary>
        /// Amount of time passed since last change.
        /// </summary>
        public float deltaTime;
        /// <summary>
        /// The siwpe or drag  type ( None, Left, Right, Up, Down, Other => look at EayTouch.SwipeType enumeration).
        /// </summary>
        public EasyTouch.SwipeType swipe;
        /// <summary>
        /// The length of the swipe.
        /// </summary>
        public float swipeLength;
        /// <summary>
        /// The swipe vector direction.
        /// </summary>
        public Vector2 swipeVector;
        /// <summary>
        /// The pinch length delta since last change.
        /// </summary>
        public float deltaPinch;
        /// <summary>
        /// The angle of the twist.
        /// </summary>
        public float twistAngle;
        /// <summary>
        /// The distance between two finger for a two finger gesture.
        /// </summary>
        public float twoFingerDistance;
        /// <summary>
        /// The current picked gameObject under the touch that raise the event.
        /// </summary>
        public GameObject pickObject;
        /// <summary>
        /// The pick camera.
        /// </summary>
        public Camera pickCamera;
        /// <summary>
        /// Is that the camera is Flage GUI
        /// </summary>
        public bool isGuiCamera;
        /// <summary>
        /// Other receiver of the event.
        /// </summary>
        public GameObject otherReceiver;
        /// <summary>
        /// The is hover controller.
        /// </summary>
        public bool isHoverReservedArea;



        /// <summary>
        /// Transforms touch position into world space, or the center position between the two touches for a two fingers gesture.
        /// </summary>
        /// <returns>
        /// The touch to word point.
        /// </returns>
        /// <param name='z'>
        /// The z position in world units from the camera or in world depending on worldZ value
        /// </param>
        /// <param name='worldZ'>
        /// true = r
        /// </param>
        /// 
        public Vector3 GetTouchToWordlPoint(float z, bool worldZ = false)
        {
            if (!worldZ)
            {
                return Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, z));
            }
            else
            {
                return Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, z - Camera.main.transform.position.z));
            }
        }
        /// <summary>
        /// Gets the swipe or drag angle. (calculate from swipe Vector)
        /// </summary>
        /// <returns>
        /// Float : The swipe or drag angle.
        /// </returns>
        public float GetSwipeOrDragAngle()
        {
            return Mathf.Atan2(swipeVector.normalized.y, swipeVector.normalized.x) * Mathf.Rad2Deg;
        }
        /// <summary>
        /// Determines whether the touch is in a specified rect.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is in rect the specified rect; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='rect'>
        /// If set to <c>true</c> rect.
        /// </param>
        public bool IsInRect(Rect rect, bool guiRect = false)
        {
            if (guiRect)
            {
                rect = new Rect(rect.x, Screen.height - rect.y - rect.height, rect.width, rect.height);
            }

            return rect.Contains(position);
        }
        /// <summary>
        /// Normalizeds the position.
        /// </summary>
        /// <returns>
        /// The position.
        /// </returns>
        public Vector2 NormalizedPosition()
        {
            return new Vector2(100f / Screen.width * position.x / 100f, 100f / Screen.height * position.y / 100f);
        }

    }
}
