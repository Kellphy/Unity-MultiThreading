using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DummySpawner;

public static class ColliderPhyiscs
{
    public static AABB Box(GameObject goToBox)
    {
        AABB box;
        box.max = goToBox.transform.position +
            new Vector3(goToBox.transform.localScale.x / 2, goToBox.transform.localScale.y / 2, goToBox.transform.localScale.z / 2);
        box.min = goToBox.transform.position +
            new Vector3(-goToBox.transform.localScale.x / 2, -goToBox.transform.localScale.y / 2, -goToBox.transform.localScale.z / 2);
        return box;
    }

    public static bool Collide(GameObject first, GameObject second)
    {
        AABB box1;
        box1.max = first.transform.position +
            new Vector3(first.transform.localScale.x / 2, first.transform.localScale.y / 2, first.transform.localScale.z / 2);
        box1.min = first.transform.position +
            new Vector3(-first.transform.localScale.x / 2, -first.transform.localScale.y / 2, -first.transform.localScale.z / 2);

        AABB box2;
        box2.max = second.transform.position +
            new Vector3(second.transform.localScale.x / 2, second.transform.localScale.y / 2, second.transform.localScale.z / 2);
        box2.min = second.transform.position +
            new Vector3(-second.transform.localScale.x / 2, -second.transform.localScale.y / 2, -second.transform.localScale.z / 2);

        return Intersect(box1, box2);
    }

    public static bool TestAABBAABB(AABB a, AABB b)
    {
        if (a.max.x < b.min.x || a.min.x > b.max.x) return false;
        if (a.max.y < b.min.y || a.min.y > b.max.y) return false;
        if (a.max.z < b.min.z || a.min.z > b.max.z) return false;
        return true;
    }

    public static bool Intersect(AABB box, Ray ray)
    {
        float tx1 = (box.min.x - ray.origin.x) * (1 / ray.direction.x);
        float tx2 = (box.max.x - ray.origin.x) * (1 / ray.direction.x);

        float tmin = Mathf.Min(tx1, tx2);
        float tmax = Mathf.Min(tx1, tx2);

        float ty1 = (box.min.y - ray.origin.y) * (1 / ray.direction.y);
        float ty2 = (box.max.y - ray.origin.y) * (1 / ray.direction.y);

        tmin = Mathf.Max(tmin, Mathf.Min(ty1, ty2));
        tmax = Mathf.Min(tmax, Mathf.Max(ty1, ty2));

        float tz1 = (box.min.z - ray.origin.z) * (1 / ray.direction.z);
        float tz2 = (box.max.z - ray.origin.z) * (1 / ray.direction.z);

        tmin = Mathf.Max(tmin, Mathf.Min(tz1, tz2));
        tmax = Mathf.Min(tmax, Mathf.Max(tz1, tz2));

        return tmax >= tmin;
    }

    public static bool Intersect(AABB box1, AABB box2)
    {
        return (box1.min.x <= box2.max.x && box1.max.x >= box2.min.x) &&
         (box1.min.y <= box2.max.y && box1.max.y >= box2.min.y) &&
         (box1.min.z <= box2.max.z && box1.max.z >= box2.min.z);
    }

    public static Vector3 PointOfContact(Vector3 point, GameObject b)
    {
        AABB box;
        box.max = b.transform.position +
            new Vector3(b.transform.localScale.x / 2, b.transform.localScale.y / 2, b.transform.localScale.z / 2);
        box.min = b.transform.position +
            new Vector3(-b.transform.localScale.x / 2, -b.transform.localScale.y / 2, -b.transform.localScale.z / 2);

        return PointOfContact(point, box);
    }

    public static Vector3 PointOfContact(Vector3 point, AABB b)
    {
        Vector3 v = point;

        if (v.x < b.min.x) v.x = b.min.x; // v = max(v, b.min[i])
        if (v.x > b.max.x) v.x = b.max.x; // v = min(v, b.max[i])

        if (v.y < b.min.y) v.y = b.min.y;
        if (v.y > b.max.y) v.y = b.max.y;

        if (v.z < b.min.z) v.z = b.min.z;
        if (v.z > b.max.z) v.z = b.max.z;

        return v;
    }
}
