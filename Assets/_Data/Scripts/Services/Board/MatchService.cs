using System.Collections.Generic;
using _Data.Scripts.Controllers;
using UnityEngine;

namespace _Data.Scripts.Services.Board
{
    public class MatchService
    {
        public List<Vector2Int> FindMatch(GameObject go, int[][] bitBoard)
        {
            var comp = go.GetComponent<CandyController>();
            var pos = comp.Position;
            var type = comp.EnumType;

            var horizontal = FindHorizontal(pos, type, bitBoard);
            var vertical = FindVertical(pos, type, bitBoard);

            var find = new HashSet<Vector2Int>();
            if (horizontal != null)
            {
                for (int i = 0; i < horizontal.Count; i++)
                {
                    find.Add(horizontal[i]);
                }
            }

            if (vertical != null)
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
    }
}