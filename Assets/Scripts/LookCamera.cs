using UnityEngine;

public class LookCamera : MonoBehaviour
{
    [SerializeField] private Transform _mainCamera;
    [SerializeField] private Transform _nameTransform;

    private void Start()
    {
        _mainCamera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(_nameTransform.position + _mainCamera.forward);
    }
}
