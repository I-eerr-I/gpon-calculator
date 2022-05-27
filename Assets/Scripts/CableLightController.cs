using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableLightController : MonoBehaviour
{
    public GameObject lightSourcePrefab;

    private LineRenderer line_renderer;
    private GameObject light_source;

    void Start()
    {
        line_renderer = GetComponent<LineRenderer>();
        light_source  = Instantiate(lightSourcePrefab, transform);
    }

    void Update()
    {
        Vector3 midpoint = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3[] line_positions = new Vector3[line_renderer.positionCount];
        line_renderer.GetPositions(line_positions);
        foreach(Vector3 line_position in line_positions)
        {
            midpoint += line_position;
        }
        light_source.transform.position = midpoint / 2.0f;
    }
}
