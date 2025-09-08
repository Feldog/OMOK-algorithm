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

    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _playerRenderer;

    private Action _onClickBoard;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
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
                _playerRenderer.color = new Color(0,0,0,0);
                break;
            case PlayerType.PlayerA:
                _playerRenderer.color = Color.black;
                break;
            case PlayerType.PlayerB:
                _playerRenderer.color = Color.white;
                break;
            case PlayerType.Forbidden:
                _playerRenderer.color = Color.red;
                break;
        }
    }

    public void SetBlockType(BlockType type)
    {
        blockType = type;
        switch (blockType)
        {
            case BlockType.None:
                _spriteRenderer.enabled = false;
                break;
            case BlockType.Gray:
                _spriteRenderer.enabled = true;
                _spriteRenderer.color = Color.gray;
                break;
            case BlockType.Yellow:
                _spriteRenderer.enabled = true;
                _spriteRenderer.color = Color.yellow;
                break;
        }
    }
}
