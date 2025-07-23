using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDrag : MonoBehaviour
{
    public float area;
    public Rigidbody rb;

    private float CalculateDragForceMagnitude(float spd)
    {
        return 0.5f * AerodynamicsController.fetch.AtmosDensityAtPosition(transform.position) * area * spd * AerodynamicsController.fetch.DragMultiplierAtSpeed(spd, transform.position);
    }


    void FixedUpdate()
    {
        if (!rb) return;

        if (area > 0f)
        {
            Vector3 com = rb.worldCenterOfMass;
            Vector3 airflow = rb.GetPointVelocity(com);
            float dragForce = CalculateDragForceMagnitude(airflow.magnitude);
            //Debug.Log($"DF: {(-airflow * dragForce).magnitude:f2}");
            if (!rb.isKinematic)
            {
                Vector3 force = -airflow * dragForce;
                if (!float.IsNaN(force.x))
                {
                    rb.AddForceAtPosition(force, com);
                }
            }
        }
    }
}
