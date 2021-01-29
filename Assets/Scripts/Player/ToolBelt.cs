using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBelt : MonoBehaviour
{
    public System.Action OnToolBlocked;
    public System.Action OnToolUnblocked;
    public System.Action OnToolSwitched;

    [System.Serializable]
    public class ToolSlot
    {
        public int Slot;
        public ATool Tool;
        public bool IsAvailable;
        public bool IsBlocked;

        public ToolSlot(int slot, ATool tool, bool isAvailable = true, bool isBlocked = false)
        {
            Slot = slot;
            Tool = tool;
            IsAvailable = isAvailable;
            IsBlocked = isBlocked;
        }
    }

    [SerializeField] private int startTool;
    [SerializeField] private ToolSlot[] slots;

    private ToolSlot _currentSlot = null;
    private ToolSlot _lastSlot = null;
    private ToolSlot _emptySlot = new ToolSlot(-1, null);

    private void Awake()
    {
        foreach (ToolSlot slot in slots)
            slot.Tool.gameObject.transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        SetTool(startTool);
    }

    public void SetEmptySlot()
    {
        PlayerController.Instance.SetCurrentTool(null);
        _lastSlot = _currentSlot;
        _currentSlot = _emptySlot;
    }

    public void SetTool(int index)
    {
        ToolSlot slot = GetToolSlot(index);
        if (!slot.IsAvailable || slot.IsBlocked)
            return;

        PlayerController.Instance.SetCurrentTool(slot.Tool);
        _lastSlot = _currentSlot;
        _currentSlot = slot;
        OnToolSwitched?.Invoke();
    }

    public void BlockTool(ATool tool)
    {
        ToolSlot slot = GetToolSlot(tool);
        if (slot == _emptySlot)
            return;
        if (slot == _currentSlot)
            SetEmptySlot();
        slot.IsBlocked = true;
        OnToolBlocked?.Invoke();
    }

    public void UnblockTool(ATool tool)
    {
        ToolSlot slot = GetToolSlot(tool);
        slot.IsBlocked = false;
        if (slot == _lastSlot)
            SetTool(slot.Slot);
        OnToolUnblocked?.Invoke();
    }

    public ToolSlot GetCurrentSlot() => _currentSlot;
    public ToolSlot[] GetToolSlots() => slots;

    private ToolSlot GetToolSlot(int index)
    {
        foreach(ToolSlot slot in slots)
        {
            if (slot.Slot == index)
                return slot;
        }
        return _emptySlot;
    }
    private ToolSlot GetToolSlot(ATool tool)
    {
        foreach (ToolSlot slot in slots)
        {
            if (slot.Tool == tool)
                return slot;
        }
        return _emptySlot;
    }
}
