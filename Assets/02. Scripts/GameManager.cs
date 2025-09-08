using System.Collections.Generic;
using OMOK;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerType[,] board;
    public List<Block> blocks;
    public PlayerType currentPlayer;

    public int boardCol;
    public int boardRow;

    private void Start()
    {
        board = new PlayerType[boardCol, boardRow];
        StartGame();
    }

    public void StartGame()
    {
        currentPlayer = PlayerType.PlayerA;
    }

    public void SetPlayerOnBoard(int boardIndex)
    {
        var col = boardIndex % boardCol;
        var row = boardIndex / boardCol;

        if(board[col, row] == PlayerType.None)
        {
            // 흑돌일때 돌을 둘수있는지 확인
            if (currentPlayer == PlayerType.PlayerA && RenjuRule.IsForbidden(board, col, row))
            {
                board[col, row] = PlayerType.Forbidden;
                blocks[boardIndex].SetPlayerType(PlayerType.Forbidden);
                return;
            }

            board[col, row] = currentPlayer;
            blocks[boardIndex].SetPlayerType(currentPlayer);

            currentPlayer = currentPlayer == PlayerType.PlayerA ? PlayerType.PlayerB : PlayerType.PlayerA;
        }
    }
}