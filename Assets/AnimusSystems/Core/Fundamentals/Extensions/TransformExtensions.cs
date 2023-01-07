using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions {

    public static Transform FindBrotherWithTag(this Transform transform, params string[] tags)
    {
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            var child = transform.parent.GetChild(i);
            for (int t = 0; t < tags.Length; t++) if (child.CompareTag(tags[t])) return child;
        }
        return null;
    }
    public static Transform FindChildWithTag(this Transform transform, params string[] tags)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            for (int t = 0; t < tags.Length; t++) if (child.CompareTag(tags[t])) return child;
        }
        return null;
    }
    public static int GetDepth(this Transform transform)
    {
        if (transform == null) return 0;
        int i = 0;
        var T = transform;
        while (T.parent!=null)
        {
            i++;
            T = T.parent;
        }
        return i;
    }
    public static Transform FindRootParent(this Transform transform)
    {
        if (transform.parent == null) return null;
        var parent = transform.parent;
        while (parent.parent != null) parent = parent.parent;
        return parent;
    }
    public static T GetComponentInChildrenNoRecursion<T>(this Transform t) where T : Component
    {
        for (int i=0; i<t.childCount;i++)
        {
            var Comp = t.GetChild(i).GetComponent<T>();
            if (Comp) return Comp;
        }
        return null;
    }
    public static void FollowToTarget(this Transform t, Transform Target, float Force, bool WorldSpace)
    {
        if (WorldSpace)
        {
            t.position = Vector3.Lerp(t.position, Target.position, Force);
            t.rotation = Quaternion.Slerp(t.rotation, Target.rotation, Force);
        } else
        {
            t.localPosition = Vector3.Lerp(t.localPosition, Target.localPosition, Force);
            t.localRotation = Quaternion.Slerp(t.localRotation, Target.localRotation, Force);
        }
    }
    public static Transform FindByPath(this Transform t, string path) {
        if (path.StartsWith("/"))
        {
            return GameObject.Find(path.Substring(1)).transform;
        }
        var Path = path.Split('/');
        var target = t;
        foreach (var dir in Path)
        {
            if (dir == "..") target = target.parent;
            else target = target.Find(dir);
            if (target == null) return null;
        }
        return target;
    }
}
