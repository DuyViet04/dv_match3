using System.Collections.Generic;
using _Data.Scripts.Services.Board;
using Base.Core.Architecture;
using UnityEngine;

namespace _Data.Scripts.Controllers
{
    public class BoardController : BaseController
    {
        [Header("Board Settings")] [SerializeField]
        private int boardSize;

        [SerializeField] private Transform holder;

        private readonly BoardGenerateService _boardGenerateService = new BoardGenerateService();

        private void Start()
        {
            _boardGenerateService.GenerateBoard(boardSize, holder);
        }

        public void ShuffleBoard()
        {
            _boardGenerateService.BoardShuffle(boardSize);
        }
    }
}