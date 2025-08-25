using UnityEngine;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour
{
    [Header("Block Creation Settings")]
    public GameObject blockPrefab;
    public Transform blockContainer;
    
    [Header("Block Colors")]
    public Color coefficientColor = Color.blue;
    public Color signColor = Color.yellow;
    public Color constantColor = Color.green;
    public Color variableColor = Color.red;
    
    [Header("Block Ranges")]
    public int maxCoefficientValue = 5;
    public int maxConstantValue = 5;
    
    private List<FormulaBlock> availableBlocks = new List<FormulaBlock>();
    
    public List<FormulaBlock> AvailableBlocks => availableBlocks;
    
    public void CreateAllBlocks()
    {
        ClearExistingBlocks();
        
        // Create coefficient blocks (1 to maxCoefficientValue)
        for (int i = 1; i <= maxCoefficientValue; i++)
        {
            CreateBlock(BlockType.Coefficient, i, "", coefficientColor);
        }
        
        // Create sign blocks
        CreateBlock(BlockType.Sign, 0, "+", signColor);
        CreateBlock(BlockType.Sign, 0, "-", signColor);
        
        // Create constant blocks (0 to maxConstantValue)
        for (int i = 0; i <= maxConstantValue; i++)
        {
            CreateBlock(BlockType.Constant, i, "", constantColor);
        }
        
        // Create the variable block (always "n") - place it in a special position
        FormulaBlock nBlock = CreateBlock(BlockType.Variable, 0, "n", variableColor);
        if (nBlock != null)
        {
            PositionVariableBlock(nBlock);
        }
    }
    
    private void PositionVariableBlock(FormulaBlock nBlock)
    {
        // Position the "n" block in the center of the formula building area
        RectTransform nRect = nBlock.GetComponent<RectTransform>();
        
        // Find or create a formula building area
        GameObject formulaArea = GameObject.Find("FormulaArea");
        if (formulaArea == null)
        {
            formulaArea = new GameObject("FormulaArea");
            formulaArea.transform.SetParent(blockContainer.parent);
            
            RectTransform areaRect = formulaArea.AddComponent<RectTransform>();
            areaRect.anchoredPosition = new Vector2(0, 100f); // Above the block container
            areaRect.sizeDelta = new Vector2(400f, 100f);
        }
        
        nBlock.transform.SetParent(formulaArea.transform);
        nRect.anchoredPosition = Vector2.zero;
    }
    
    public void ClearExistingBlocks()
    {
        foreach (FormulaBlock block in availableBlocks)
        {
            if (block != null)
                DestroyImmediate(block.gameObject);
        }
        availableBlocks.Clear();
    }
    
    private FormulaBlock CreateBlock(BlockType type, int value, string symbol, Color color)
    {
        if (blockPrefab == null || blockContainer == null)
        {
            Debug.LogError("BlockManager: Block prefab or container not assigned!");
            return null;
        }
        
        GameObject blockObj = Instantiate(blockPrefab, blockContainer);
        FormulaBlock block = blockObj.GetComponent<FormulaBlock>();
        
        if (block != null)
        {
            block.Initialize(type, value, symbol, color);
            availableBlocks.Add(block);
            return block;
        }
        else
        {
            Debug.LogError("BlockManager: Block prefab doesn't have FormulaBlock component!");
            DestroyImmediate(blockObj);
            return null;
        }
    }
    
    public FormulaBlock FindBlock(BlockType type, int value, string symbol = "")
    {
        return availableBlocks.Find(b => 
            b.blockType == type && 
            (type == BlockType.Sign ? b.symbol == symbol : b.value == value));
    }
    
    public List<FormulaBlock> GetBlocksByType(BlockType type)
    {
        return availableBlocks.FindAll(b => b.blockType == type);
    }
    
    public void ResetAllBlocks()
    {
        foreach (FormulaBlock block in availableBlocks)
        {
            if (block != null && !block.IsLocked)
            {
                block.ReturnToOriginalPosition();
            }
        }
    }
    
    public void SetBlocksInteractable(bool interactable)
    {
        foreach (FormulaBlock block in availableBlocks)
        {
            if (block != null && !block.IsLocked)
            {
                block.GetComponent<CanvasGroup>().interactable = interactable;
            }
        }
    }
    
    public int GetAvailableBlockCount(BlockType type)
    {
        int count = 0;
        foreach (FormulaBlock block in availableBlocks)
        {
            if (block != null && block.blockType == type && !block.IsSnapped())
            {
                count++;
            }
        }
        return count;
    }
    
    public void OrganizeBlocks()
    {
        // Organize blocks by type in the container
        List<FormulaBlock> coefficientBlocks = GetBlocksByType(BlockType.Coefficient);
        List<FormulaBlock> signBlocks = GetBlocksByType(BlockType.Sign);
        List<FormulaBlock> constantBlocks = GetBlocksByType(BlockType.Constant);
        List<FormulaBlock> variableBlocks = GetBlocksByType(BlockType.Variable);
        
        int index = 0;
        
        // Arrange coefficient blocks
        foreach (FormulaBlock block in coefficientBlocks)
        {
            if (!block.IsSnapped())
            {
                block.transform.SetSiblingIndex(index++);
            }
        }
        
        // Arrange variable blocks
        foreach (FormulaBlock block in variableBlocks)
        {
            if (!block.IsSnapped())
            {
                block.transform.SetSiblingIndex(index++);
            }
        }
        
        // Arrange sign blocks
        foreach (FormulaBlock block in signBlocks)
        {
            if (!block.IsSnapped())
            {
                block.transform.SetSiblingIndex(index++);
            }
        }
        
        // Arrange constant blocks
        foreach (FormulaBlock block in constantBlocks)
        {
            if (!block.IsSnapped())
            {
                block.transform.SetSiblingIndex(index++);
            }
        }
    }
}