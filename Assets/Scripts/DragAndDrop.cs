using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{

    private Vector3 offset;
    private float   mouse_z_coord;
    private bool turned_off = false;

    void OnMouseDown() {
      mouse_z_coord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
      offset = gameObject.transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos() {
      Vector3 mousePoint = Input.mousePosition;
      mousePoint.z       = mouse_z_coord;
      return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag() {
      if (!turned_off) transform.position = GetMouseWorldPos() + offset;
    }

    public void TurnOn()
    {
        turned_off = false;
    }

    public void TurnOff()
    {
        turned_off = true;
    }
}
