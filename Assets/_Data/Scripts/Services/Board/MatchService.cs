using System.Collections.Generic;
using _Data.Scripts.Controllers;
using UnityEngine;

namespace _Data.Scripts.Services.Board
{
    public class MatchService
    {
        private readonly List<Vector2Int> _dirs = new List<Vector2Int> { new Vector2Int(1, 0), new Vector2Int(0, 1) };

        public void FindCombo(int boardSize, Transform holder, BoardService boardService)
        {
            while (true)
            {
                var matched = FindAllMatches(boardSize, boardService);

                if (matched.Count == 0) break;

                boardService.RemoveMatch(matched);
                boardService.Fall(boardSize);
                boardService.Fill(boardSize, holder, this);
            }
        }

        public List<Vector2Int> FindAllMatches(int boardSize, BoardService boardService)
        {
            var allMatchedPositions = new HashSet<Vector2Int>();

            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    if (boardService.BoardData[x][y] == null) continue;

                    var find = FindMatch(boardService.BoardData[x][y], boardService.BitBoardData);
                    for (int k = 0; k < find.Count; k++)
                    {
                        allMatchedPositions.Add(find[k]);
                    }
                }
            }

            return new List<Vector2Int>(allMatchedPositions);
        }

        public List<Vector2Int> FindMatch(GameObject go, int[][] bitBoard)
        {
            var comp = go.GetComponent<CandyController>();
            var pos = comp.Position;
            var type = comp.EnumType;

            var horizontal = FindHorizontal(pos, type, bitBoard);
            var vertical = FindVertical(pos, type, bitBoard);

            var find = new HashSet<Vector2Int>();
            if (horizontal.Count > 0)
            {
                for (int i = 0; i < horizontal.Count; i++)
                {
                    find.Add(horizontal[i]);
                }
            }

            if (vertical.Count > 0)
            {
                for (int i = 0; i < vertical.Count; i++)
                {
                    find.Add(vertical[i]);
                }
            }

            return new List<Vector2Int>(find);
        }

        private List<Vector2Int> FindHorizontal(Vector2Int startPos, int type, int[][] bitBoard)
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            positions.Add(startPos);

            var rightPos = startPos + new Vector2Int(1, 0);
            while (rightPos.x < bitBoard.Length && bitBoard[rightPos.x][rightPos.y] == type)
            {
                positions.Add(new Vector2Int(rightPos.x, rightPos.y));
                rightPos += new Vector2Int(1, 0);
            }

            var leftPos = startPos + new Vector2Int(-1, 0);
            while (leftPos.x >= 0 && bitBoard[leftPos.x][leftPos.y] == type)
            {
                positions.Add(new Vector2Int(leftPos.x, leftPos.y));
                leftPos += new Vector2Int(-1, 0);
            }

            if (positions.Count >= 3)
            {
                return positions;
            }
            else
            {
                return new List<Vector2Int>();
            }
        }

        private List<Vector2Int> FindVertical(Vector2Int startPos, int type, int[][] bitBoard)
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            positions.Add(startPos);

            var upPos = startPos + new Vector2Int(0, 1);
            while (upPos.y < bitBoard.Length && bitBoard[upPos.x][upPos.y] == type)
            {
                positions.Add(new Vector2Int(upPos.x, upPos.y));
                upPos += new Vector2Int(0, 1);
            }

            var downPos = startPos + new Vector2Int(0, -1);
            while (downPos.y >= 0 && bitBoard[downPos.x][downPos.y] == type)
            {
                positions.Add(new Vector2Int(downPos.x, downPos.y));
                downPos += new Vector2Int(0, -1);
            }

            if (positions.Count >= 3)
            {
                return positions;
            }
            else
            {
                return new List<Vector2Int>();
            }
        }

        public bool CanMove(int boardSize, BoardService boardService)
        {
            var bitBoard = boardService.BitBoardData; 

            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    for (int k = 0; k < _dirs.Count; k++)
                    {
                        var posA = new Vector2Int(x, y);
                        var posB = posA + _dirs[k];

                        if (posB.x < 0 || posB.x >= boardSize ||
                            posB.y < 0 || posB.y >= boardSize) continue;

                        if (CheckMatch(posA, posB, bitBoard)) return true;
                    }
                }
            }

            return false;
        }

        private bool CheckMatch(Vector2Int posA, Vector2Int posB, int[][] bitBoard)
        {
            int temp = bitBoard[posA.x][posA.y];
            bitBoard[posA.x][posA.y] = bitBoard[posB.x][posB.y];
            bitBoard[posB.x][posB.y] = temp;

            bool hasMatch =
                FindHorizontal(posA, bitBoard[posA.x][posA.y], bitBoard).Count > 0 ||
                FindVertical(posA, bitBoard[posA.x][posA.y], bitBoard).Count > 0 ||
                FindHorizontal(posB, bitBoard[posB.x][posB.y], bitBoard).Count > 0 ||
                FindVertical(posB, bitBoard[posB.x][posB.y], bitBoard).Count > 0;

            int tempB = bitBoard[posA.x][posA.y];
            bitBoard[posA.x][posA.y] = bitBoard[posB.x][posB.y];
            bitBoard[posB.x][posB.y] = tempB;

            return hasMatch;
        }
    }
}