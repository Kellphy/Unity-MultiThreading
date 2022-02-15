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

    public static Sphere Sphere(GameObject goToSphere)
    {
        Sphere sphere;
        sphere.position = goToSphere.transform.position;
        sphere.radius = goToSphere.transform.localScale.magnitude;

        return sphere;
    }

    public static bool Collide(Shape first, Shape second)
    {
        if (first.isBox)
        {
            AABB box1 = first.box;
            if (second.isBox)
            {
                AABB box2 = second.box;
                return AABBtoAABB(box1, box2);
            }
            else
            {
                Sphere sphere2 = second.sphere;
                return SphereToBox(sphere2, box1);
            }
        }
        else
        {
            Sphere sphere1 = first.sphere;
            if (second.isBox)
            {
                AABB box2 = second.box;
                return SphereToBox(sphere1, box2);
            }
            else
            {
                Sphere sphere2 = second.sphere;
                return SpheretoSphere(sphere1, sphere2);
            }
        }
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

    public static bool AABBtoAABB(AABB box1, AABB box2)
    {
        return (box1.min.x <= box2.max.x && box1.max.x >= box2.min.x) &&
         (box1.min.y <= box2.max.y && box1.max.y >= box2.min.y) &&
         (box1.min.z <= box2.max.z && box1.max.z >= box2.min.z);
    }

    public static bool SpheretoSphere(Sphere sphere, Sphere other)
    {
        var distance = Mathf.Sqrt((sphere.position.x - other.position.x) * (sphere.position.x - other.position.x) +
            (sphere.position.y - other.position.y) * (sphere.position.y - other.position.y) +
            (sphere.position.z - other.position.z) * (sphere.position.z - other.position.z));

        return distance < (sphere.radius + other.radius);
    }

    public static bool BoxToSphere(AABB box, Sphere sphere)
    {
        return SphereToBox(sphere, box);
    }

    public static bool SphereToBox(Sphere sphere, AABB box)
    {
        var x = Mathf.Max(box.min.x, Mathf.Min(sphere.position.x, box.max.x));
        var y = Mathf.Max(box.min.y, Mathf.Min(sphere.position.y, box.max.y));
        var z = Mathf.Max(box.min.z, Mathf.Min(sphere.position.z, box.max.z));

        var distance = Mathf.Sqrt((x - sphere.position.x) * (x - sphere.position.x) +
                         (y - sphere.position.y) * (y - sphere.position.y) +
                         (z - sphere.position.z) * (z - sphere.position.z));

        return distance < sphere.radius;
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
