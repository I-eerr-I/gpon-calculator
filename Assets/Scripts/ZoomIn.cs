using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomIn : MonoBehaviour
{
    public float defaultZoom = 5f;
    public float minZoom     = 2f;
    public float maxZoom     = 15f;
    public float sensitivity = 1250f;

    private float zoom;

    void Start()
    {
        zoom = defaultZoom;
    }

    void Update()
    {
        Camera.main.orthographicSize = zoom;
    }

    void OnMouseOver()
    {
        zoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * sensitivity;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }    
}
