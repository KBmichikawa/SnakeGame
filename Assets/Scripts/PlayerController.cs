using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private Camera _mainCamera;
    private Vector3 _mouseInput = Vector3.zero;
    [SerializeField]
    float speed = 3.0f;

    private void Initialize()
    {
        _mainCamera = Camera.main;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }

    private void Update()
    {
        if (!IsOwner || !Application.isFocused) return;

        // move
        _mouseInput.x = Input.mousePosition.x;
        _mouseInput.y = Input.mousePosition.y;
        _mouseInput.z = _mainCamera.nearClipPlane;
        Vector3 mouseWorldCoordinates = _mainCamera.ScreenToViewportPoint(_mouseInput);
        transform.position = Vector3.MoveTowards(transform.position, mouseWorldCoordinates, Time.deltaTime * speed);

        // rot
        if (mouseWorldCoordinates != transform.position)
        {
            Vector3 targetDirection = mouseWorldCoordinates - transform.position;
            targetDirection.z = 0.0f;
            transform.up = targetDirection;

        }
    }
}
