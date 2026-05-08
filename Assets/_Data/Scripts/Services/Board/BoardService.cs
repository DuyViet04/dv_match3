using System.Collections.Generic;
using _Data.Scripts.Controllers;
using _Data.Scripts.Enum;
using Base.Systems.Pool;
using Base.Utilities;
using UnityEngine;

namespace _Data.Scripts.Services.Board
{
    public class BoardService
    {
        private GameObject[][] _boardData;
        private int[][] _bitBoardData;
        private GameObject _second;

        public GameObject[][] BoardData => _boardData;
        public int[][] BitBoardData => _bitBoardData;
        public GameObject Second => _second;

        #region Board Generation

        public void GenerateBoard(int boardSize, Transform holder)
        {
            _boardData = new GameObject[boardSize][];
            _bitBoardData = new int[boardSize][];

            for (int i = 0; i < boardSize; i++)
            {
                _boardData[i] = new GameObject[boardSize];
                _bitBoardData[i] = new int[boardSize];

                for (int j = 0; j < boardSize; j++)
                {
                    Spawn(i, j, boardSize, holder);
                }
            }
        }

        public void BoardShuffle(int boardSize)
        {
            _boardData.Shuffle();
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    _boardData[i][j].transform.position = GetSpawnPos(i, j, GetOffest(boardSize));
                    _boardData[i][j].GetComponent<CandyController>().SetPosition(new Vector2Int(i, j));
                    _bitBoardData[i][j] = _boardData[i][j].GetComponent<CandyController>().EnumType;
                }
            }
        }

        #endregion

        #region Core

        public void Swap(GameObject first, Vector2Int dir)
        {
            var firstComp = first.GetComponent<CandyController>();
            var firstPos = firstComp.Position;

            var secondPos = firstPos + dir;
            if ((secondPos.x < 0 || secondPos.x >= _boardData.Length) ||
                (secondPos.y < 0 || secondPos.y >= _boardData[0].Length)) return;
            _second = _boardData[secondPos.x][secondPos.y];
            var secondComp = _second.GetComponent<CandyController>();

            (first.transform.position, _second.transform.position) =
                (_second.transform.position, first.transform.position);

            _boardData[firstPos.x][firstPos.y] = _second;
            _boardData[secondPos.x][secondPos.y] = first;
            _bitBoardData[firstPos.x][firstPos.y] = secondComp.EnumType;
            _bitBoardData[secondPos.x][secondPos.y] = firstComp.EnumType;

            firstComp.SetPosition(secondPos);
            secondComp.SetPosition(firstPos);
        }

        public bool TrySwap(GameObject first, Vector2Int dir, MatchService matchService)
        {
            Swap(first, dir);
            var hs = new HashSet<Vector2Int>();
            var find1 = matchService.FindMatch(first, _bitBoardData);
            var find2 = matchService.FindMatch(_second, _bitBoardData);
            for (int i = 0; i < find1.Count; i++)
            {
                hs.Add(find1[i]);
            }

            for (int i = 0; i < find2.Count; i++)
            {
                hs.Add(find2[i]);
            }

            var total = new List<Vector2Int>(hs);
            if (total.Count > 0)
            {
                return true;
            }
            else
            {
                Swap(first, -dir);
                return false;
            }
        }

        public void RemoveMatch(List<Vector2Int> pos)
        {
            for (int i = 0; i < pos.Count; i++)
            {
                PoolManager.Ins.Despawn(_boardData[pos[i].x][pos[i].y]);
                _boardData[pos[i].x][pos[i].y] = null;
                _bitBoardData[pos[i].x][pos[i].y] = 0;
            }
        }

        public void Fall(int boardSize)
        {
            float offset = GetOffest(boardSize);

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (_bitBoardData[i][j] == 0)
                    {
                        for (int k = j + 1; k < boardSize; k++)
                        {
                            if (_bitBoardData[i][k] != 0)
                            {
                                _boardData[i][j] = _boardData[i][k];
                                _bitBoardData[i][j] = _bitBoardData[i][k];

                                var candy = _boardData[i][j].GetComponent<CandyController>();
                                candy.SetPosition(new Vector2Int(i, j));
                                _boardData[i][j].transform.position = GetSpawnPos(i, j, offset);

                                _boardData[i][k] = null;
                                _bitBoardData[i][k] = 0;

                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Fill(int boardSize, Transform holder, MatchService matchService)
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (_bitBoardData[i][j] == 0)
                    {
                        Spawn(i, j, boardSize, holder);
                    }
                }
            }

            while (!matchService.CanMove(boardSize, this))
            {
                BoardShuffle(boardSize);
            }
        }

        #endregion

        #region Helper

        float GetOffest(int boardSize)
        {
            float offest = -(boardSize - 1) / 4f;
            return offest;
        }

        Vector2 GetSpawnPos(int i, int j, float offset)
        {
            return new Vector2(i / 2f + offset, j / 2f + offset);
        }

        void Spawn(int i, int j, int boardSize, Transform holder)
        {
            var type = Random.Range(1, 7);
            var enumType = (CandyEnum)type;
            var name = enumType.ToString();

            var newCandy = PoolManager.Ins.Spawn(name, GetSpawnPos(i, j, GetOffest(boardSize)), Quaternion.identity);
            var comp = newCandy.GetComponent<CandyController>();
            comp.SetPosition(new Vector2Int(i, j));
            comp.SetEnumType(type);

            _boardData[i][j] = newCandy;
            _bitBoardData[i][j] = type;

            newCandy.transform.parent = holder;
        }

        #endregion
    }
}