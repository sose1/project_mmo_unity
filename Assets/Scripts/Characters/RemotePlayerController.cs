using System;
using UnityEngine;

public class RemotePlayerController : MonoBehaviour
{
    private Vector3 _lastPosition;
    private Vector3 _endPosition;
    public Vector3 velocity = Vector3.zero;
    void Start()
    {
        _lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(_lastPosition, _endPosition, ref velocity, 0.5f * Time.smoothDeltaTime);
    }

    public void Move(Vector3 position)
    {
        _endPosition = position;
        _lastPosition = transform.position;
    }
}