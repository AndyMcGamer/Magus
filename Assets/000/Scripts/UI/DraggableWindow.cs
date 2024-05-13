using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Magus.UserInterface
{
    public class DraggableWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [SerializeField] private RectTransform windowTransform;
        [SerializeField] private Canvas canvas;

        private void Awake()
        {
            windowTransform = windowTransform != null ? windowTransform : transform.parent.GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            windowTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            windowTransform.SetAsLastSibling();
        }
    }
}
