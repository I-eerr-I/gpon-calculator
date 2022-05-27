using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Connection
{
    public GameObject cable;
    public GameObject source;
    public GameObject target;
    public LineRenderer lineRenderer;
    public ElementController sourceController;
    public ElementController targetController;

    public Connection(GameObject source, GameObject cable, GameObject target)
    {
        this.cable       = cable;
        this.source      = source;
        this.target      = target;
        lineRenderer     = cable.GetComponent<LineRenderer>();
        sourceController = source.GetComponent<ElementController>();
        targetController = target.GetComponent<ElementController>();
    }
}

public class ElementController : MonoBehaviour
{

    public int   maximumOutputs;
    public GameObject cablePrefab;
    
    // Input and outputs
    private Connection input;
    private bool with_input;
    private List<Connection> outputs  = new List<Connection>();

    // Temporary data
    private GameObject tmp_cable = null;
    private LineRenderer tmp_lr  = null;
    private bool holding_cable   = false;

    // Wires color and type
    private Color over_connector          = new Color(1.0f, 1.0f, 0.0f, 1.0f);
    private Color over_welding            = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    private bool output_is_over_connector = true;
    private bool input_is_over_connector  = true;

    // Main text
    private TextMeshPro text;
    private string element_name;

    // Element types
    private int fbt_type             = 50;
    private int plc_type             = 6;
    private int[] plc_types          = { 2, 3, 4, 6, 8, 12, 16, 24, 32, 64, 128 };
    private float[] plc_attenuations = { 4.3f, 6.2f, 7.4f, 9.5f, 10.7f, 12.5f, 13.9f, 16.0f, 17.2f, 21.5f, 25.5f };
    private int olt_type             = 2;
    private int ont_type             = -27;

    // Main characteristics
    private float power;
    private float attenuation;
    private int   cable_index;

    // Helping variables
    private bool on_me;


    void Start()
    {
        text         = GetComponentInChildren<TextMeshPro>();
        element_name = text.text.Split('\n')[0];
        ChangeText();
        
        if (gameObject.tag == "FBT") CorrectFBTType();
        if (gameObject.tag == "PLC") CorrectPLCType();
        if (gameObject.tag == "OLT") CorrectOLTType();
        if (gameObject.tag == "ONT") CorrectONTType();
        if (gameObject.tag == "PLC") maximumOutputs = plc_types[plc_type];
    }

    void Update()
    {
        DeleteElement();
        ChangeType();
        DrawCables();
        ControllInput();
        ControllOutput();
        CalculateCharacteristics();
    }

    void DeleteElement()
    {
        if (gameObject.tag == "OLT") return;
        if (on_me && (Input.GetKey(KeyCode.Delete) || Input.GetKey(KeyCode.R)))
        {
            foreach (Connection output in outputs)
            {
                output.targetController.NullInput();
                Destroy(output.cable);
            }
            if (with_input) input.sourceController.DestroyOutput(gameObject);
            Destroy(gameObject);
        }
    }

    void ChangeType()
    {
        float change = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(change) > 0 && on_me)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                input_is_over_connector = !input_is_over_connector;
            }
            else if (Input.GetKey(KeyCode.LeftAlt))
            {
                int pom = (change > 0 ? 1 : -1);
                if (gameObject.tag == "FBT")
                {
                    fbt_type += 5 * pom;
                    CorrectFBTType();
                }
                else if (gameObject.tag == "PLC")
                {
                    plc_type += 1 * pom;
                    CorrectPLCType();
                    maximumOutputs = plc_types[plc_type];
                    DestroyIncorrectCables();
                }
                else if (gameObject.tag == "OLT")
                {
                    olt_type += 1 * pom;
                    CorrectOLTType();
                }
                else if (gameObject.tag == "ONT")
                {
                    ont_type += 1 * pom;
                    CorrectONTType();
                }
            }
            else
            {
                output_is_over_connector = !output_is_over_connector;
            }
        }
        ChangeText();
    }

    void DrawCables()
    {
        if (Input.GetMouseButtonDown(1) && on_me && outputs.Count < maximumOutputs && !Input.GetKey(KeyCode.LeftShift))
        {
            tmp_cable   = Instantiate(cablePrefab);
            tmp_lr      = tmp_cable.GetComponent<LineRenderer>();
            tmp_lr.startColor = output_is_over_connector ? over_connector : over_welding;
            Vector3 start_position  = transform.position;
            start_position.y        = 0.5f;
            tmp_lr.SetPosition(0, transform.position);
            holding_cable = true;
        }
        if (holding_cable)
        {
            Vector3 end_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            end_position.y       = 0.5f;
            tmp_lr.SetPosition(1, end_position);
        }
        if (Input.GetMouseButtonUp(1) && holding_cable)
        {
            holding_cable         = false;
            GameObject tmp_target = Raycast();
            if (!(DestroyIfTrue(!((bool)tmp_target))        ||
                  DestroyIfTrue(tmp_target.tag == "OLT")    ||
                  DestroyIfTrue(tmp_target.layer != 8)      ||
                  DestroyIfTrue(IsInOutputs(tmp_target))    ||
                  DestroyIfTrue(tmp_target.GetComponent<ElementController>().IsWithInput()) ||
                  DestroyIfTrue(tmp_target.GetInstanceID() == gameObject.GetInstanceID())))
            {
                Connection new_connection = new Connection(gameObject, tmp_cable, tmp_target);
                new_connection.targetController.SetInput(new_connection);
                outputs.Add(new_connection);
                tmp_cable = null;
                tmp_lr    = null;
            }
        }
    }

    void ControllInput()
    {
        if (with_input)
        {
            Color color                 = input_is_over_connector ? over_connector : over_welding;
            Vector3 end_position        = transform.position;
            end_position.y              = 0.5f;
            input.lineRenderer.endColor = color;
            input.lineRenderer.SetPosition(1, end_position);
            if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift) && on_me)
            {
                input.sourceController.DestroyOutput(gameObject);
                input       = null;
                with_input  = false;
            }
        }
    }

    void ControllOutput()
    {
        Color color           = output_is_over_connector ? over_connector : over_welding;
        Vector3 line_position = transform.position;
        line_position.y       = 0.5f;
        foreach (Connection output in outputs)
        {
            output.lineRenderer.SetPosition(0, line_position);
            output.lineRenderer.startColor = color;
        }
    }

    void CalculateCharacteristics()
    {
        if (gameObject.tag == "OLT")
        {
            power       = olt_type;
            attenuation = 0.0f;
            cable_index = 0;
        }
        if (gameObject.tag == "FBT" || gameObject.tag == "PLC" || gameObject.tag == "ONT")
        {
            if (with_input)
            {
                attenuation = input.sourceController.GetAttenuation(gameObject) + AttenuationOverWires();
                if (gameObject.tag == "ONT") attenuation += 0.5f;
                power       = input.sourceController.GetPower() - attenuation;
                cable_index = input.sourceController.GetCableIndex() + 1;
            }
            else
            {
                power = 0.0f;
                if (gameObject.tag == "PLC")
                    attenuation = GetPLCAttenuation();
                else if (gameObject.tag == "ONT")
                    attenuation = 0.5f;
                else
                    attenuation = 0.0f;
                cable_index = 0;
            }
        }
    }

    void ChangeText()
    {
        text.SetText(element_name + "\n");
        if (gameObject.tag != "OLT")
            text.SetText(text.text + "< " + (input_is_over_connector ? "C " : "W "));
        if (gameObject.tag != "ONT")
            text.SetText(text.text + (output_is_over_connector ? " C" : " W") + " >");
        if (gameObject.tag == "OLT")
            text.SetText(text.text + "\n" + olt_type.ToString() + " dB");
        if (gameObject.tag == "ONT")
            text.SetText(text.text + "\n" + ont_type.ToString() + " dB");
        if (gameObject.tag == "FBT")
            text.SetText(text.text + "\n" + fbt_type.ToString() + "/" + (100 - fbt_type).ToString());
        if (gameObject.tag == "PLC")
            text.SetText(text.text + "\n" + "1x" + plc_types[plc_type].ToString());
    }

    void CorrectFBTType()
    {
        if (fbt_type % 5 != 0)
            fbt_type = 5;
        if (fbt_type > 95)
            fbt_type = 5;
        if (fbt_type < 5)
            fbt_type = 95;
    }

    void CorrectPLCType()
    {
        if (plc_type > plc_types.Length - 1)
            plc_type = 0;
        if (plc_type < 0)
            plc_type = plc_types.Length - 1;
    }

    void CorrectOLTType()
    {
        if (olt_type > 4)
            olt_type = 1;
        if (olt_type < 1)
            olt_type = 4;
    }

    void CorrectONTType()
    {
        if (ont_type < -29)
            ont_type = -26;
        if (ont_type > -26)
            ont_type = -29;
    }

    void DestroyIncorrectCables()
    {
        int current_count = outputs.Count;
        if (current_count > maximumOutputs)
        {
            for (int i = 0; i < current_count - maximumOutputs; i++)
            {
                outputs[outputs.Count - 1].targetController.NullInput();
                Destroy(outputs[outputs.Count - 1].cable);
                outputs.RemoveAt(outputs.Count - 1);
            }
        }
    }

    void DestoyTmpCable()
    {
        Destroy(tmp_cable);
        tmp_lr = null;
    }

    void OnMouseEnter()
    {
        on_me = true;
    }

    void OnMouseExit()
    {
        on_me = false;
    }

    GameObject Raycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            return hit.transform.gameObject;
        }
        return null;
    }

    bool IsInOutputs(GameObject go)
    {
        int id = go.GetInstanceID();
        foreach (Connection output in outputs)
        {
            if (output.target.GetInstanceID() == id) return true;
        }
        return false;
    }

    bool DestroyIfTrue(bool condition)
    {
        if (condition) DestoyTmpCable();
        return condition;
    }

    float AttenuationOverWires()
    {
        return AttenuationOverWire(input.sourceController.GetIsOutputOverConnector()) + AttenuationOverWire(input_is_over_connector) + 0.033f;
    }

    float AttenuationOverWire(bool over_connector)
    {
        if (over_connector)
            return 0.3f;
        return 0.1f;
    }

    public void DestroyOutput(GameObject target)
    {
        Connection to_destroy = null;
        bool found            = false;
        int i                 = 0;
        foreach (Connection output in outputs)
        {
            if (output.target.GetInstanceID() == target.GetInstanceID())
            {
                to_destroy = output;
                found      = true;
                break;
            }
            i++;
        }
        if (found)
        {
            Destroy(to_destroy.cable);
            outputs.RemoveAt(i);
        }
    }

    public void NullInput()
    {
        input      = null;
        with_input = false;
    }

    public void CopySettings(GameObject element)
    {
        ElementController ec = element.GetComponent<ElementController>();
        this.maximumOutputs = ec.maximumOutputs;
        this.output_is_over_connector = ec.GetIsOutputOverConnector();
        this.input_is_over_connector = ec.GetIsInputOverConnector();
        this.fbt_type = ec.GetFBTType();
        this.plc_type = ec.GetPLCType();
        this.olt_type = ec.GetOLTType();
        this.ont_type = ec.GetONTType();
    }

    public bool IsWithInput()
    {
        return with_input;
    }

    public bool OnMe()
    {
        return on_me;
    }

    public bool GetIsOutputOverConnector()
    {
        return output_is_over_connector;
    }

    public bool GetIsInputOverConnector()
    {
        return input_is_over_connector;
    }

    public int GetFBTType()
    {
        return fbt_type;
    }

    public int GetPLCType()
    {
        return plc_type;
    }

    public int GetOLTType()
    {
        return olt_type;
    }

    public int GetONTType()
    {
        return ont_type;
    }

    public int GetCableIndex()
    {
        return cable_index;
    }

    public float GetPower()
    {
        return power;
    }

    public float GetAttenuation(GameObject target)
    {
        if (gameObject.tag == "FBT")
        {
            int index = 0;
            foreach(Connection output in outputs)
            {
                if (output.target.GetInstanceID() == target.GetInstanceID())
                    break;
                index++;
            }
            return attenuation + GetFBTAttenuation()[index];
        }
        if (gameObject.tag == "PLC")
        {
            return attenuation + GetPLCAttenuation();
        }
        return attenuation;
    }

    public float GetONTAttenuation()
    {
        return attenuation;
    }

    public float GetPLCAttenuation()
    {
        return plc_attenuations[plc_type];
    }

    public float[] GetFBTAttenuation()
    {
        switch (fbt_type)
        {
            case 5:
                return new float[] { 12.83f, 0.35f };
            case 10:
                return new float[] { 10.21f, 0.60f };
            case 15:
                return new float[] { 8.17f, 0.82f };
            case 20:
                return new float[] { 7.21f, 1.06f };
            case 25:
                return new float[] { 6.28f, 1.28f };
            case 30:
                return new float[] { 5.53f, 1.57f };
            case 35:
                return new float[] { 4.69f, 1.96f };
            case 40:
                return new float[] { 3.92f, 2.32f };
            case 45:
                return new float[] { 3.73f, 2.72f };
            case 50:
                return new float[] { 3.12f, 3.17f };
            case 55:
                return new float[] { 2.72f, 3.73f };
            case 60:
                return new float[] { 2.32f, 3.92f };
            case 65:
                return new float[] { 1.96f, 4.69f };
            case 70:
                return new float[] { 1.57f, 5.53f };
            case 75:
                return new float[] { 1.28f, 6.28f };
            case 80:
                return new float[] { 1.06f, 7.21f };
            case 85:
                return new float[] { 0.82f, 8.17f };
            case 90:
                return new float[] { 0.60f, 10.21f };
            case 95:
                return new float[] { 0.35f, 12.83f };
        }
        return null;
    }

    public List<Connection> GetOutputs()
    {
        return outputs;
    }

    public Connection GetInput()
    {
        return input;
    }

    public void SetInput(Connection connection)
    {
        Connection new_connection = new Connection(connection.source, connection.cable, connection.target);
        this.input = new_connection;
        with_input = true;
    }

}
