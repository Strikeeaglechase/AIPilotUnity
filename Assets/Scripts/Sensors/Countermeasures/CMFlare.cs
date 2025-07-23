using Recorder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMFlare : MonoBehaviour
{
    public float heatEmission;
    public HeatEmitter heatEmitter;
    public float gravFactor;
    public Vector3 velocity;
    public float drag = 0.2f;
    public float flareLife = 7f;
    private float finalEmission;

    private void Start()
    {
        StartCoroutine(LifeRoutine());
    }

    private IEnumerator LifeRoutine()
    {
        finalEmission = heatEmission;
        yield return new WaitForSeconds(flareLife);
        finalEmission = 0f;

        Destroy(gameObject);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        velocity += Time.fixedDeltaTime * gravFactor * Physics.gravity;
        velocity += Time.fixedDeltaTime * drag * -velocity;
        heatEmitter.velocity = velocity;
        heatEmitter.AddHeat(finalEmission * Time.fixedDeltaTime);
        transform.position += velocity * Time.fixedDeltaTime;
    }
}
