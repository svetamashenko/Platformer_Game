using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private int _speed;

    private Vector2 _direction;
    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
    }

    private void Update()
    {
        if (_direction != new Vector2(0, 0))
        {
            var delta = _speed * Time.deltaTime * _direction;
            var new2DPos = new Vector2(transform.position.x, transform.position.y) + delta;
            transform.position = new Vector3(new2DPos.x, new2DPos.y, transform.position.z);
        }
    }

    public void SaySomething()
    {
        Debug.Log("Something...");
    }
}
