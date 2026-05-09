using System.Collections.Generic;
using _Data.Scripts.Controllers;
using _Data.Scripts.Enum;
using Base.Systems.Pool;
using Base.Utilities;
using DG.Tweening;
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

        public Sequence GenerateBoard(int boardSize, Transform holder)
        {
            Sequence s = DOTween.Sequence();

            _boardData = new GameObject[boardSize][];
            _bitBoardData = new int[boardSize][];

            for (int i = 0; i < boardSize; i++)
            {
                _boardData[i] = new GameObject[boardSize];
                _bitBoardData[i] = new int[boardSize];
            }

            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    s.Append(Spawn(x, y, boardSize, holder));
                }
            }

            return s;
        }

        public void BoardShuffle(int boardSize)
        {
            _boardData.Shuffle();
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    var candy = _boardData[x][y];
                    candy.transform.position = GetSpawnPos(x, y, GetOffest(boardSize));
                    candy.GetComponent<CandyController>().SetPosition(new Vector2Int(x, y));
                    _bitBoardData[x][y] = candy.GetComponent<CandyController>().EnumType;
                }
            }
        }

        #endregion

        #region Core

        public void SwapData(GameObject first, GameObject second)
        {
            var firstComp = first.GetComponent<CandyController>();
            var firstPos = firstComp.Position;
            var secondComp = second.GetComponent<CandyController>();
            var secondPos = secondComp.Position;

            _boardData[firstPos.x][firstPos.y] = second;
            _boardData[secondPos.x][secondPos.y] = first;
            _bitBoardData[firstPos.x][firstPos.y] = secondComp.EnumType;
            _bitBoardData[secondPos.x][secondPos.y] = firstComp.EnumType;

            firstComp.SetPosition(secondPos);
            secondComp.SetPosition(firstPos);
        }

        public Sequence TrySwap(GameObject first, Vector2Int dir, MatchService matchService, out bool isMatch)
        {
            isMatch = false;
            var firstComp = first.GetComponent<CandyController>();
            var firstPos = firstComp.Position;
            var secondPos = firstPos + dir;

            if (secondPos.x < 0 || secondPos.x >= _boardData.Length ||
                secondPos.y < 0 || secondPos.y >= _boardData[0].Length) return null;

            _second = _boardData[secondPos.x][secondPos.y];

            var pos1 = first.transform.position;
            var pos2 = _second.transform.position;

            SwapData(first, _second);

            var find1 = matchService.FindMatch(first, _bitBoardData);
            var find2 = matchService.FindMatch(_second, _bitBoardData);

            Sequence s = DOTween.Sequence();

            if (find1.Count > 0 || find2.Count > 0)
            {
                isMatch = true;
                s.Append(first.transform.DOMove(pos2, 0.25f).SetEase(Ease.OutQuad));
                s.Join(_second.transform.DOMove(pos1, 0.25f).SetEase(Ease.OutQuad));
            }
            else
            {
                SwapData(first, _second);

                s.Append(first.transform.DOMove(pos2, 0.2f).SetEase(Ease.OutQuad));
                s.Join(_second.transform.DOMove(pos1, 0.2f).SetEase(Ease.OutQuad));

                s.Append(first.transform.DOMove(pos1, 0.2f).SetEase(Ease.InQuad));
                s.Join(_second.transform.DOMove(pos2, 0.2f).SetEase(Ease.InQuad));
            }

            return s;
        }

        public Sequence RemoveMatch(List<Vector2Int> pos)
        {
            Sequence s = DOTween.Sequence();

            var candyList = new List<GameObject>(pos.Count);

            for (int i = 0; i < pos.Count; i++)
            {
                var x = pos[i].x;
                var y = pos[i].y;

                candyList.Add(_boardData[x][y]);
                s.Join(_boardData[x][y].transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutQuad));

                _boardData[x][y] = null;
                _bitBoardData[x][y] = 0;
            }

            s.OnComplete(() =>
            {
                for (int i = 0; i < candyList.Count; i++)
                {
                    PoolManager.Ins.Despawn(candyList[i]);
                }
            });

            return s;
        }

        public Sequence Fall(int boardSize)
        {
            Sequence s = DOTween.Sequence();

            float offset = GetOffest(boardSize);

            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    if (_bitBoardData[x][y] == 0)
                    {
                        for (int k = y + 1; k < boardSize; k++)
                        {
                            if (_bitBoardData[x][k] != 0)
                            {
                                _boardData[x][y] = _boardData[x][k];
                                _bitBoardData[x][y] = _bitBoardData[x][k];

                                var candy = _boardData[x][y].GetComponent<CandyController>();
                                candy.SetPosition(new Vector2Int(x, y));
                                s.Join(candy.transform.DOMove(GetSpawnPos(x, y, offset), 0.25f).SetEase(Ease.OutQuad));

                                // _boardData[x][y].transform.position = GetSpawnPos(x, y, offset);
                                _boardData[x][k] = null;
                                _bitBoardData[x][k] = 0;

                                break;
                            }
                        }
                    }
                }
            }

            return s;
        }

        public void Fill(int boardSize, Transform holder, MatchService matchService)
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    if (_bitBoardData[x][y] == 0)
                    {
                        Spawn(x, y, boardSize, holder);
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

        Vector2 GetSpawnPos(int x, int y, float offset)
        {
            return new Vector2(x / 2f + offset, y / 2f + offset);
        }

        Tween Spawn(int x, int y, int boardSize, Transform holder)
        {
            var type = Random.Range(1, 7);
            var enumType = (CandyEnum)type;
            var name = enumType.ToString();

            var newCandy = PoolManager.Ins.Spawn(name, GetSpawnPos(x, y, GetOffest(boardSize)), Quaternion.identity);
            Tween spawn = newCandy.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutQuad);
            var comp = newCandy.GetComponent<CandyController>();
            comp.SetPosition(new Vector2Int(x, y));
            comp.SetEnumType(type);

            _boardData[x][y] = newCandy;
            _bitBoardData[x][y] = type;

            newCandy.transform.parent = holder;

            return spawn;
        }

        #endregion
    }
}