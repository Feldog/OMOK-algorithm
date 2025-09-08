using UnityEngine;
using OMOK;
using UnityEngine.UIElements;

public static class RenjuRule
{
    private static (int x, int y)[] directions =
    {
        (1, 0),     // �¿� Ž��
        (0, 1),     // �� �Ʒ� Ž��
        (1, 1),     // ������ ���� �밢�� Ž��
        (1, -1)     // �� ������ ���� �밢�� Ž��
    };


    /// <summary>
    /// x,y ��ǥ�� �浹�� ���� �� �ִ� �� �Ǻ�
    /// </summary>
    /// <param name="board">������ ���� ���� �����Ͱ�</param>
    /// <param name="col">������ x ��ǥ</param>
    /// <param name="row">������ y ��ǥ</param>
    /// <returns></returns>
    public static bool IsForbidden(PlayerType[,] board, int col, int row)
    {

        // �ٸ� �÷��̾ ��ġ�� �ڸ��϶�
        if (board[col,row] == PlayerType.PlayerB) return false;

        board[col, row] = PlayerType.PlayerA;

        for(int i = 0; i < 4;i++)
        {
            int count = CountLine(col, row, i, board);
            // �ٷ� 5���� �� ���� ��� ����
            if (count == 5)
            {
                board[col, row] = PlayerType.None;
                return false;
            }
            // 6�� �̻��� ��� ���ַ꿡 ���� �Ѽ� ����
            else if (count >= 6) 
            {
                board[col, row] = PlayerType.Forbidden;
                return true;
            }
        }

        
        // 3-3, 4-4�� ��� ���ַ꿡 ���� �Ѽ� ����
        if(IsDoubleThreeOrFour(col, row, board))
        {
            board[col, row] = PlayerType.Forbidden;
            return true;
        }

        board[col, row] = PlayerType.None;
        return false;
    }

    public static int CountLine(int col, int row, int dirIndex,PlayerType[,] board)
    {
        if (board[col, row] != PlayerType.PlayerA) { return 0; }
        int count = 1;

        for(int i = 1; i < 6; i++)
        {
            // ������
            var dirCol = col + directions[dirIndex].x * i;
            var dirRow = row + directions[dirIndex].y * i;

            // ������ ������ ����ų� �浹�� �ƴ� ���� ������ ����
            if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                break;

            // ���ӵ� �浹�� ���� �߰�
            count++;
        }

        for(int i = 1; i < 6;i++)
        {
            // ������
            var dirCol = col - directions[dirIndex].x * i;
            var dirRow = row - directions[dirIndex].y * i;

            // ������ ������ ����ų� �浹�� �ƴ� �鵹�� ������ �ݺ� ����
            if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                break;

            // ���ӵ� �浹�� ���� �߰�
            count++;
        }

        // ī��Ʈ�� �浹�� ���� ��ȯ
        return count;
    }

    // 3-3, 4-4�� ������ �ľ��ϴ� �Լ�
    public static bool IsDoubleThreeOrFour(int col, int row, PlayerType[,] board)
    {
        int threeCount = 0;
        int fourCount = 0;

        for(int i = 0; i < 4; i++)
        {
            // ���� ����� ���� �ϳ��ϳ� Ȯ���Ͽ� ����� ��ȯ
            (int fours, int threes) = CheckOpenThreeOrFour(col, row, i, board);
            fourCount += fours;
            threeCount += threes;
        }

        if (fourCount >= 2) return true;
        if (threeCount >= 2) return true;

        return false;
    }

    // ���� 4�� ������ִ� 3�� 4�� ��츦 Ž���ϴ� �Լ�
    public static (int fours, int threes) CheckOpenThreeOrFour(int col, int row, int dirIndex, PlayerType[,] board)
    {
        // ���� ��ġ�� ������ dir ���� ���� 4���� �����͸� ����
        var line = new PlayerType[9];

        for(int i = -4; i <= 4; i++)
        {
            int dirCol = col + directions[dirIndex].x * i;
            int dirRow = row + directions[dirIndex].y * i;

            // �迭�� �����ϱ� ���� offset����
            int index = i + 4;

            if(CheckBoardInside(dirCol, dirRow, board))
            {
                // ������ ������ ����� ���� �鵹�� ����Ͽ��� �������
                line[index] = PlayerType.PlayerB;
            }
            else
            {
                line[index] = board[dirCol,dirRow];
            }
        }
        int fourConunt = 0;
        int threeCount = 0;

        // 0~4 / 1~5 / 2~6 / 3~7 / 4~8  5���� �ε����� �浹�� 4���� ����ִ� ��츦 ã��
        for (int i = 0; i <= 4; i++)
        {
            int count = 0;
            for (int j = 0; j < 5; j++)
            {
                if (line[j + i] == PlayerType.PlayerA) count++;
            }

            // �ε����� 4���� ���������� 4�� ��ȯ
            if (count == 4) fourConunt++;
        }

        // ���� 4�� ������ִ� 3(XOOOX)�� ��� üũ (���� �鵹�� �ְų� ������ ���鿡 �پ������� ����)
        // i�� ���� 0�� 4�� ��쿡�� 4�� �ε����� �������� �ξ����� �񱳴���� ������ None�� ���Ͽ� �� ���ʿ�
        for(int i = 1;i <= 3;i++)
        {
            if (line[i] == PlayerType.None &&
                line[i + 1] == PlayerType.PlayerA &&
                line[i + 2] == PlayerType.PlayerA &&
                line[i + 3] == PlayerType.PlayerA &&
                line[i + 4] == PlayerType.None)
            {
                threeCount++;
            }
        }

        // ����� 3�� ã�� ���� Ž�� XOXOOX (�� ���� ����ְ� ������ ���� 4�� ������ִ� 3)
        for(int i = 0; i <= 3; i++)
        {
            if (line[i] == PlayerType.None &&
                line[i + 1] == PlayerType.PlayerA &&
                line[i + 2] == PlayerType.None &&
                line[i + 3] == PlayerType.PlayerA &&
                line[i + 4] == PlayerType.PlayerA &&
                line[i + 5] == PlayerType.None)
            {
                threeCount++;
            }
        }

        // ���� ��쿡 ���� ����
        // ����� 3�� ã�� ���� Ž�� XOOXOX (�� ���� ����ְ� ������ ���� 4�� ������ִ� 3)
        for (int i = 0; i <= 3; i++)
        {
            if (line[i] == PlayerType.None &&
                line[i + 1] == PlayerType.PlayerA &&
                line[i + 2] == PlayerType.PlayerA &&
                line[i + 3] == PlayerType.None &&
                line[i + 4] == PlayerType.PlayerA &&
                line[i + 5] == PlayerType.None)
            {
                threeCount++;
            }
        }

        // 4�� ���� ��� 4 3�̹Ƿ� �ݼ��� �ƴ� 4�� ����� ���� Ȯ��
        if (fourConunt > 0)
        {
            return (fourConunt, 0);
        }

        return (fourConunt, threeCount);
    }

    // �ش� ��ǥ�� ���忡�� ������� Ȯ���ϴ� �Լ�
    public static bool CheckBoardInside(int col, int row, PlayerType[,] board)
    {
        return col < 0 || row < 0 || col > board.GetLength(0) - 1 || row > board.GetLength(1) - 1;
    }

    
    /// <summary>
    /// �ش� �Լ��� 3/3 4/4 Ȯ�ο����� ��������� 3/3�� ã�� �������� ���� 3 ���� Ȯ���� �������
    /// �Լ� ���
    /// </summary>
    /// <param name="col">x��ǥ</param>
    /// <param name="row">y��ǥ</param>
    /// <param name="dirIndex">Ž�� ����</param>
    /// <param name="board">���� ������</param>
    /// <returns></returns>
    
    // None�̳� Forbidden�� ����� ��� ��ĭ �ʸ��� �ڸ��� ���ӵǴ� �浹�� ���� Ȯ��
    public static (int, int) OverLineCount(int col, int row, int dirIndex, PlayerType[,] board)
    {
        // �� �ʱ�ȭ
        int forward = 0, backward = 0;

        //������
        for(int i = 1; i < 4; i++)
        {
            var dirCol = col + directions[dirIndex].x * i;
            var dirRow = row + directions[dirIndex].y * i;

            // ��ǥ���� ����ų� �浹�� �ƴѰ��� ������
            if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                break;

            if (board[dirCol, dirRow] == PlayerType.None || board[dirCol, dirRow] == PlayerType.Forbidden)
            {
                // �� ������ ã�����Ƿ� ��ĭ �ǳʶ�
                dirCol += directions[dirIndex].x;
                dirRow += directions[dirIndex].y;
                
                // ���� �ִ� 3�� �������� �����ϴ��� Ȯ����
                for(int j = 0; j < 3; j++)
                { 
                    // ��ǥ���� ����ų� �浹�� �ƴѰ��� ������
                    if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                        break;

                    forward++;

                    // ������
                    dirCol += directions[dirIndex].x;
                    dirRow += directions[dirIndex].y;
                }
                break;
            }
        }
        // ������
        for (int i = 1; i < 4; i++)
        {
            var dirCol = col - directions[dirIndex].x * i;
            var dirRow = row - directions[dirIndex].y * i;

            // ��ǥ���� ����ų� �浹�� �ƴѰ��� ������ ����
            if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                break;

            if (board[dirCol, dirRow] == PlayerType.None || board[dirCol, dirRow] == PlayerType.Forbidden)
            {
                // �� ������ ã�����Ƿ� ��ĭ �ǳʶ�
                dirCol -= directions[dirIndex].x;
                dirRow -= directions[dirIndex].y;

                // ���� �ִ� 3�� �������� �����ϴ��� Ȯ����
                for (int j = 0; j < 3; j++)
                {
                    backward++;

                    // ������
                    dirCol -= directions[dirIndex].x;
                    dirRow -= directions[dirIndex].y;

                    // ��ǥ���� ����ų� �浹�� �ƴѰ��� ������ ����
                    if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                        break;
                }
                break;
            }
        }

        return (forward, backward);
    }

}
