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

        private readonly BoardGenerateService _boardGenerateService = new BoardGenerateService();

        private void Start()
        {
            _boardGenerateService.GenerateBoard(boardSize, holder);
        }

        private void Update()
        {
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
                _boardGenerateService.Swap(_firstCandy, dragPos);
            }
        }

        // Test Shuffle
        public void ShuffleBoard()
        {
            _boardGenerateService.BoardShuffle(boardSize);
        }
    }
}