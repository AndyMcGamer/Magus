using Magus.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Magus.Skills
{
    public class SkillDisplayHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public SkillDisplaySlot displaySlot;
        [SerializeField] private Image selectionFrame;
        [SerializeField] private Image hover;

        public event Action<SkillDisplayHover> OnStartDrag;
        public event Action<SkillDisplayHover> OnStopDrag;
        public event Action<SkillDisplayHover> OnHover;
        public event Action OnStopHover;

        bool dragging = false;

        private void Awake()
        {
            selectionFrame.gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!displaySlot.DraggableSkill.gameObject.activeSelf) return;
            displaySlot.DraggableSkill.OnBeginDrag(eventData);
            OnStartDrag?.Invoke(this);
            selectionFrame.gameObject.SetActive(false);
            OnStopHover?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!displaySlot.DraggableSkill.gameObject.activeSelf) return;
            displaySlot.DraggableSkill.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!displaySlot.DraggableSkill.gameObject.activeSelf) return;
            displaySlot.DraggableSkill.OnEndDrag(eventData);
            OnStopDrag?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!displaySlot.DraggableSkill.gameObject.activeSelf || dragging) return;
            selectionFrame.gameObject.SetActive(true);
            OnHover?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            selectionFrame.gameObject.SetActive(false);
            if(!dragging) OnStopHover?.Invoke();
        }

        public void SetSelectionState(bool active)
        {
            hover.raycastTarget = active;
            dragging = !active;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnEndDrag(eventData);
            eventData.pointerDrag = displaySlot.DraggableSkill.gameObject;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }
    }
}
