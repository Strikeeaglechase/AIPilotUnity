using UnityEngine;

public class TerrainCollider : MonoBehaviour
{
    private void FixedUpdate()
    {
        var height = Map.instance.GetHeightAtSubpoint(transform.position);
        if (transform.position.y <= height)
        {
            var damageRec = gameObject.GetComponentsImplementing<IDamageReceiver>();
            foreach (var dr in damageRec) dr.OnDamage();
        }
    }
}
