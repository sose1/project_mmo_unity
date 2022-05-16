using Network.Models.Other;
using TMPro;
using UnityEngine;

public class RemotePlayerController : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    public float turnTime = 2000f;
    public float smoothTime = 0.1f;
    private Vector3 _lastPosition;
    private Vector3 _endPosition;
    private Vector3 _endRotation;

    void Start()
    {
        _lastPosition = transform.position;
        _endPosition = transform.eulerAngles;
    }

    private void Update()
    {
        if (Camera.main is { }) GetComponentInChildren<TextMeshPro>().transform.LookAt(Camera.main.transform);
        GetComponentInChildren<TextMeshPro>().transform.Rotate(0, 180, 0);
      
    }
    
    private void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(_lastPosition, _endPosition, ref velocity, smoothTime * Time.fixedDeltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(_endRotation), turnTime * Time.fixedDeltaTime);
    }

    public void Move(Position position)
    {
        var move = new Vector3(position.x, position.y, position.z);
        _endPosition = move;
        _lastPosition = transform.position;
        _endRotation = new Vector3(transform.eulerAngles.x, position.rotation, transform.eulerAngles.z);
    }
}