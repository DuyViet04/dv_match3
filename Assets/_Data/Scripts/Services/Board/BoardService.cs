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

        public int[][] BitBoardData => _bitBoardData;
        public GameObject Second => _second;

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
                    int idx = Random.Range(1, 7);
                    CandyEnum candyEnum = (CandyEnum)idx;
                    string candyName = candyEnum.ToString();

                    GameObject newPrefab = PoolManager.Ins.Spawn(candyName, GetSpawnPos(i, j, GetOffest(boardSize)),
                        Quaternion.identity);

                    CandyController candyController = newPrefab.GetComponent<CandyController>();
                    candyController.SetPosition(new Vector2Int(i, j));
                    candyController.SetEnumType(idx);
                    _boardData[i][j] = newPrefab;
                    _bitBoardData[i][j] = idx;
                    newPrefab.transform.parent = holder;
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

        public void RemoveMatch(List<Vector2Int> pos)
        {
            for (int i = 0; i < pos.Count; i++)
            {
                PoolManager.Ins.Despawn(_boardData[pos[i].x][pos[i].y]);
                _bitBoardData[pos[i].x][pos[i].y] = 0;
            }
        }

        float GetOffest(int boardSize)
        {
            float offest = -(boardSize - 1) / 4f;
            return offest;
        }

        Vector2 GetSpawnPos(int i, int j, float offset)
        {
            return new Vector2(i / 2f + offset, j / 2f + offset);
        }
    }
}