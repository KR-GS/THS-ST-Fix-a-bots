using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// New enum for block types
public enum BlockType
{
    Coefficient,
    Sign,
    Constant,
    Variable
}

// New enum for snap positions
public enum SnapPosition
{
    Left,
    Right
}

[System.Serializable]
public class FormulaResult
{
    public int coefficient;
    public int constant;
    public bool isComplete;
    
    public FormulaResult()
    {
        coefficient = 0;
        constant = 0;
        isComplete = false;
    }
    
    public FormulaResult(int coef, int constt, bool complete)
    {
        coefficient = coef;
        constant = constt;
        isComplete = complete;
    }
    
    public override string ToString()
    {
        string sign = constant >= 0 ? "+" : "-";
        int absConst = Mathf.Abs(constant);
        return $"{coefficient}n {sign} {absConst}";
    }
}

public class FormulaBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Block Settings")]
    public BlockType blockType;
    public int value;
    public string symbol;
    public Color blockColor = Color.white;

    [Header("UI References")]
    public TextMeshProUGUI blockText;
    public Image blockImage;

    [Header("Snap Slots")]
    public BlockSnapSlot leftSnapSlot;
    public BlockSnapSlot rightSnapSlot;

    [Header("Block Sprites")]
    public Sprite coefficientSprite;
    public Sprite signSprite;
    public Sprite constantSprite;
    public Sprite variableSprite;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;
    private bool isDragging = false;
    private bool isLocked = false;

    // Connected blocks
    private FormulaBlock leftConnectedBlock;
    private FormulaBlock rightConnectedBlock;
    private FormulaBlock parentBlock; // The block this is connected to

    public bool IsLocked => isLocked;
    public int Value => value;
    public string Symbol => symbol;
    public FormulaBlock LeftConnectedBlock => leftConnectedBlock;
    public FormulaBlock RightConnectedBlock => rightConnectedBlock;
    public bool HasLeftConnection => leftConnectedBlock != null;
    public bool HasRightConnection => rightConnectedBlock != null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // Set up snap slots 
        SetupSnapSlots();

        // Set up the block appearance
        UpdateBlockAppearance();
    }

    private void SetupSnapSlots()
    {
        // Create snap slots based on block type
        switch (blockType)
        {
            case BlockType.Variable: // n block - can accept coefficient on left, sign on right
                if (leftSnapSlot == null)
                    leftSnapSlot = CreateSnapSlot(SnapPosition.Left, BlockType.Coefficient);
                if (rightSnapSlot == null)
                    rightSnapSlot = CreateSnapSlot(SnapPosition.Right, BlockType.Sign);
                break;

            case BlockType.Sign: // sign block - can accept constant on right
                if (rightSnapSlot == null)
                    rightSnapSlot = CreateSnapSlot(SnapPosition.Right, BlockType.Constant);
                break;

            case BlockType.Coefficient: // coefficient - no snap slots (gets connected to others)
            case BlockType.Constant: // constant - no snap slots (gets connected to others)
                break;
        }
    }

    private BlockSnapSlot CreateSnapSlot(SnapPosition position, BlockType acceptedType)
    {
        GameObject slotObj = new GameObject($"{position}SnapSlot");
        slotObj.transform.SetParent(transform);

        RectTransform slotRect = slotObj.AddComponent<RectTransform>();
        BlockSnapSlot snapSlot = slotObj.AddComponent<BlockSnapSlot>();

        // Position the snap slot
        float xOffset = position == SnapPosition.Left ? -115f : 115f;
        slotRect.anchoredPosition = new Vector2(xOffset, 0);
        slotRect.sizeDelta = new Vector2(60f, 60f);

        snapSlot.Initialize(acceptedType, this, position);

        return snapSlot;
    }

    public void Initialize(BlockType type, int val, string sym, Color color)
    {
        blockType = type;
        value = val;
        symbol = sym;
        blockColor = color;
        UpdateBlockAppearance();
        SetupSnapSlots();
    }

    private void UpdateBlockAppearance()
    {
        if (blockText != null)
        {
            blockText.text = blockType == BlockType.Variable ? symbol :
                            blockType == BlockType.Sign ? symbol : value.ToString();
        }

        if (blockImage != null)
        {
            blockImage.color = blockColor;

            switch (blockType)
            {
                case BlockType.Coefficient:
                    if (coefficientSprite != null) blockImage.sprite = coefficientSprite;
                    break;
                case BlockType.Sign:
                    if (signSprite != null) blockImage.sprite = signSprite;
                    break;
                case BlockType.Constant:
                    if (constantSprite != null) blockImage.sprite = constantSprite;
                    break;
                case BlockType.Variable:
                    if (variableSprite != null) blockImage.sprite = variableSprite;
                    break;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        isDragging = true;

        // Special handling for Variable block (n) - move entire chain
        if (blockType == BlockType.Variable)
        {
            StartDragChain();
        }
        else
        {
            // Disconnect from parent if connected
            DisconnectFromParent();
            StartSingleDrag();
        }
    }

    private void StartDragChain()
    {
        // Make all connected blocks semi-transparent and non-interactive
        List<FormulaBlock> chainBlocks = GetAllConnectedBlocks();
        foreach (FormulaBlock block in chainBlocks)
        {
            block.canvasGroup.alpha = 0.6f;
            block.canvasGroup.blocksRaycasts = false;
        }

        // Move entire chain to top layer
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }

    private void StartSingleDrag()
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && !isLocked)
        {
            Vector2 deltaPosition = eventData.delta / canvas.scaleFactor;

            if (blockType == BlockType.Variable)
            {
                Vector2 currentPos = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = currentPos + deltaPosition;
            }
            else
            {
                Vector2 currentPos = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = currentPos + deltaPosition;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        isDragging = false;

        if (blockType == BlockType.Variable)
        {
            EndDragChain();
        }
        else
        {
            EndSingleDrag();
        }
    }

    private void EndDragChain()
    {
        // Restore all connected blocks
        List<FormulaBlock> chainBlocks = GetAllConnectedBlocks();
        foreach (FormulaBlock block in chainBlocks)
        {
            block.canvasGroup.alpha = 1f;
            block.canvasGroup.blocksRaycasts = true;
        }

        // Try to snap the entire chain to a valid position
        EnsureChainInBounds();
    }

    private void EndSingleDrag()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Try to find a valid snap target
        BlockSnapSlot nearestSlot = FindNearestValidSnapSlot();

        if (nearestSlot != null && nearestSlot.CanAcceptBlock(this))
        {
            // Snap to the slot
            ConnectToSlot(nearestSlot);
        }
        else
        {
            EnsureBlockInReasonableContainer();
        }
    }

    private void EnsureBlockInReasonableContainer()
    {
        if (transform.parent == canvas.transform)
        {           
            return;
        }
    }

    private void DisconnectFromParent()
    {
        if (parentBlock == null) return;

        // Store the parent reference before nullifying
        FormulaBlock previousParent = parentBlock;

        if (parentBlock.leftConnectedBlock == this)
        {
            parentBlock.leftConnectedBlock = null;
            if (parentBlock.leftSnapSlot != null)
                parentBlock.leftSnapSlot.SetConnectedBlock(null);
        }
        else if (parentBlock.rightConnectedBlock == this)
        {
            parentBlock.rightConnectedBlock = null;
            if (parentBlock.rightSnapSlot != null)
                parentBlock.rightSnapSlot.SetConnectedBlock(null);
        }

        // Clear the parent reference
        parentBlock = null;
        
        // Reset color on disconnect
        blockImage.color = blockColor; 

        // Notify about the disconnection
        FormulaInputPanel.Instance?.OnBlockDisconnected(this, previousParent);
    }

    public void UpdateOriginalPosition()
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }


    private BlockSnapSlot FindNearestValidSnapSlot()
    {
        BlockSnapSlot[] allSlots = FindObjectsOfType<BlockSnapSlot>();
        BlockSnapSlot nearestSlot = null;
        float nearestDistance = float.MaxValue;
        float maxSnapDistance = 100f;

        foreach (BlockSnapSlot slot in allSlots)
        {
            if (!slot.CanAcceptBlock(this)) continue;

            float distance = Vector3.Distance(rectTransform.position, slot.transform.position);
            if (distance < nearestDistance && distance < maxSnapDistance)
            {
                nearestDistance = distance;
                nearestSlot = slot;
            }
        }

        return nearestSlot;
    }

    public void ConnectToSlot(BlockSnapSlot slot)
    {
        // Position this block at the slot
        transform.SetParent(slot.transform);
        rectTransform.anchoredPosition = Vector2.zero;

        // Establish the connection
        FormulaBlock targetBlock = slot.GetOwnerBlock();

        if (slot.GetPosition() == SnapPosition.Left)
        {
            targetBlock.leftConnectedBlock = this;
            parentBlock = targetBlock;
        }
        else
        {
            targetBlock.rightConnectedBlock = this;
            parentBlock = targetBlock;
        }

        slot.SetConnectedBlock(this);
        
        // Change color to indicate correct placement
        // TODO: Make this only work for certain stages so new StaticData var?
        if (FormulaInputPanel.Instance != null)
        {
            var seq = FormulaInputPanel.Instance.GetTargetSequence(); // get the correct target
            if (slot.GetPosition() == SnapPosition.Left && blockType == BlockType.Coefficient)
            {
                // Check if coefficient matches
                if (value == seq.Coefficient)
                    blockImage.color = BlockManager.variableColor;
            }
            else if (slot.GetPosition() == SnapPosition.Right && blockType == BlockType.Sign)
            {
                // For sign, check if this symbol is the correct one
                string expectedSymbol = seq.Constant >= 0 ? "+" : "-";
                if (symbol == expectedSymbol)
                    blockImage.color = BlockManager.variableColor;
            }
            else if (slot.GetPosition() == SnapPosition.Right && blockType == BlockType.Constant)
            {
                // Check if constant matches
                if (value == Mathf.Abs(seq.Constant))
                    blockImage.color = BlockManager.variableColor;
            }
        }

        // Notify the formula panel
        FormulaInputPanel.Instance?.OnBlockConnected(this, targetBlock, slot.GetPosition());

        // Update positions of all blocks in chain
        UpdateChainPositions();
    }

    public bool IsSnapped()
    {
        return parentBlock != null;
    }

    public void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }

    private List<FormulaBlock> GetAllConnectedBlocks()
    {
        List<FormulaBlock> blocks = new List<FormulaBlock>();
        CollectConnectedBlocks(blocks);
        return blocks;
    }

    private void CollectConnectedBlocks(List<FormulaBlock> blocks)
    {
        if (blocks.Contains(this)) return;

        blocks.Add(this);

        if (leftConnectedBlock != null)
            leftConnectedBlock.CollectConnectedBlocks(blocks);

        if (rightConnectedBlock != null)
            rightConnectedBlock.CollectConnectedBlocks(blocks);
    }

    private void UpdateChainPositions()
    {
        // Only update positions if this is the variable block (center of formula)
        if (blockType != BlockType.Variable) return;
        
        // Keep the variable block where it is, just update connected blocks relative to it
        Vector2 nPosition = rectTransform.anchoredPosition;
        
        // Position coefficient (left of n)
        if (leftConnectedBlock != null)
        {
            leftConnectedBlock.rectTransform.anchoredPosition = nPosition + new Vector2(-100f, 0);
        }
        
        // Position sign (right of n)
        if (rightConnectedBlock != null)
        {
            rightConnectedBlock.rectTransform.anchoredPosition = nPosition + new Vector2(100f, 0);
            
            // Position constant (right of sign)
            if (rightConnectedBlock.rightConnectedBlock != null)
            {
                rightConnectedBlock.rightConnectedBlock.rectTransform.anchoredPosition = nPosition + new Vector2(200f, 0);
            }
        }
    }

    private void PositionChainFromThis(Vector2 startPosition)
    {
        // Don't move the variable block if it's already positioned
        if (blockType == BlockType.Variable && rectTransform.anchoredPosition != Vector2.zero)
        {
            // Use the current position of the variable block as the reference
            startPosition = rectTransform.anchoredPosition;
        }
        else
        {
            rectTransform.anchoredPosition = startPosition;
        }

        if (rightConnectedBlock != null)
        {
            Vector2 nextPosition = startPosition + new Vector2(100f, 0);
            rightConnectedBlock.PositionChainFromThis(nextPosition);
        }
    }

    private void EnsureChainInBounds()
    {
        // Simple bounds checking - keep the chain within the canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.sizeDelta;

        List<FormulaBlock> chainBlocks = GetAllConnectedBlocks();

        // Find bounds of the entire chain
        float minX = float.MaxValue, maxX = float.MinValue;
        foreach (FormulaBlock block in chainBlocks)
        {
            Vector3[] corners = new Vector3[4];
            block.rectTransform.GetWorldCorners(corners);

            for (int i = 0; i < 4; i++)
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, corners[i], null, out localPoint);

                minX = Mathf.Min(minX, localPoint.x);
                maxX = Mathf.Max(maxX, localPoint.x);
            }
        }

        // Adjust position if out of bounds
        float adjustment = 0;
        if (minX < -canvasSize.x / 2)
            adjustment = (-canvasSize.x / 2) - minX + 50f;
        else if (maxX > canvasSize.x / 2)
            adjustment = (canvasSize.x / 2) - maxX - 50f;

        if (adjustment != 0)
        {
            foreach (FormulaBlock block in chainBlocks)
            {
                block.rectTransform.anchoredPosition += new Vector2(adjustment, 0);
            }
        }
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
        canvasGroup.interactable = !locked;

        if (locked)
        {
            blockColor = BlockManager.variableColor;
            blockImage.color = Color.Lerp(blockColor, Color.gray, 0.5f);
        }
        else
        {
            blockImage.color = blockColor;
        }
    }

    public bool IsPartOfCompleteFormula()
    {
        // Check if this block is part of a complete formula chain
        FormulaBlock nBlock = FindVariableBlockInChain();
        if (nBlock == null) return false;

        return nBlock.HasLeftConnection && nBlock.HasRightConnection &&
               nBlock.rightConnectedBlock.HasRightConnection;
    }

    private FormulaBlock FindVariableBlockInChain()
    {
        List<FormulaBlock> chainBlocks = GetAllConnectedBlocks();
        return chainBlocks.Find(b => b.blockType == BlockType.Variable);
    }
}



