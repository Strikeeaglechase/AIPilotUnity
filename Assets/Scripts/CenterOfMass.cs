using UnityEngine;

[ExecuteAlways]
public class CenterOfMass : MonoBehaviour
{
    private void OnEnable()
    {
        Rigidbody rb = GetComponentInParent<Rigidbody>();
        rb.automaticCenterOfMass = false;
        rb.centerOfMass = rb.transform.InverseTransformPoint(base.transform.position);
    }
}
