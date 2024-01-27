using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOTSInMars
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed = 5;
        [SerializeField] private float _zoomSpeed = 5;
        [SerializeField] private float _yMaxZoom = 15;
        [SerializeField] private float _yMinZoom = 2;
        [SerializeField] private Transform _camera;

        void Update()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var combined = new Vector2(horizontal, vertical);
            if (combined.magnitude > 1)
            {
                combined = combined.normalized;
            }

            transform.position = transform.position + new Vector3(combined.x, 0, combined.y) * _movementSpeed * Time.deltaTime;

            var zoom = -Input.GetAxis("Mouse ScrollWheel");
            if (transform.position.y >= _yMaxZoom)
            {
                zoom = Mathf.Min(zoom, 0);
            }
            else if (transform.position.y <= _yMinZoom)
            {
                zoom = Mathf.Max(zoom, 0);
            }
            transform.position = transform.position + _camera.forward * -_zoomSpeed * zoom * Time.deltaTime; 
        }
    }
}