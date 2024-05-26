using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Magus.UserInterface
{
    public class DraggableSkill : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image maskImage;
        [SerializeField] private Canvas canvas;

        [HideInInspector] public Transform originalParent;

        private void Awake()
        {
            originalParent = transform.parent;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(canvas.transform);
            transform.SetAsLastSibling();
            maskImage.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ResetPosition();
        }

        public void ResetPosition()
        {
            if (originalParent == null) return;
            transform.SetParent(originalParent);
            transform.SetAsFirstSibling();
            maskImage.raycastTarget = true;
        }
    }
}
