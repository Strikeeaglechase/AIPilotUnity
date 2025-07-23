using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinecastTest : MonoBehaviour
{
    public Transform pt1;
    public Transform pt2;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pt1.position, 10);
        Gizmos.DrawWireSphere(pt2.position, 10);

        // Map.instance.Linecast(pt1.position, pt2.position);
    }
}
