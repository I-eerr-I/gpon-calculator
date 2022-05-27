using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ElementSpawner : MonoBehaviour, IPointerDownHandler 
{

    public GameObject toSpawn;


    private Transform field;
    private DragAndDrop field_dnd;

    private bool is_down;

    private Vector3 offset;
    private float mouse_z_coord;

    private GameObject tmp_element;

    void Start() 
    {
        field = GameObject.FindGameObjectWithTag("Background").transform;
        field_dnd = field.gameObject.GetComponent<DragAndDrop>();
    }

    void Update()
    {
        if (is_down)
        {
            if (Input.GetMouseButtonUp(0))
            {
                is_down = false;
                tmp_element = null;
                field_dnd.TurnOn();
            } 
            else
            {
                tmp_element.transform.position = GetMouseWorldPos();
            }
            
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        is_down       = true;
        field_dnd.TurnOff();
        tmp_element   = Instantiate(toSpawn, field);
        mouse_z_coord = Camera.main.WorldToScreenPoint(tmp_element.transform.position).z;
        offset = tmp_element.transform.position;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouse_z_coord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
