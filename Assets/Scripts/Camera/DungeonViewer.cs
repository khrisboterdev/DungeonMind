using UnityEngine;
using UnityEngine.InputSystem;

public class DungeonViewer : MonoBehaviour
{
    private Camera _camera;

    // Input Actions
    private InputAction _moveAction;
    private InputAction _zoomAction;

    [Header("Camera Settings")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _smooth = 15f;
    [SerializeField] private float _minZoom = 3f;
    [SerializeField] private float _maxZoom = 15f;
    [SerializeField] private float _zoomSpeed = 5f;

    // Input variables
    private Vector2 _moveInput;
    private float _zoomInput;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        _moveAction = InputSystem.actions.FindAction(InputConstants.MOVE);
        _zoomAction = InputSystem.actions.FindAction(InputConstants.ZOOM);
    }

    private void LateUpdate()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        _zoomInput = _zoomAction.ReadValue<Vector2>().y;
    }

    private void FixedUpdate()
    {
        MoveCamera(_moveInput);

        if (_zoomInput != 0)
        {
            AdjustZoom(_zoomInput);
        }
    }

    private void MoveCamera(Vector2 dir)
    {
        Vector3 newPosition = transform.position;
        newPosition.x += dir.x * _moveSpeed;
        newPosition.y += dir.y * _moveSpeed;
        newPosition.z = -10;

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * _smooth);
    }

    private void AdjustZoom(float zoomInput)
    {
        float newSize = _camera.orthographicSize - zoomInput * _zoomSpeed;
        _camera.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);     
    }
}
