using UnityEngine;
using OMOK;
using System;

public class Block : MonoBehaviour
{
    public int blockIndex;

    public int col;
    public int row;

    public PlayerType playerType { get; private set; }
    public BlockType blockType { get; private set; }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer playerRenderer;

    private Action _onClickBoard;

    private void Awake()
    {
    }

    private void OnMouseDown()
    {
        GameManager.Instance.SetPlayerOnBoard(blockIndex);
    }

    public void SetPlayerType(PlayerType type)
    {
        playerType = type;
        switch (playerType)
        {
            case PlayerType.None:
                playerRenderer.color = new Color(0,0,0,0);
                break;
            case PlayerType.PlayerA:
                playerRenderer.color = Color.black;
                break;
            case PlayerType.PlayerB:
                playerRenderer.color = Color.white;
                break;
            case PlayerType.Forbidden:
                playerRenderer.color = Color.red;
                break;
        }
    }

    public void SetBlockType(BlockType type)
    {
        blockType = type;
        switch (blockType)
        {
            case BlockType.None:
                spriteRenderer.enabled = false;
                break;
            case BlockType.Gray:
                spriteRenderer.enabled = true;
                spriteRenderer.color = Color.gray;
                break;
            case BlockType.Yellow:
                spriteRenderer.enabled = true;
                spriteRenderer.color = Color.yellow;
                break;
        }
    }
}
