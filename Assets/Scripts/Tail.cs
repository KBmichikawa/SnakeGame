using UnityEngine;

public class Tail : MonoBehaviour
{
    public Transform networkedOwner;
    public Transform followTransform;

    [SerializeField] private float delayTime = 0.1f;
    [SerializeField] private float distance = 0.3f;
    [SerializeField] private float moveStep = 10.0f;

    private Vector3 _targetPosition;

    private void Update()
    {
        if (networkedOwner == null) return;
        if (followTransform == null) return;

        _targetPosition = followTransform.position - followTransform.forward * distance;
        _targetPosition += (transform.position - _targetPosition) * delayTime;
        _targetPosition.z = 0f;

        transform.position = Vector3.Lerp(transform.position, _targetPosition, moveStep * Time.deltaTime);
    }
}
