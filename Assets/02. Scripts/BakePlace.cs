using UnityEngine;
using OMOK;
using UnityEditor.Experimental.GraphView;

public class BakePlace : MonoBehaviour
{
    [SerializeField] private float blockSize;
    
    [SerializeField] private GameObject blocks;
    [SerializeField] private Vector2 offsetBoard;

    // ���� ����
    private void Start()
    {
        BakeBoard();
    }

    // ���� �����
    private void BakeBoard()
    {
        int blockIndex = 0;

        var boardRow = GameManager.Instance.boardRow;
        var boardCol = GameManager.Instance.boardCol;

        offsetBoard = new Vector2(boardRow * blockSize / -2, boardCol * blockSize / -2);

        for (int row = 0; row < boardRow; row++)
        {
            for (int col = 0; col < boardCol; col++)
            {
                // col�� x, row�� y
                Vector2 temp = new Vector2(col * blockSize, row * blockSize);
                BlockInstantiate(blockIndex++, col + row, temp);
            }
        }
    }

    // �� ����
    private void BlockInstantiate(int blockIndex, int blockNum, Vector2 pos)
    {
        var obj = Instantiate(blocks, pos + offsetBoard, Quaternion.identity);
        var block = obj.GetComponent<Block>();

        block.blockIndex = blockIndex;
        block.SetBlockType((BlockType)(blockNum % 2 + 1));
        block.SetPlayerType(PlayerType.None);

        // ������ ���� ���ӸŴ����� ����
        GameManager.Instance.blocks.Add(block);

        obj.name = $"Block {blockIndex}";
        obj.transform.SetParent(transform);
    }
}
