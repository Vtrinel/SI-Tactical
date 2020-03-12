using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCircle : MonoBehaviour
{
    [SerializeField] Animator animController = default;
    [SerializeField] SpriteRenderer spriteRenderer = default;

    bool _hovered = false;
    public void SetHovered(bool hovered)
    {
        _hovered = hovered;
        animController.SetBool("Hovered", hovered);
    }

    public void SetColor(Color hoverColor)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = hoverColor;
    }
}