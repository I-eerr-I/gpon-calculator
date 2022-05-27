using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPaste : MonoBehaviour
{
    private Transform field;

    private GameObject saved = null;
    private bool in_save = false;

    void Start()
    {
        field = GameObject.FindWithTag("Background").transform;
    }

    void Update()
    {
        bool is_control = Input.GetKey(KeyCode.LeftControl);
        if (is_control && Input.GetKeyDown(KeyCode.C))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.gameObject.layer == 8 && hit.transform.gameObject.tag != "OLT")
                {
                    saved = hit.transform.gameObject;
                    in_save = true;
                }
            }
        }

        if (is_control && Input.GetKeyDown(KeyCode.V) && in_save)
        {
            GameObject pasted = Instantiate(saved, field);
            pasted.GetComponent<ElementController>().CopySettings(saved); 
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.y = 0.5f;
            pasted.transform.position = position;
        }
    }
}
