using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AirbaseBounds : MonoBehaviour
{
    public GameObject oref;

    private Vector3 center = new Vector3(288.73f, 0, -122.06f);
    private Vector3 extents = new Vector3(905.4f, 0, 1795.76f);
    private Vector3 Rotate(Vector3 pt)
    {
        return center + (transform.rotation * pt);
    }

    private Vector3 V3(Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

    private void OnDrawGizmos()
    {
        var quad = GetQuadPoints();

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + center, 10);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(V3(quad[0]), V3(quad[1]));
        Gizmos.DrawLine(V3(quad[1]), V3(quad[2]));
        Gizmos.DrawLine(V3(quad[2]), V3(quad[3]));
        Gizmos.DrawLine(V3(quad[3]), V3(quad[0]));

        Gizmos.DrawSphere(V3(quad[0]), 10);
        Gizmos.DrawSphere(V3(quad[1]), 10);
        Gizmos.DrawSphere(V3(quad[2]), 10);
        Gizmos.DrawSphere(V3(quad[3]), 10);

        if (oref != null)
        {
            var tp = new Vector2(oref.transform.position.x, oref.transform.position.z);
            var distance = DistanceToQuad(tp, quad);
            Debug.Log(distance);
        }
    }

    void Update() { }

    private Vector2[] GetQuadPoints()
    {
        var a = transform.position + Rotate(new Vector3(-extents.x / 2, 0, -extents.z / 2));
        var b = transform.position + Rotate(new Vector3(-extents.x / 2, 0, extents.z / 2));
        var c = transform.position + Rotate(new Vector3(extents.x / 2, 0, extents.z / 2));
        var d = transform.position + Rotate(new Vector3(extents.x / 2, 0, -extents.z / 2));


        return new Vector3[] { a, b, c, d }.Select(v => new Vector2(v.x, v.z)).ToArray();
    }

    private float DistanceToQuad(Vector2 point, Vector2[] quad)
    {
        return DistanceToQuad(point, quad[0], quad[1], quad[2], quad[3]);
    }
    private float DistanceToQuad(Vector2 point, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        var lines = new Vector2[][] { new Vector2[] { p0, p1 }, new Vector2[] { p1, p2 }, new Vector2[] { p2, p3 }, new Vector2[] { p3, p0 } };

        Gizmos.color = Color.blue;
        float minDistance = float.MaxValue;
        bool allPerp = true;

        foreach (var line in lines)
        {

            FindDistanceToSegment(point, line[0], line[1], out Vector2 closest, out bool isPerp);
            var distance = (closest - point).magnitude;
            if (distance < minDistance) minDistance = distance;
            if (!isPerp) allPerp = false;

            Gizmos.DrawLine(
                new Vector3(point.x, 0, point.y),
                new Vector3(closest.x, 0, closest.y)
                );
        }

        return minDistance * (allPerp ? -1 : 1);
    }

    private float FindDistanceToSegment(Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 closest, out bool isPerp)
    {
        isPerp = false;
        float dx = p2.x - p1.x;
        float dy = p2.y - p1.y;
        if ((dx == 0) && (dy == 0))
        {
            dx = pt.x - p1.x;
            dy = pt.y - p1.y;
            closest = p1;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        // Calculate the t that minimizes the distance.
        float t = ((pt.x - p1.x) * dx + (pt.y - p1.y) * dy) /
            (dx * dx + dy * dy);

        // See if this represents one of the segment's
        // end points or a point in the middle.
        if (t < 0)
        {
            closest = new Vector2(p1.x, p1.y);
            dx = pt.x - p1.x;
            dy = pt.y - p1.y;
        }
        else if (t > 1)
        {
            closest = new Vector2(p2.x, p2.y);
            dx = pt.x - p2.x;
            dy = pt.y - p2.y;
        }
        else
        {
            closest = new Vector2(p1.x + t * dx, p1.y + t * dy);
            dx = pt.x - p1.x + t * dx;
            dy = pt.y - p1.y + t * dy;
            isPerp = true;
        }

        return Mathf.Sqrt(dx * dx + dy * dy);
    }
}
