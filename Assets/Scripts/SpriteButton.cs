using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum SpriteButtonTransitionType
{
    None,
    Color,
    Scale,
    Sprite,
}

public class SpriteButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    public SpriteButtonTransitionType transitionType = SpriteButtonTransitionType.None;

    private SpriteRenderer _spriteRenderer;

    // 定义不同状态的颜色
    private Color _normalColor;    // 正常状态颜色 
    private Color _highlightColor; // 高亮状态颜色
    private Color _pressedColor;   // 按下状态颜色
    private Color _disabledColor;  // 禁用状态颜色

    private bool _isInteractable = true; // 是否可交互
    private bool _isPressed = false;     // 追踪按下状态

    public UnityEvent onClick = new();

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // 初始化各种状态的颜色
        _normalColor = _spriteRenderer.color;
        _highlightColor = new Color(_normalColor.r * 1.2f, _normalColor.g * 1.2f, _normalColor.b * 1.2f, _normalColor.a);
        _pressedColor = new Color(_normalColor.r * 0.8f, _normalColor.g * 0.8f, _normalColor.b * 0.8f, _normalColor.a);
        _disabledColor = new Color(_normalColor.r * 0.5f, _normalColor.g * 0.5f, _normalColor.b * 0.5f, _normalColor.a * 0.5f);
    }

    public void SetAlpha(float alpha)
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, alpha);
    }


    // 设置交互状态
    public void SetInteractable(bool interactable)
    {
        _isInteractable = interactable;

        switch (transitionType)
        {
            case SpriteButtonTransitionType.Color:
                _spriteRenderer.color = _isInteractable ? _normalColor : _disabledColor;
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isInteractable) return;

        switch (transitionType)
        {
            case SpriteButtonTransitionType.Color:
                _spriteRenderer.color = _highlightColor;
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isInteractable) return;

        switch (transitionType)
        {
            case SpriteButtonTransitionType.Color:
                _spriteRenderer.color = _normalColor;
                break;
        }
        _isPressed = false; // 鼠标移出时重置按下状态
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !_isInteractable) return;

        switch (transitionType)
        {
            case SpriteButtonTransitionType.Color:
                _spriteRenderer.color = _pressedColor;
                break;
        }
        _isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !_isInteractable) return;

        switch (transitionType)
        {
            case SpriteButtonTransitionType.Color:
                _spriteRenderer.color = _highlightColor;
                break;
        }

        if (_isPressed) // 只有在之前按下的情况下才触发点击
        {
            Press();
        }
        _isPressed = false;
    }

    private void Press()
    {
        onClick.Invoke();
    }

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }
}