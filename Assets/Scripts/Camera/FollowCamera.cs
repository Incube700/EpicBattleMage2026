using UnityEngine;

[DefaultExecutionOrder(100)]
public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 12f, -10f);
    [SerializeField] private float _smoothTime = 0.12f;
    [SerializeField] private bool _lockYRotation = true; // для топдауна: не крутить камеру вокруг вертикали
    [SerializeField] private float _pitch = 35f; // наклон камеры вниз для арены

    private Vector3 _velocity;

    private void LateUpdate()
    {
        if (_target == null) return;

        var desired = _target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, _smoothTime);

        var rot = Quaternion.Euler(_pitch, _lockYRotation ? 0f : transform.eulerAngles.y, 0f);
        transform.rotation = rot;
    }

    public void SetTarget(Transform t) => _target = t;
}
