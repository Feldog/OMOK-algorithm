using UnityEngine;
using OMOK;
using UnityEngine.UIElements;

public static class RenjuRule
{
    private static (int x, int y)[] directions =
    {
        (1, 0),     // 좌우 탐색
        (0, 1),     // 위 아래 탐색
        (1, 1),     // 슬래쉬 방향 대각선 탐색
        (1, -1)     // 역 슬래쉬 방향 대각선 탐색
    };


    /// <summary>
    /// x,y 좌표에 흑돌이 놓을 수 있는 지 판별
    /// </summary>
    /// <param name="board">보드의 현재 상태 데이터값</param>
    /// <param name="col">보드의 x 좌표</param>
    /// <param name="row">보드의 y 좌표</param>
    /// <returns></returns>
    public static bool IsForbidden(PlayerType[,] board, int col, int row)
    {

        // 다른 플레이어가 설치한 자리일때
        if (board[col,row] == PlayerType.PlayerB) return false;

        board[col, row] = PlayerType.PlayerA;

        for(int i = 0; i < 4;i++)
        {
            int count = CountLine(col, row, i, board);
            // 바로 5목이 될 경우는 상관 없음
            if (count == 5)
            {
                board[col, row] = PlayerType.None;
                return false;
            }
            // 6목 이상일 경우 렌주룰에 의해 둘수 없음
            else if (count >= 6) 
            {
                board[col, row] = PlayerType.Forbidden;
                return true;
            }
        }

        
        // 3-3, 4-4일 경우 렌주룰에 의해 둘수 없음
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
            // 정방향
            var dirCol = col + directions[dirIndex].x * i;
            var dirRow = row + directions[dirIndex].y * i;

            // 보드의 범위를 벗어나거나 흑돌이 아닌 돌을 만나면 종료
            if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                break;

            // 연속된 흑돌의 갯수 추가
            count++;
        }

        for(int i = 1; i < 6;i++)
        {
            // 역방향
            var dirCol = col - directions[dirIndex].x * i;
            var dirRow = row - directions[dirIndex].y * i;

            // 보드의 범위를 벗어나거나 흑돌이 아닌 백돌을 만나면 반복 종료
            if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                break;

            // 연속된 흑돌의 갯수 추가
            count++;
        }

        // 카운트된 흑돌의 갯수 반환
        return count;
    }

    // 3-3, 4-4의 갯수를 파악하는 함수
    public static bool IsDoubleThreeOrFour(int col, int row, PlayerType[,] board)
    {
        int threeCount = 0;
        int fourCount = 0;

        for(int i = 0; i < 4; i++)
        {
            // 여러 경우의 수를 하나하나 확인하여 결과를 반환
            (int fours, int threes) = CheckOpenThreeOrFour(col, row, i, board);
            fourCount += fours;
            threeCount += threes;
        }

        if (fourCount >= 2) return true;
        if (threeCount >= 2) return true;

        return false;
    }

    // 열린 4를 만들수있는 3과 4의 경우를 탐색하는 함수
    public static (int fours, int threes) CheckOpenThreeOrFour(int col, int row, int dirIndex, PlayerType[,] board)
    {
        // 새로 배치한 돌에서 dir 기준 양쪽 4개의 데이터를 수집
        var line = new PlayerType[9];

        for(int i = -4; i <= 4; i++)
        {
            int dirCol = col + directions[dirIndex].x * i;
            int dirRow = row + directions[dirIndex].y * i;

            // 배열에 접근하기 위해 offset조절
            int index = i + 4;

            if(CheckBoardInside(dirCol, dirRow, board))
            {
                // 보드의 범위를 벗어나는 값은 백돌로 취급하여서 예외취급
                line[index] = PlayerType.PlayerB;
            }
            else
            {
                line[index] = board[dirCol,dirRow];
            }
        }
        int fourConunt = 0;
        int threeCount = 0;

        // 0~4 / 1~5 / 2~6 / 3~7 / 4~8  5개의 인덱스에 흑돌이 4개가 들어있는 경우를 찾기
        for (int i = 0; i <= 4; i++)
        {
            int count = 0;
            for (int j = 0; j < 5; j++)
            {
                if (line[j + i] == PlayerType.PlayerA) count++;
            }

            // 인덱스에 4개가 들어있을경우 4를 반환
            if (count == 4) fourConunt++;
        }

        // 열린 4를 만들수있는 3(XOOOX)의 경우 체크 (끝에 백돌이 있거나 보드의 벽면에 붙어있으면 예외)
        // i의 값이 0과 4일 경우에는 4번 인덱스에 검은돌을 두었으나 비교대상이 무조건 None과 비교하여 비교 불필요
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

        // 띄어진 3을 찾기 위한 탐색 XOXOOX (끝 쪽은 비어있고 떨어진 열린 4를 만들수있는 3)
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

        // 위의 경우에 수의 역순
        // 띄어진 3을 찾기 위한 탐색 XOOXOX (끝 쪽은 비어있고 떨어진 열린 4를 만들수있는 3)
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

        // 4가 있을 경우 4 3이므로 금수가 아님 4의 경우의 수만 확인
        if (fourConunt > 0)
        {
            return (fourConunt, 0);
        }

        return (fourConunt, threeCount);
    }

    // 해당 좌표가 보드에서 벗어나는지 확인하는 함수
    public static bool CheckBoardInside(int col, int row, PlayerType[,] board)
    {
        return col < 0 || row < 0 || col > board.GetLength(0) - 1 || row > board.GetLength(1) - 1;
    }

    
    /// <summary>
    /// 해당 함수는 3/3 4/4 확인용으로 만들었으나 3/3을 찾는 과정에서 열린 3 인지 확인이 어려웠음
    /// 함수 폐기
    /// </summary>
    /// <param name="col">x좌표</param>
    /// <param name="row">y좌표</param>
    /// <param name="dirIndex">탐색 방향</param>
    /// <param name="board">보드 데이터</param>
    /// <returns></returns>
    
    // None이나 Forbidden에 닿았을 경우 한칸 너머의 자리로 연속되는 흑돌의 갯수 확인
    public static (int, int) OverLineCount(int col, int row, int dirIndex, PlayerType[,] board)
    {
        // 값 초기화
        int forward = 0, backward = 0;

        //정방향
        for(int i = 1; i < 4; i++)
        {
            var dirCol = col + directions[dirIndex].x * i;
            var dirRow = row + directions[dirIndex].y * i;

            // 좌표에서 벗어나거나 흑돌이 아닌것을 만나면
            if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                break;

            if (board[dirCol, dirRow] == PlayerType.None || board[dirCol, dirRow] == PlayerType.Forbidden)
            {
                // 빈 공간을 찾았으므로 한칸 건너띔
                dirCol += directions[dirIndex].x;
                dirRow += directions[dirIndex].y;
                
                // 이후 최대 3개 연속으로 존재하는지 확인함
                for(int j = 0; j < 3; j++)
                { 
                    // 좌표에서 벗어나거나 흑돌이 아닌것을 만나면
                    if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                        break;

                    forward++;

                    // 정방향
                    dirCol += directions[dirIndex].x;
                    dirRow += directions[dirIndex].y;
                }
                break;
            }
        }
        // 역방향
        for (int i = 1; i < 4; i++)
        {
            var dirCol = col - directions[dirIndex].x * i;
            var dirRow = row - directions[dirIndex].y * i;

            // 좌표에서 벗어나거나 흑돌이 아닌것을 만나면 종료
            if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                break;

            if (board[dirCol, dirRow] == PlayerType.None || board[dirCol, dirRow] == PlayerType.Forbidden)
            {
                // 빈 공간을 찾았으므로 한칸 건너띔
                dirCol -= directions[dirIndex].x;
                dirRow -= directions[dirIndex].y;

                // 이후 최대 3개 연속으로 존재하는지 확인함
                for (int j = 0; j < 3; j++)
                {
                    backward++;

                    // 역방향
                    dirCol -= directions[dirIndex].x;
                    dirRow -= directions[dirIndex].y;

                    // 좌표에서 벗어나거나 흑돌이 아닌것을 만나면 종료
                    if (CheckBoardInside(dirCol, dirRow, board) || board[dirCol, dirRow] != PlayerType.PlayerA)
                        break;
                }
                break;
            }
        }

        return (forward, backward);
    }

}
