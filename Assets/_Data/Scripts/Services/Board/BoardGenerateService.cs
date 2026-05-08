using _Data.Scripts.Enum;
using Base.Systems.Pool;
using Base.Utilities;
using UnityEngine;

namespace _Data.Scripts.Services.Board
{
    public class BoardGenerateService
    {
        private GameObject[][] _boardData;

        public void GenerateBoard(int boardSize, Transform holder)
        {
            _boardData = new GameObject[boardSize][];

            for (int i = 0; i < boardSize; i++)
            {
                _boardData[i] = new GameObject[boardSize];
                for (int j = 0; j < boardSize; j++)
                {
                    int idx = Random.Range(1, 7);
                    CandyEnum candyEnum = (CandyEnum)idx;
                    string candyName = candyEnum.ToString();

                    GameObject newPrefab = PoolManager.Ins.Spawn(candyName, GetSpawnPos(i, j, GetOffest(boardSize)),
                        Quaternion.identity);
                    _boardData[i][j] = newPrefab;
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
                }
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