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
    private int maxCoefficientValue = 5;
    private int maxConstantValue = 5;
    
    [Header("Random Spawning Settings")]
    public bool useRandomSpawning = true;
    private int randomCoefficientCount = 2;
    private int randomConstantCount = 2;
    private Vector2 containerSize = new Vector2(1800f, 400f);
    private float blockSpacing = 100f; // Minimum distance between blocks
    
    private List<FormulaBlock> availableBlocks = new List<FormulaBlock>();
    private List<Vector2> usedPositions = new List<Vector2>();
    
    public List<FormulaBlock> AvailableBlocks => availableBlocks;
    
    public void CreateAllBlocks()
    {
        ClearExistingBlocks();
        usedPositions.Clear();

        //spawns correct blocks
        CreateRandomBlock(BlockType.Coefficient, StaticData.coefficient, "", coefficientColor);
        if (StaticData.constant < 0)
        {
            CreateRandomBlock(BlockType.Constant, -StaticData.constant, "", constantColor);
        }
        else
        {
            CreateRandomBlock(BlockType.Constant, StaticData.constant, "", constantColor);
        }
        
        //spawns signs
        CreateRandomBlock(BlockType.Sign, 0, "+", signColor);
        CreateRandomBlock(BlockType.Sign, 0, "-", signColor);
        


        if (useRandomSpawning)
        {
            CreateRandomBlocks();
        }
        else
        {
            CreateOrderedBlocks();
        }
    }
    
    private void CreateRandomBlocks()
    {
         int stage = StaticData.stageNum; 

        int randomCoefficientCount = StaticData.stageRandomCoefficientCount[stage];
        int maxCoefficientValue = StaticData.stageMaxCoefficientValue[stage];
        int randomConstantCount = StaticData.stageRandomConstantCount[stage];
        int maxConstantValue = StaticData.stageMaxConstantValue[stage];

        // Create random coefficient blocks
        for (int i = 0; i < randomCoefficientCount; i++)
        {
            int randomValue = Random.Range(1, maxCoefficientValue + 1);
            CreateRandomBlock(BlockType.Coefficient, randomValue, "", coefficientColor);
        }

        // Create random constant blocks
        for (int i = 0; i < randomConstantCount; i++)
        {
            int randomValue = Random.Range(0, maxConstantValue + 1);
            CreateRandomBlock(BlockType.Constant, randomValue, "", constantColor);
        }

        // Create variable block
        FormulaBlock nBlock = CreateRandomBlock(BlockType.Variable, 0, "n", variableColor);
        if (nBlock != null)
        {
            PositionVariableBlock(nBlock);
        }
    }
    
    private void CreateOrderedBlocks()
    {
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
    
    private FormulaBlock CreateRandomBlock(BlockType type, int value, string symbol, Color color)
    {
        FormulaBlock block = CreateBlock(type, value, symbol, color);
        if (block != null && type != BlockType.Variable) // Don't randomize variable block position
        {
            SetRandomPosition(block);
        }
        return block;
    }
    
    private void SetRandomPosition(FormulaBlock block)
    {
        RectTransform rectTransform = block.GetComponent<RectTransform>();
        Vector2 newPosition;
        int attempts = 0;
        int maxAttempts = 50; // Prevent infinite loop
        
        do
        {
            // Generate random position within container bounds
            float x = Random.Range(-containerSize.x / 2f, containerSize.x / 2f);
            float y = Random.Range(-containerSize.y / 2f, containerSize.y / 2f);
            newPosition = new Vector2(x, y);
            attempts++;
        }
        while (IsPositionTooClose(newPosition) && attempts < maxAttempts);
        
        rectTransform.anchoredPosition = newPosition;
        usedPositions.Add(newPosition);
    }
    
    private bool IsPositionTooClose(Vector2 position)
    {
        foreach (Vector2 usedPos in usedPositions)
        {
            if (Vector2.Distance(position, usedPos) < blockSpacing)
            {
                return true;
            }
        }
        return false;
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
        usedPositions.Clear();
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