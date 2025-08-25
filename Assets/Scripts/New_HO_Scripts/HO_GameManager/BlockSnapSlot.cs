using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockSnapSlot : MonoBehaviour
{
    [Header("Slot Settings")]
    private BlockType acceptedBlockType;
    private FormulaBlock ownerBlock;
    private SnapPosition position;
    private FormulaBlock connectedBlock;
    
    [Header("Visual Settings")]
    public Color normalColor = new Color(1f, 1f, 1f, 0.3f);
    public Color highlightColor = new Color(1f, 1f, 0f, 0.6f);
    public Color occupiedColor = new Color(0f, 1f, 0f, 0.3f);
    
    private Image slotImage;
    private CircleCollider2D slotCollider;
    
    public BlockType AcceptedBlockType => acceptedBlockType;
    public FormulaBlock ConnectedBlock => connectedBlock;
    public bool HasBlock => connectedBlock != null;
    public SnapPosition GetPosition() => position;
    public FormulaBlock GetOwnerBlock() => ownerBlock;
    
    public void Initialize(BlockType acceptedType, FormulaBlock owner, SnapPosition pos)
    {
        acceptedBlockType = acceptedType;
        ownerBlock = owner;
        position = pos;
        SetupCollider();
    }
    
    
    private void SetupCollider()
    {
        // Add collider for trigger detection
        slotCollider = gameObject.AddComponent<CircleCollider2D>();
        slotCollider.isTrigger = true;
        slotCollider.radius = 30f; // Snap detection radius
    }

    
    private string GetSlotSymbol()
    {
        switch (acceptedBlockType)
        {
            case BlockType.Coefficient:
                return "C";
            case BlockType.Sign:
                return "±";
            case BlockType.Constant:
                return "N";
            case BlockType.Variable:
                return "V";
            default:
                return "?";
        }
    }
    
    public bool CanAcceptBlock(FormulaBlock block)
    {
        if (HasBlock) return false;
        if (block == null) return false;
        if (block == ownerBlock) return false; // Can't connect to self
        
        return block.blockType == acceptedBlockType;
    }
    
    public void SetConnectedBlock(FormulaBlock block)
    {
        connectedBlock = block;
        UpdateVisualState();
    }
    
    public void RemoveConnectedBlock()
    {
        connectedBlock = null;
        UpdateVisualState();
    }
    
    private void UpdateVisualState()
    {
        if (slotImage == null) return;
        
        if (HasBlock)
        {
            slotImage.color = occupiedColor;
        }
        else
        {
            slotImage.color = normalColor;
        }
    }
    
    public void HighlightSlot(bool highlight)
    {
        if (slotImage == null || HasBlock) return;
        
        slotImage.color = highlight ? highlightColor : normalColor;
    }
    
    // Trigger events for highlighting
    private void OnTriggerEnter2D(Collider2D other)
    {
        FormulaBlock block = other.GetComponent<FormulaBlock>();
        if (block != null && CanAcceptBlock(block))
        {
            HighlightSlot(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        FormulaBlock block = other.GetComponent<FormulaBlock>();
        if (block != null)
        {
            HighlightSlot(false);
        }
    }
    
    // Get the world position for snapping
    public Vector3 GetSnapPosition()
    {
        return transform.position;
    }
    
    // Get color based on accepted block type
    public Color GetTypeColor()
    {
        switch (acceptedBlockType)
        {
            case BlockType.Coefficient:
                return Color.blue;
            case BlockType.Sign:
                return Color.yellow;
            case BlockType.Constant:
                return Color.green;
            case BlockType.Variable:
                return Color.red;
            default:
                return Color.white;
        }
    }
    
    public void ShowSlot(bool show)
    {
        if (slotImage != null)
        {
            Color color = slotImage.color;
            color.a = show ? (HasBlock ? occupiedColor.a : normalColor.a) : 0f;
            slotImage.color = color;
        }
    }
    
    // Debug information
    public string GetDebugInfo()
    {
        string connectionStatus = HasBlock ? $"Connected to {connectedBlock.blockType}" : "Empty";
        return $"{position} slot for {acceptedBlockType} - {connectionStatus}";
    }
}