using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class MenuEventSystemHandler : MonoBehaviour
{
    [Header("References")] public List<Selectable> Selectables = new List<Selectable>();
    [SerializeField] protected Selectable _firstSelected;

    [Header("Control")] [SerializeField] protected InputActionReference _navigateReference;

    [Header("Animation")] [SerializeField] protected float _selectedAnimationScale = 1.1f;
    [SerializeField] protected float _scaleDuration = 0.25f;

    protected Dictionary<Selectable, Vector3> _scales = new Dictionary<Selectable, Vector3>();

    protected Selectable _lastSelected;

    protected Tween _scaleUpTween;
    protected Tween _scaleDownTween;

    public void Awake()
    {
        foreach (Selectable selectable in Selectables)
        {
            AddSelectionListener(selectable);
            _scales.Add(selectable, selectable.transform.localScale);
        }
    }

    public void OnEnable()
    {
        _navigateReference.action.performed += OnNavigate;

        // ensure all selectables are reset back to their original scale
        for (int i = 0; i < Selectables.Count; i++)
        {
            Selectables[i].transform.localScale = _scales[Selectables[i]];
        }

        StartCoroutine(SelectAfterDelay());
    }

    protected virtual IEnumerator SelectAfterDelay()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
    }

    public void OnDisable()
    {
        _navigateReference.action.performed -= OnNavigate;

        _scaleUpTween.Kill(true);
        _scaleDownTween.Kill(true);
    }

    protected virtual void AddSelectionListener(Selectable selectable)
    {
        // add listner
        EventTrigger trigger = selectable.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = selectable.gameObject.AddComponent<EventTrigger>();
        }

        // add select event
        EventTrigger.Entry SelectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Select
        };
        SelectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(SelectEntry);

        // add deselect event
        EventTrigger.Entry DeselectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Deselect
        };
        DeselectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(DeselectEntry);

        // add pointer enter event
        EventTrigger.Entry PointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        PointerEnter.callback.AddListener(OnPointerEnter);
        trigger.triggers.Add(PointerEnter);

        // add pointer exit event
        EventTrigger.Entry PointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        PointerExit.callback.AddListener(OnPointerExit);
        trigger.triggers.Add(PointerExit);
    }

    public void OnSelect(BaseEventData eventData)
    {
        _lastSelected = eventData.selectedObject.GetComponent<Selectable>();

        Vector3 newScale = eventData.selectedObject.transform.localScale * _selectedAnimationScale;
        _scaleUpTween = eventData.selectedObject.transform.DOScale(newScale, _scaleDuration);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Selectable sel = eventData.selectedObject.GetComponent<Selectable>();
        _scaleDownTween = sel.transform.DOScale(_scales[sel], _scaleDuration);
    }

    public void OnPointerEnter(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            Selectable sel = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            if (sel == null)
            {
                sel = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
            }

            pointerEventData.selectedObject = sel.gameObject;
        }
    }

    public void OnPointerExit(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            pointerEventData.selectedObject = null;
        }
    }

    protected virtual void OnNavigate(InputAction.CallbackContext context)
    {
        // 每次按下导航按钮时，都会调用此方法
        if (EventSystem.current.currentSelectedGameObject == null && _lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
        }
    }
}