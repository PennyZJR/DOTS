using System;
using MonoBehaviuors;
using UnityEngine;

namespace UI
{
    public class UnitSelectionManagerUI:MonoBehaviour
    {
        [SerializeField]
        private RectTransform selectionAreaRectTransform;

        [SerializeField]
        private Canvas canvas;
        private void Start()
        {
            UnitSelectionManager.Instance.OnSelectionAreaStart+=UnitSelectionManager_OnSelectionAreaStart;
            UnitSelectionManager.Instance.OnSelectionAreaEnd+=UnitSelectionManager_OnSelectionAreaEnd;
            selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void Update()
        {
            if(selectionAreaRectTransform.gameObject.activeSelf)
                UpdateVisual();
        }

        private void UnitSelectionManager_OnSelectionAreaStart(object sender, EventArgs e)
        {
            selectionAreaRectTransform.gameObject.SetActive(true);
            UpdateVisual();
        }
        private void UnitSelectionManager_OnSelectionAreaEnd(object sender, EventArgs e)
        {
            selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void UpdateVisual()
        {
            var rect=UnitSelectionManager.Instance.GetSelectionAreaRect();
            float canvasScale = canvas.transform.localScale.x;
            selectionAreaRectTransform.anchoredPosition = new Vector2(rect.x,rect.y)/canvasScale;
            selectionAreaRectTransform.sizeDelta = new Vector2(rect.width,rect.height)/canvasScale;
        }

    }
}