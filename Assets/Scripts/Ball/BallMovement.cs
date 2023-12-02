using UnityEngine;

public class BallMovement : MonoBehaviour
{
    Rigidbody _rigiBody;
    [SerializeField] float speed;

    private void Awake()
    {
        _rigiBody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 direction)
    {
        direction *= speed;
        direction.y = _rigiBody.velocity.y;
        _rigiBody.velocity = direction;
    }
}
