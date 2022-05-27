using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelMover : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{

    public float interpolation = 50.0f;

    private RectTransform panel;
    private bool move_out = false;
    private bool move_back = false;

    void Start()
    {
        panel = GetComponent<RectTransform>();
        Vector2 position = panel.anchoredPosition;
        position.x = -panel.rect.width/2.0f;
        panel.anchoredPosition = position;
    }

    void Update()
    {
        if (move_out)
        {
            Vector2 position = panel.anchoredPosition;
            panel.anchoredPosition = Vector2.Lerp(position, new Vector2(0.0f, position.y), interpolation * Time.deltaTime);
            if (panel.anchoredPosition.x == 0.0f) move_out = false;
        }

        if (move_back)
        {
            Vector2 position = panel.anchoredPosition;
            panel.anchoredPosition = Vector2.Lerp(position, new Vector2(-panel.rect.width/2.0f, position.y), interpolation * Time.deltaTime);
            if (panel.anchoredPosition.x == -panel.rect.width/2.0f) move_back = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        move_out = true;
        move_back = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        move_back = true;
        move_out = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        move_back = true;
        move_out = false;
    }
}
