using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeLookAt : MonoBehaviour
{
    Transform _camera;
    [SerializeField] Vector3 _offset;

    public void OnEnable()
    {
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        if (_camera != null)
            transform.rotation = Quaternion.LookRotation(_camera.position - transform.position) * Quaternion.Euler(_offset);    }
}
