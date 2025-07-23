using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class SOCurve : ScriptableObject
{
    public AnimationCurve curve;

    public float Evaluate(float t)
    {
        return curve.Evaluate(t);
    }
}