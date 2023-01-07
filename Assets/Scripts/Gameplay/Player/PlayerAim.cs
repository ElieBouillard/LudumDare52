using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Transform _camera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, Mathf.Infinity);
            Debug.DrawRay(hit.point, Vector3.up * 5f, Color.red, 5f);
        }
    }
}
