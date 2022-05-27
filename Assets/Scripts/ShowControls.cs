using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowControls : MonoBehaviour
{

    public GameObject controls;
    public GameObject f1;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            controls.SetActive(false);
            f1.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            controls.SetActive(true);
            f1.SetActive(false);
        }
    }

}
