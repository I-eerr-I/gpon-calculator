using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayResult : MonoBehaviour
{

    public TextMeshPro text;

    private ElementController olt;
    private ElementController ont;

    private float power_budget;
    private float max_length;

    void Start()
    {
        olt = GameObject.FindWithTag("OLT").GetComponent<ElementController>();
        ont = GetComponent<ElementController>();
    }

    void Update()
    {
        power_budget = (float)(olt.GetPower() - ont.GetONTType());
        if (ont.IsWithInput())
            max_length = (power_budget - (ont.GetONTAttenuation() - ont.GetCableIndex() * 0.33f)) / 0.33f;
        else
            max_length = 0.0f;
        UpdateText();  
    }

    void UpdateText()
    {
        text.SetText("Power: {0:3} dB\nAttenuation: {1:3} dB\nPower budget: {2} dB\nMax length: {3:3} km", ont.GetPower(), ont.GetONTAttenuation() + 3f, power_budget, max_length);
    }

}
