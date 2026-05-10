using System.Collections.Generic;
using _Data.Scripts.Controllers;
using _Data.Scripts.Enum;
using UnityEngine;

namespace _Data.Scripts.Services.Board
{
    public struct SpawnInfo
    {
        public SpecialEnum Special;
        public Vector2Int SpawnPos;
        public int ColorType; // 1-6
    }

    public class MatchService
    {
        private readonly List<Vector2Int> _dirs = new List<Vector2Int> { Vector2Int.up, Vector2Int.right };

        public void FindCombo(int boardSize, Transform holder, BoardService boardService)
        {
            var matched = FindAllMatches(boardSize, boardService);
            if (matched.Count == 0) return;
            boardService.RemoveMatch(matched);
        }

        public List<Vector2Int> FindAllMatches(int boardSize, BoardService boardService)
        {
            var allMatchedPositions = new HashSet<Vector2Int>();

            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    if (boardService.BitBoardData[x][y] <= 0) continue;

                    var find = FindMatch(boardService.BoardData[x][y], boardService.BitBoardData, out _);
                    for (int k = 0; k < find.Count; k++)
                    {
                        allMatchedPositions.Add(find[k]);
                    }
                }
            }

            return new List<Vector2Int>(allMatchedPositions);
        }

        public List<Vector2Int> FindMatch(GameObject go, int[][] bitBoard, out SpawnInfo spawnInfo)
        {
            spawnInfo = default;
            spawnInfo.Special = SpecialEnum.None;

            var comp = go.GetComponent<CandyController>();
            var pos = comp.Position;
            var type = comp.EnumType;

            if (type <= 0)
            {
                return new List<Vector2Int>();
            }

            // Base color: 1-6
            int baseColor = ((type - 1) % 6) + 1;

            var horizontal = FindHorizontal(pos, type, bitBoard);
            var vertical = FindVertical(pos, type, bitBoard);

            var find = new HashSet<Vector2Int>();
            for (int i = 0; i < horizontal.Count; i++)
            {
                find.Add(horizontal[i]);
            }
            
            for (int i = 0; i < vertical.Count; i++)
            {
                find.Add(vertical[i]);
            }

            if (find.Count == 0) return new List<Vector2Int>();

            spawnInfo.SpawnPos = pos;
            spawnInfo.ColorType = baseColor;

            // Priority: ColorBomb > Bomb > Horizontal > Vertical > None
            if (horizontal.Count >= 5 || vertical.Count >= 5)
            {
                spawnInfo.Special = SpecialEnum.ColorBomb;
            }
            else if (horizontal.Count >= 3 && vertical.Count >= 3)
            {
                spawnInfo.Special = SpecialEnum.Bomb;
            }
            else if (horizontal.Count == 4)
            {
                spawnInfo.Special = SpecialEnum.Horizontal;
            }
            else if (vertical.Count == 4)
            {
                spawnInfo.Special = SpecialEnum.Vertical;
            }

            return new List<Vector2Int>(find);
        }

        private List<Vector2Int> FindHorizontal(Vector2Int startPos, int type, int[][] bitBoard)
        {
            if (type <= 0) return new List<Vector2Int>();
            List<Vector2Int> positions = new List<Vector2Int>();
            positions.Add(startPos);

            var rightPos = startPos + Vector2Int.right;
            while (rightPos.x < bitBoard.Length && bitBoard[rightPos.x][rightPos.y] == type)
            {
                positions.Add(new Vector2Int(rightPos.x, rightPos.y));
                rightPos += Vector2Int.right;
            }

            var leftPos = startPos + Vector2Int.left;
            while (leftPos.x >= 0 && bitBoard[leftPos.x][leftPos.y] == type)
            {
                positions.Add(new Vector2Int(leftPos.x, leftPos.y));
                leftPos += Vector2Int.left;
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
            if (type <= 0) return new List<Vector2Int>();
            List<Vector2Int> positions = new List<Vector2Int>();
            positions.Add(startPos);

            var upPos = startPos + Vector2Int.up;
            while (upPos.y < bitBoard.Length && bitBoard[upPos.x][upPos.y] == type)
            {
                positions.Add(new Vector2Int(upPos.x, upPos.y));
                upPos += Vector2Int.up;
            }

            var downPos = startPos + Vector2Int.down;
            while (downPos.y >= 0 && bitBoard[downPos.x][downPos.y] == type)
            {
                positions.Add(new Vector2Int(downPos.x, downPos.y));
                downPos += Vector2Int.down;
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

        public SpawnInfo PickBestSpawnInfo(SpawnInfo a, SpawnInfo b)
        {
            int PriorityOf(SpecialEnum s) => s switch
            {
                SpecialEnum.ColorBomb => 4,
                SpecialEnum.Bomb => 3,
                SpecialEnum.Horizontal => 2,
                SpecialEnum.Vertical => 2,
                _ => 0
            };

            return PriorityOf(a.Special) >= PriorityOf(b.Special) ? a : b;
        }
    }
}