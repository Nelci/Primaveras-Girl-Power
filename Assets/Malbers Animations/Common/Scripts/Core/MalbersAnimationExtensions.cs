using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using MalbersAnimations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using System.Linq.Expressions;

public static class MalbersAnimationsExtensions
{
    /// <summary>
    /// Same as StartCoroutine but it also stores the coroytine in an IEnumerator
    /// </summary>
    public static void StartCoroutine(this MonoBehaviour Mono, out IEnumerator Cor, IEnumerator newCoro)
    {
        Cor = null;
        if (Mono.gameObject.activeInHierarchy)
        {
            Cor = newCoro;
            Mono.StartCoroutine(Cor);
        }
    }

    public static void Resize<T>(this List<T> list, int size, T element = default(T))
    {
        int count = list.Count;

        if (size < count)
        {
            list.RemoveRange(size, count - size);
        }
        else if (size > count)
        {
            if (size > list.Capacity)   // Optimization
                list.Capacity = size;

            list.AddRange(Enumerable.Repeat(element, size - count));
        }
    }


    public static int GetListenerNumber(this UnityEventBase unityEvent)
    {
        var field = typeof(UnityEventBase).GetField("m_Calls", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        var invokeCallList = field.GetValue(unityEvent);
        var property = invokeCallList.GetType().GetProperty("Count");
        return (int)property.GetValue(invokeCallList);
    }


    /// <summary> Find the first transform grandchild with this name inside this transform</summary>
    public static Transform FindGrandChild(this Transform aParent, string aName)
    {
        var result = aParent.ChildContainsName(aName);

        if (result != null) return result;

        foreach (Transform child in aParent)
        {
            result = child.FindGrandChild(aName);
            if (result != null)
                return result;
        }
        return null;
    }

    /// <summary> Find the closest transform from the origin </summary>
    public static Transform NearestTransform(this Transform t, params Transform[] transforms)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = t.position;
        foreach (Transform potentialTarget in transforms)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

    /// <summary> Calculate the Direction from an Origin to a Target or Destination  </summary>
    public static Vector3 DirectionTo(this Transform origin, Transform destination) => DirectionTo(origin.position, destination.position);
    /// <summary> Calculate the Direction from an Origin to a Target or Destination  </summary>
    public static Vector3 DirectionTo(this Transform origin, Vector3 destination) => DirectionTo(origin.position, destination);
    /// <summary> Calculate the Direction from an Origin to a Target or Destination  </summary>
    public static Vector3 DirectionTo(this Vector3 origin, Vector3 destination) => Vector3.Normalize(destination - origin);

    /// <summary> Find the farest transform from the origin </summary>
    public static Transform FarestTransform(this Transform t, params Transform[] transforms)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = t.position;
        foreach (Transform potentialTarget in transforms)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget > closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

    public static Transform ChildContainsName(this Transform aParent, string aName)
    {
        foreach (Transform child in aParent)
        {
            if (child.name.Contains(aName))
                return child;
        }
        return null;
    }

    /// <summary>The GameObject is a prefab, Meaning in not in any scene</summary>
    public static bool IsPrefab(this GameObject go) => !go.scene.IsValid();


    /// <summary>Do an action the next frame</summary>
    public static void Delay_Action(this MonoBehaviour mono, Action action) => mono.StartCoroutine(DelayedAction(1, action));

    /// <summary>Do an action the next given frames</summary>
    public static void Delay_Action(this MonoBehaviour mono, int frames, Action action) => mono.StartCoroutine(DelayedAction(frames, action));

    /// <summary>Do an action after certain time</summary>
    public static void Delay_Action(this MonoBehaviour mono, float time, Action action) => mono.StartCoroutine(DelayedAction(time, action));
    public static void Delay_Action(this MonoBehaviour mono, Func<bool> Condition, Action action) => mono.StartCoroutine(DelayedAction(Condition, action));
    public static void Delay_Action(this MonoBehaviour mono, WaitForSeconds time, Action action) => mono.StartCoroutine(DelayedAction(time, action));

    private static IEnumerator DelayedAction(int frame, Action action)
    {
        for (int i = 0; i < frame; i++)
            yield return null;

        action.Invoke();
    }

    private static IEnumerator DelayedAction(Func<bool> Condition, Action action)
    {
        yield return new WaitWhile(Condition);
        action.Invoke();
    }


    private static IEnumerator DelayedAction(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }

    private static IEnumerator DelayedAction(WaitForSeconds time, Action action)
    {
        yield return time;
        action.Invoke();
    }

    public static T FindComponent<T>(this GameObject c) where T: Component
    {
        T Ttt = c.GetComponent<T>();
        if (Ttt != null) return Ttt;

        Ttt = c.GetComponentInParent<T>();
        if (Ttt != null) return Ttt;

        Ttt = c.GetComponentInChildren<T>(true);
        if (Ttt != null) return Ttt;

        return default;
    }

    public static T[] FindComponents<T>(this GameObject c) where T : Component
    {
        T[] Ttt = c.GetComponents<T>();
        if (Ttt != null) return Ttt;

        Ttt = c.GetComponentsInParent<T>();
        if (Ttt != null) return Ttt;

        Ttt = c.GetComponentsInChildren<T>(true);
        if (Ttt != null) return Ttt;

        return default;
    }



    /// <summary>Search for the Component in the root of the Object </summary>
    public static T FindComponentInRoot<T>(this GameObject c) where T : Component
    {
        var root = c.transform.root;
        T Ttt = root.GetComponent<T>();
        if (Ttt != null) return Ttt;

        Ttt = root.GetComponentInChildren<T>(true);
        if (Ttt != null) return Ttt;

        return default;
    }


    public static T FindInterface<T>(this GameObject c)
    {
        T Ttt = c.GetComponent<T>();
        if (Ttt != null) return Ttt;

        Ttt = c.GetComponentInParent<T>();
        if (Ttt != null) return Ttt;

        Ttt = c.GetComponentInChildren<T>(true);
        if (Ttt != null) return Ttt; 

        return default;
    }

    public static T[] FindInterfaces<T>(this GameObject c)
    {
        T[] Ttt = c.GetComponents<T>();
        if (Ttt != null) return Ttt;

        Ttt = c.GetComponentsInParent<T>();
        if (Ttt != null) return Ttt;

        Ttt = c.GetComponentsInChildren<T>(true);
        if (Ttt != null) return Ttt;

        return default;
    }

    /// <summary>Parent a transform to another Transform, and Solves the Scale problem in case the Parent has a deformed scale  </summary>
    /// <param name="parent">Transform to be the parent</param>
    /// <param name="Position">Relative position to the Parent (World Position)</param>
    public static void SetParentScaleFixer(this Transform transform, Transform parent, Vector3 Position)
    {
        if (parent.lossyScale.x == parent.lossyScale.y && parent.lossyScale.x == parent.lossyScale.z) //Check if the Scale is Uniform
        {
            transform.SetParent(parent, true);
            transform.position = Position;
            return;
        }

        Vector3 NewScale = parent.transform.lossyScale;
        NewScale.x = 1f / Mathf.Max(NewScale.x, 0.00001f);
        NewScale.y = 1f / Mathf.Max(NewScale.y, 0.00001f);
        NewScale.z = 1f / Mathf.Max(NewScale.z, 0.00001f);

        GameObject Hlper = new GameObject   { name = transform.name + "Link" };

        Hlper.transform.SetParent(parent);
        Hlper.transform.localScale = NewScale;
        Hlper.transform.position = Position;
        Hlper.transform.localRotation = Quaternion.identity;

        transform.SetParent(Hlper.transform);
        transform.localPosition = Vector3.zero;
    }

    /// <summary>Search for the Component in the hierarchy Up or Down</summary>
    public static T FindComponent<T>(this Component c) where T : Component => c.gameObject.FindComponent<T>();

    public static T FindInterface<T>(this Component c) => c.gameObject.FindInterface<T>();
    public static T[] FindInterfaces<T>(this Component c) => c.gameObject.FindInterfaces<T>();
     
   

    /// <summary>Search for the Component in the root of the Object </summary>
    public static T FindComponentInRoot<T>(this Component c) where T : Component => c.gameObject.FindComponentInRoot<T>();


    /// <summary>returns the delta position from a rotation.</summary>
    public static Vector3 DeltaPositionFromRotate(this Transform transform, Vector3 point, Vector3 axis, float deltaAngle)
    {
        var pos = transform.position;
        var direction = pos - point;
        var rotation = Quaternion.AngleAxis(deltaAngle, axis);
        direction = rotation * direction;

        pos = point + direction - pos;
        pos.y = 0;                                                      //the Y is handled by the Fix Position method

        return pos;
    }

    /// <summary>returns the delta position from a rotation.</summary>
    public static Vector3 DeltaPositionFromRotate(this Transform transform, Transform platform, Quaternion deltaRotation)
    {
        var pos = transform.position;

        var direction = pos - platform.position;
        var directionAfterRotation = deltaRotation * direction;

        var NewPoint = platform.position + directionAfterRotation;


        pos = NewPoint - transform.position;

        return pos;
    }


    /// <summary>
    /// Checks if a GameObject has been destroyed.
    /// </summary>
    /// <param name="gameObject">GameObject reference to check for destructedness</param>
    /// <returns>If the game object has been marked as destroyed by UnityEngine</returns>
    public static bool IsDestroyed(this GameObject gameObject)
    {
        // UnityEngine overloads the == opeator for the GameObject type
        // and returns null when the object has been destroyed, but 
        // actually the object is still there but has not been cleaned up yet
        // if we test both we can determine if the object has been destroyed.
        return gameObject == null && !ReferenceEquals(gameObject, null);
    }


    /// <summary>Resets the Local Position and rotation of a transform</summary>

    public static void ResetLocal(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    /// <summary>Resets the Local Position and rotation of a transform</summary>
    public static void SetLocalTransform(this Transform transform, Vector3 LocalPos, Vector3 LocalRot, Vector3 localScale)
    {
        transform.localPosition = LocalPos;
        transform.localEulerAngles = LocalRot;
        transform.localScale = localScale;
    }

    /// <summary>Resets the Local Position and rotation of a transform</summary>
    public static void SetLocalTransform(this Transform transform, TransformOffset offset)
    {
        transform.localPosition = offset.Position;
        transform.localEulerAngles = offset.Rotation;
        transform.localScale = offset.Scale;
    }

    public static UnityAction<T> CreateDelegate<T>(object target, MethodInfo method)
    {
        var del = (UnityAction<T>)Delegate.CreateDelegate(typeof(UnityAction<T>), target, method);
        return del;
    }

    public static UnityAction CreateDelegate(object target, MethodInfo method)
    {
        var del = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, method);
        return del;
    }
    /// <summary>
    /// Returns a Unity Action from a component and a method. Used to connect methods in the inspector
    /// </summary>
    public static UnityAction GetUnityAction(this Component owner, string classtype, string method)
    {
        var sender = owner.GetComponent(classtype);
        MethodInfo methodPtr;

        if (sender != null)
        {
            methodPtr = sender.GetType().GetMethod(method, new Type[0]);
        }
        else return null;

        if (methodPtr != null)
        {
           // Debug.Log("methodPtr = " + methodPtr.Name);
            var action = CreateDelegate(sender, methodPtr);
            return (action);
        }
       
        return null;
    }

    public static T GetFieldClass<T>(this Component owner, string component, string field) where T : class
    {
        var sender = owner.GetComponent(component);

        if (sender != null)
        {
            FieldInfo methodPtr = sender.GetType().GetField(field, BindingFlags.Public | BindingFlags.Instance);

            if (methodPtr != null)
            {
                return methodPtr.GetValue(sender) as T;
            }
        }
        return null;
    } 
    

    public static UnityAction<T> GetUnityAction<T>(this Component c, string component, string method)
    {
        var sender = c.GetComponent(component);

        if (sender == null) return null;
        
         var methodPtr = sender.GetType().GetMethod(method, new Type[] { typeof(T) });

        if (methodPtr != null)
        {
          //  Debug.Log("methodPtr = " + methodPtr.Name);
            var action = CreateDelegate<T>(sender, methodPtr);
            return (action);
        }

        PropertyInfo property = sender.GetType().GetProperty(method);

        if (property != null)
        {
          //  Debug.Log("property = " + property.Name);
            var action = CreateDelegate<T>(sender, property.SetMethod);
            return (action);
        }

        return null;
    }

    /// <summary> Invoke with Parameters </summary>
    public static bool InvokeWithParams(this MonoBehaviour sender, string method, object args)
    {
        Type argType = null;

        if (args != null) argType = args.GetType();
      

        MethodInfo methodPtr = null;

        if (argType != null)
        {
            methodPtr = sender.GetType().GetMethod(method, new Type[] { argType });
        }
        else
        {
            methodPtr = sender.GetType().GetMethod(method);
        }

        if (methodPtr != null)
        {
            if (args != null)
            {
                var arguments = new object[1] { args };
                methodPtr.Invoke(sender, arguments);
                return true;
            }
            else
            {
                methodPtr.Invoke(sender, null);
                return true;
            }
        }

        PropertyInfo property = sender.GetType().GetProperty(method);

        if (property != null)
        {
            property.SetValue(sender, args, null);
            return true;

        }
        return false;
    }


    /// <summary>Invoke with Parameters and Delay </summary>
    public static void InvokeDelay(this MonoBehaviour behaviour, string method, object options, YieldInstruction wait)
    {
        behaviour.StartCoroutine(_invoke(behaviour, method, wait, options));
    }

    private static IEnumerator _invoke(this MonoBehaviour behaviour, string method, YieldInstruction wait, object options)
    {
        yield return wait;

        Type instance = behaviour.GetType();
        MethodInfo mthd = instance.GetMethod(method);
        mthd.Invoke(behaviour, new object[] { options });

        yield return null;
    }


    /// <summary>Invoke with Parameters for Scriptable objects</summary>
    public static void Invoke(this ScriptableObject sender, string method, object args)
    {
        var methodPtr = sender.GetType().GetMethod(method);

        if (methodPtr != null)
        {
            if (args != null)
            {
                var arguments = new object[1] { args };
                methodPtr.Invoke(sender, arguments);
            }
            else
            {
                methodPtr.Invoke(sender, null);
            }
        }
    }

    /// <summary> Changes the Layer of a GameObject and its children.  </summary>
    public static void SetLayer(this GameObject parent, int layer, bool includeChildren = true)
    {
        parent.layer = layer;
        if (includeChildren)
        {
            foreach (var trans in parent.transform.GetComponentsInChildren<Transform>(true))
                trans.gameObject.layer = layer;
        }
    }



    /// <summary>Enable disable the Mono</summary>
    public static void SetEnable(this MonoBehaviour c, bool enable) => c.enabled = enable;
    /// <summary>Enable disable the Mono</summary>
    public static void SetEnable(this Collider c, bool enable) => c.enabled = enable;



    /// --------------- EDITOR EXTENSIONS -------------------------

#if UNITY_EDITOR

#endif

}