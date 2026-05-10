using System.Collections.Generic;
using _Data.Scripts.Controllers;
using _Data.Scripts.Enum;
using _Data.Scripts.Manager;
using Base.Systems.Pool;
using Base.Systems.Sound;
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

        // Cache
        private Vector2Int _candyPos;
        private Vector2 _spawnPos;

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
            List<GameObject> candiesToShuffle = new List<GameObject>();
            List<Vector2Int> positionsToShuffle = new List<Vector2Int>();

            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    if (_bitBoardData[x][y] > 0)
                    {
                        _candyPos.x = x;
                        _candyPos.y = y;
                        candiesToShuffle.Add(_boardData[x][y]);
                        positionsToShuffle.Add(_candyPos);
                    }
                }
            }

            candiesToShuffle.Shuffle();

            float offset = GetOffset(boardSize);
            for (int i = 0; i < positionsToShuffle.Count; i++)
            {
                Vector2Int pos = positionsToShuffle[i];
                GameObject candy = candiesToShuffle[i];

                _boardData[pos.x][pos.y] = candy;

                var comp = candy.GetComponent<CandyController>();
                comp.SetPosition(pos);
                _bitBoardData[pos.x][pos.y] = comp.EnumType;

                candy.transform.position = GetSpawnPos(pos.x, pos.y, offset);
            }
        }

        #endregion

        #region Core

        public void SwapData(GameObject first, GameObject second)
        {
            SoundManager.Ins.PlaySfx(nameof(SoundEnum.Swap));
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

            if (_bitBoardData[firstPos.x][firstPos.y] <= 0 || _bitBoardData[secondPos.x][secondPos.y] <= 0)
                return null;

            _second = _boardData[secondPos.x][secondPos.y];

            var pos1 = first.transform.position;
            var pos2 = _second.transform.position;

            SwapData(first, _second);

            bool eitherIsSpecial = IsSpecial(firstComp.EnumType) ||
                                   IsSpecial(_second.GetComponent<CandyController>().EnumType);
            var find1 = matchService.FindMatch(first, _bitBoardData, out _);
            var find2 = matchService.FindMatch(_second, _bitBoardData, out _);

            Sequence s = DOTween.Sequence();

            if (eitherIsSpecial || find1.Count > 0 || find2.Count > 0)
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

        public Sequence RemoveMatch(List<Vector2Int> pos, bool canDestroyBrick = false)
        {
            SoundManager.Ins.PlaySfx(nameof(SoundEnum.Remove));
            Sequence s = DOTween.Sequence();
            var candyList = new List<GameObject>();

            for (int i = 0; i < pos.Count; i++)
            {
                var x = pos[i].x;
                var y = pos[i].y;

                if (_boardData[x][y] == null) continue;

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

            ScoreManager.Ins.AddScore(candyList.Count);
            return s;
        }

        public Sequence Fall(int boardSize)
        {
            Sequence s = DOTween.Sequence();
            float offset = GetOffset(boardSize);

            bool changed = true;

            // Xử lý logic rơi trên data trước
            while (changed)
            {
                changed = false;

                // Rơi thẳng
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        if (_bitBoardData[x][y] != 0) continue;

                        for (int k = y + 1; k < boardSize; k++)
                        {
                            if (_bitBoardData[x][k] == -1) break;
                            if (_bitBoardData[x][k] != 0)
                            {
                                MoveCandyDataOnly(x, k, x, y);
                                changed = true;
                                break;
                            }
                        }
                    }
                }

                // Rơi chéo
                for (int y = 0; y < boardSize; y++)
                {
                    for (int x = 0; x < boardSize; x++)
                    {
                        if (_bitBoardData[x][y] != 0) continue;

                        int diagY = y + 1;
                        if (diagY >= boardSize) continue;

                        int leftX = x - 1;
                        int rightX = x + 1;

                        if (leftX >= 0 && _bitBoardData[leftX][diagY] > 0)
                        {
                            MoveCandyDataOnly(leftX, diagY, x, y);
                            changed = true;
                        }
                        else if (rightX < boardSize && _bitBoardData[rightX][diagY] > 0)
                        {
                            MoveCandyDataOnly(rightX, diagY, x, y);
                            changed = true;
                        }
                    }
                }
            }

            // Tạo animation sau khi đã có vị trí cuối cùng
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    var candy = _boardData[x][y];
                    if (candy != null)
                    {
                        var comp = candy.GetComponent<CandyController>();
                        var targetPos = GetSpawnPos(x, y, offset);

                        comp.SetPosition(new Vector2Int(x, y));

                        if (Vector2.Distance(candy.transform.position, targetPos) > 0.01f)
                        {
                            s.Join(candy.transform.DOMove(targetPos, 0.25f).SetEase(Ease.OutQuad));
                        }
                    }
                }
            }

            return s;
        }

        public Sequence Fill(int boardSize, Transform holder)
        {
            Sequence s = DOTween.Sequence();
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    if (_bitBoardData[x][y] == 0)
                    {
                        s.Join(Spawn(x, y, boardSize, holder));
                    }
                }
            }

            return s;
        }

        #endregion

        #region Special Candy

        public List<Vector2Int> GetActivationArea(GameObject candy, int boardSize, int targetColor = -1)
        {
            var comp = candy.GetComponent<CandyController>();
            var pos = comp.Position;
            var type = comp.EnumType;
            var result = new List<Vector2Int>();

            if (IsLineHorizontal(type))
            {
                for (int x = 0; x < boardSize; x++)
                {
                    result.Add(new Vector2Int(x, pos.y));
                }
            }
            else if (IsLineVertical(type))
            {
                for (int y = 0; y < boardSize; y++)
                {
                    result.Add(new Vector2Int(pos.x, y));
                }
            }
            else if (IsBomb(type))
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = pos.x + dx, ny = pos.y + dy;
                        if (nx >= 0 && nx < boardSize && ny >= 0 && ny < boardSize)
                        {
                            result.Add(new Vector2Int(nx, ny));
                        }
                    }
                }
            }
            else if (IsColorBomb(type))
            {
                int color = (targetColor > 0) ? targetColor : GetMostCommonColor(boardSize);
                result.AddRange(GetColorBombArea(color, boardSize));
            }

            return result;
        }

        public List<Vector2Int> GetColorBombArea(int targetColor, int boardSize)
        {
            var result = new List<Vector2Int>();
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    int t = _bitBoardData[x][y];
                    if (t > 0 && GetBaseColor(t) == targetColor)
                    {
                        _candyPos.x = x;
                        _candyPos.y = y;
                        result.Add(_candyPos);
                    }
                }
            }

            return result;
        }

        public int GetMostCommonColor(int boardSize)
        {
            var counts = new int[7]; // index 1-6
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    int t = _bitBoardData[x][y];
                    if (t >= 1 && t <= 6) counts[t]++;
                }
            }

            int best = 1;
            for (int c = 2; c <= 6; c++)
            {
                if (counts[c] > counts[best])
                {
                    best = c;
                }
            }
            return best;
        }

        public void SpawnSpecialCandy(SpawnInfo spawnInfo, Transform holder, int boardSize)
        {
            if (spawnInfo.Special == SpecialEnum.None) return;

            var pos = spawnInfo.SpawnPos;

            if (_bitBoardData[pos.x][pos.y] != 0) return;

            int spawnType;
            string poolKey;

            if (spawnInfo.Special == SpecialEnum.ColorBomb)
            {
                spawnType = (int)CandyEnum.ColorBomb;
                poolKey = nameof(CandyEnum.ColorBomb);
            }
            else
            {
                int offset = (int)spawnInfo.Special * 6;
                spawnType = spawnInfo.ColorType + offset;
                poolKey = ((CandyEnum)spawnType).ToString();
            }

            float o = GetOffset(boardSize);
            var candy = PoolManager.Ins.Spawn(poolKey, GetSpawnPos(pos.x, pos.y, o), Quaternion.identity);
            candy.transform.SetParent(holder);
            candy.transform.localScale = Vector3.zero;
            candy.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

            var comp = candy.GetComponent<CandyController>();
            comp.SetPosition(pos);
            comp.SetEnumType(spawnType);

            _boardData[pos.x][pos.y] = candy;
            _bitBoardData[pos.x][pos.y] = spawnType;
        }

        #endregion

        #region Helper

        public float GetOffset(int boardSize)
        {
            float offset = -(boardSize - 1) / 4f;
            return offset;
        }

        public Vector2 GetSpawnPos(int x, int y, float offset)
        {
            _spawnPos.x = x / 2f + offset;
            _spawnPos.y = y / 2f + offset;
            return _spawnPos;
        }

        Tween Spawn(int x, int y, int boardSize, Transform holder)
        {
            var type = Random.Range(1, 7);
            var poolKey = ((CandyEnum)type).ToString();

            var newCandy = PoolManager.Ins.Spawn(poolKey, GetSpawnPos(x, y, GetOffset(boardSize)), Quaternion.identity);
            Tween spawn = newCandy.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutQuad);
            var comp = newCandy.GetComponent<CandyController>();
            comp.SetPosition(new Vector2Int(x, y));
            comp.SetEnumType(type);

            _boardData[x][y] = newCandy;
            _bitBoardData[x][y] = type;

            newCandy.transform.parent = holder;

            return spawn;
        }

        void MoveCandyDataOnly(int fromX, int fromY, int toX, int toY)
        {
            _boardData[toX][toY] = _boardData[fromX][fromY];
            _bitBoardData[toX][toY] = _bitBoardData[fromX][fromY];

            _boardData[fromX][fromY] = null;
            _bitBoardData[fromX][fromY] = 0;
        }

        #endregion

        #region Type Helpers

        public bool IsLineHorizontal(int type) => type >= 7 && type <= 12;
        public bool IsLineVertical(int type) => type >= 13 && type <= 18;
        public bool IsBomb(int type) => type >= 19 && type <= 24;
        public bool IsColorBomb(int type) => type == 25;
        public bool IsSpecial(int type) => type >= 7;

        public int GetBaseColor(int type)
        {
            if (type <= 0) return 0;
            return ((type - 1) % 6) + 1;
        }

        #endregion
    }
}