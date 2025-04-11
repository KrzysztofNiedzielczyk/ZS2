using UnityEngine;

public class MousePosition : MonoBehaviour
{
    //the layers the ray can hit
    [SerializeField] private LayerMask _hitLayers;

    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;

        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, _hitLayers))
        {
            if(hit.transform.TryGetComponent(out IAimable aimable))
            {
                transform.position = aimable.AimTransform.position;
            }
            else
            {
                transform.position = hit.point;
            }
        }
    }
}