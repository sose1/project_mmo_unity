using System;
using System.Numerics;
using Cinemachine;
using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class RemotePlayerController : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    private Vector3 _lastPosition;
    private Vector3 _endPosition;

    void Start()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        if (Camera.main is { }) GetComponentInChildren<TextMeshPro>().transform.LookAt(Camera.main.transform);
        GetComponentInChildren<TextMeshPro>().transform.Rotate(0, 180, 0);
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