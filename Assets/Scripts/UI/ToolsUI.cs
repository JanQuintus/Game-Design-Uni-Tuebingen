using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ToolBelt;
using DG.Tweening;

public class ToolsUI : MonoBehaviour
{
    [SerializeField] private Color availableColor;
    [SerializeField] private Color equippedColor;
    [SerializeField] private Color blockedColor;

    private class ToolSlotUI
    {
        public Transform Parent;
        public ToolSlot Slot;
        public Image Fill;

        public ToolSlotUI(Transform parent, ToolSlot slot, Image fill)
        {
            Parent = parent;
            Slot = slot;
            Fill = fill;
        }
    }

    private List<ToolSlotUI> _uiSlots = new List<ToolSlotUI>();

    private void Start()
    {
        foreach(ToolSlot slot in PlayerController.Instance.GetToolBelt().GetToolSlots())
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Equals(slot.Tool.name))
                {
                    ToolSlotUI slotUI = new ToolSlotUI(transform.GetChild(i), slot, transform.GetChild(i).Find("Image").GetComponent<Image>());
                    _uiSlots.Add(slotUI);
                    slotUI.Slot.Tool.OnFillChanged += () =>
                        slotUI.Fill.fillAmount = slotUI.Slot.IsBlocked ? 1f : slotUI.Slot.Tool.GetFillPercentage();
                }
            }
        }

        PlayerController.Instance.GetToolBelt().OnToolBlocked += UpdateUI;
        PlayerController.Instance.GetToolBelt().OnToolUnblocked += UpdateUI;
        PlayerController.Instance.GetToolBelt().OnToolSwitched += UpdateUI;
        PlayerController.Instance.GetToolBelt().OnToolUnlocked += UpdateUI;

        UpdateUI();
    }

    private void OnDestroy()
    {
        PlayerController.Instance.GetToolBelt().OnToolBlocked -= UpdateUI;
        PlayerController.Instance.GetToolBelt().OnToolUnblocked -= UpdateUI;
        PlayerController.Instance.GetToolBelt().OnToolSwitched -= UpdateUI;
        PlayerController.Instance.GetToolBelt().OnToolUnlocked -= UpdateUI;
    }

    private void UpdateUI() { 

        foreach(ToolSlotUI slot in _uiSlots)
        {
            if (!slot.Slot.IsAvailable)
            {
                slot.Parent.localScale = Vector3.zero;
                continue;
            }
            slot.Parent.localScale = Vector3.one;
            Color color = slot.Slot.IsBlocked ? blockedColor : (slot.Slot == PlayerController.Instance.GetToolBelt().GetCurrentSlot() ? equippedColor : availableColor);
            slot.Fill.DOColor(color, .2f);
            slot.Fill.fillAmount = slot.Slot.IsBlocked ? 1f : slot.Slot.Tool.GetFillPercentage();
            Canvas.ForceUpdateCanvases();
        }

    }
}
