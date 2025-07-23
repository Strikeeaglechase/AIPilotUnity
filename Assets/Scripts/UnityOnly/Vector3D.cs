using System;
using UnityEngine;

[Serializable]
public struct Vector3D
{
    public double x;

    public double y;

    public double z;

    public Vector3 toVector3 => new Vector3((float)x, (float)y, (float)z);

    public double magnitude => Math.Sqrt(sqrMagnitude);

    public double sqrMagnitude => x * x + y * y + z * z;

    public Vector3D normalized
    {
        get
        {
            double num = sqrMagnitude;
            if (num == 0.0)
            {
                return zero;
            }
            return new Vector3D(x, y, z) / Math.Sqrt(num);
        }
    }

    public static Vector3D zero => new Vector3D(0.0, 0.0, 0.0);

    public Vector3D(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3D(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vector3D operator +(Vector3D a, Vector3D b)
    {
        return new Vector3D(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vector3D operator +(Vector3D a, Vector3 b)
    {
        return new Vector3D(a.x + (double)b.x, a.y + (double)b.y, a.z + (double)b.z);
    }

    public static Vector3D operator -(Vector3D a, Vector3D b)
    {
        return new Vector3D(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vector3D operator -(Vector3D a, Vector3 b)
    {
        return new Vector3D(a.x - (double)b.x, a.y - (double)b.y, a.z - (double)b.z);
    }

    public static Vector3D operator *(Vector3D a, float c)
    {
        return (double)c * a;
    }

    public static Vector3D operator *(float c, Vector3D a)
    {
        return a * c;
    }

    public static Vector3D operator /(Vector3D a, float c)
    {
        return 1.0 / (double)c * a;
    }

    public static Vector3D operator *(Vector3D a, double c)
    {
        return new Vector3D(c * a.x, c * a.y, c * a.z);
    }

    public static Vector3D operator *(double c, Vector3D a)
    {
        return a * c;
    }

    public static Vector3D operator /(Vector3D a, double c)
    {
        return 1.0 / c * a;
    }

    public static Vector3D Lerp(Vector3D a, Vector3D b, double t)
    {
        double num = LerpD(a.x, b.x, t);
        double num2 = LerpD(a.y, b.y, t);
        double num3 = LerpD(a.z, b.z, t);
        return new Vector3D(num, num2, num3);
    }

    public static Vector3D Lerp(Vector3D a, Vector3D b, float t)
    {
        return Lerp(a, b, (double)t);
    }

    public static double LerpD(double a, double b, double t)
    {
        t = Clamp01(t);
        return a + (b - a) * t;
    }

    public static Vector3D MoveTowards(Vector3D a, Vector3D b, double maxDelta)
    {
        Vector3D vector3D = b - a;
        if (vector3D.sqrMagnitude > maxDelta * maxDelta)
        {
            return a + vector3D.normalized * maxDelta;
        }
        return b;
    }

    private static double Clamp01(double d)
    {
        if (d < 0.0)
        {
            return 0.0;
        }
        if (d > 1.0)
        {
            return 1.0;
        }
        return d;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1}, {2})", x.ToString("G17"), y.ToString("G17"), z.ToString("G17"));
    }
}
