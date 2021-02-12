using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBelt : MonoBehaviour, ISaveable
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
        if(_currentSlot == null)
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

    public ATool GetToolByClassName(string className)
    {
        foreach (ToolSlot slot in slots)
        {
            if (slot.Tool.GetType().ToString().Equals(className))
                return slot.Tool;
        }
        return null;
    }

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

    public object CaptureState()
    {
        Dictionary<string, float> toolFills = new Dictionary<string, float>();
        List<string> availableTools = new List<string>();
        List<string> blockedTools = new List<string>();
        foreach (ToolSlot toolSlot in slots)
        {
            toolFills.Add(toolSlot.Tool.GetType().ToString(), toolSlot.Tool.GetFill());
            if (toolSlot.IsAvailable)
                availableTools.Add(toolSlot.Tool.GetType().ToString());
            if (toolSlot.IsBlocked)
                blockedTools.Add(toolSlot.Tool.GetType().ToString());
        }
        return new SaveState
        {
            CurrentSlot = _currentSlot != null ? _currentSlot.Slot : -1,
            LastSlot = _lastSlot != null ? _lastSlot.Slot : -1,
            ToolFills = toolFills,
            AvailableTools = availableTools,
            BlockedTools = blockedTools
        };
    }

    public void RestoreState(object state)
    {
        SaveState saveState = (SaveState)state;

        SetTool(saveState.CurrentSlot);
        _lastSlot = GetToolSlot(saveState.LastSlot);
        foreach (ToolSlot toolSlot in slots)
        {
            if (saveState.ToolFills.TryGetValue(toolSlot.Tool.GetType().ToString(), out float fill))
                toolSlot.Tool.SetFill(fill);
            toolSlot.IsAvailable = saveState.AvailableTools.Contains(toolSlot.Tool.GetType().ToString());
            toolSlot.IsBlocked = saveState.BlockedTools.Contains(toolSlot.Tool.GetType().ToString());
        }

        OnToolBlocked?.Invoke();
        OnToolUnblocked?.Invoke();
        OnToolSwitched?.Invoke();
    }

    [System.Serializable]
    private struct SaveState
    {
        public int CurrentSlot;
        public int LastSlot;
        public Dictionary<string, float> ToolFills;
        public List<string> AvailableTools;
        public List<string> BlockedTools;
    }
}
