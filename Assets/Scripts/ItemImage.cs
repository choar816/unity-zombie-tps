using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemImage : MonoBehaviour, IPointerClickHandler
{
    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (image.sprite == null)
        {
            GameManager.Instance.HideContextMenu();
            return;
        }

        // left mouse button
        if (pointerEventData.pointerId == -1)
        {
            GameManager.Instance.HideContextMenu();
        }
        // right mouse button
        else if (pointerEventData.pointerId == -2)
        {
            GameManager.Instance.SetSelectedItem(transform.parent.gameObject);
            GameManager.Instance.ShowContextMenu(pointerEventData.pressPosition);
        }
    }
}
