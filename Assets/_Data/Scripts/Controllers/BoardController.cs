using System.Collections.Generic;
using _Data.Scripts.Services.Board;
using Base.Core.Architecture;
using UnityEngine;

namespace _Data.Scripts.Controllers
{
    public class BoardController : BaseController
    {
        [SerializeField] private InputController inputController;

        [Header("Board Settings")] [SerializeField]
        private int boardSize;

        [SerializeField] private Transform holder;

        private GameObject _firstCandy;
        private GameObject _secondCandy;

        private readonly BoardService _boardService = new BoardService();
        private readonly MatchService _matchService = new MatchService();

        private void Start()
        {
            _boardService.GenerateBoard(boardSize, holder);
        }

        private void Update()
        {
            _matchService.FindCombo(boardSize, holder, _boardService);

            if (inputController.IsClick())
            {
                var startPos = inputController.StartPosition;
                RaycastHit2D hit2D = Physics2D.Raycast(startPos, Vector2.zero);
                if (hit2D.collider != null)
                {
                    _firstCandy = hit2D.collider.gameObject;
                }
            }

            if (inputController.IsDrag())
            {
                var dragPos = inputController.DragDirection;
                _boardService.Swap(_firstCandy, dragPos);

                _secondCandy = _boardService.Second;
                var totalRemove = Find(_firstCandy, _secondCandy, _boardService.BitBoardData);

                _boardService.RemoveMatch(totalRemove);

                _boardService.Fall(boardSize);
                _boardService.Fill(boardSize, holder);
            }
        }

        List<Vector2Int> Find(GameObject firstCandy, GameObject secondCandy, int[][] bitBoardData)
        {
            var hashSet = new HashSet<Vector2Int>();
            var firstFind = _matchService.FindMatch(firstCandy, bitBoardData);
            var secondFind = _matchService.FindMatch(secondCandy, bitBoardData);

            for (int i = 0; i < firstFind.Count; i++)
            {
                hashSet.Add(firstFind[i]);
            }

            for (int i = 0; i < secondFind.Count; i++)
            {
                hashSet.Add(secondFind[i]);
            }

            return new List<Vector2Int>(hashSet);
        }

        // Test Shuffle
        public void ShuffleBoard()
        {
            _boardService.BoardShuffle(boardSize);
        }
    }
}