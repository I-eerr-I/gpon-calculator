using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterField : MonoBehaviour
{

    public float interpolation = 0.25f;

    private bool pressed = false;

    void Update()
    {
        if (pressed)
            transform.position = Vector3.Lerp(transform.position, Vector3.zero, interpolation*Time.deltaTime);
        if (Vector3.Distance(transform.position, Vector3.zero) < 0.5f)
            pressed = false;
    }

    void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            pressed = true;
        }
    }

    void OnMouseDrag()
    {
        pressed = false;
    }
}
