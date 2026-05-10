using _Data.Scripts.Controllers;
using _Data.Scripts.Manager;
using Base.Core.StateMachine;
using DG.Tweening;
using UnityEngine;

namespace _Data.Scripts.States.Board
{
    public class IdleState : BaseBoardState
    {
        private int _brickCount;
        private bool _isProcessing;

        public IdleState(BoardController controller, StateMachine<BoardState> stateMachine) : base(controller,
            stateMachine)
        {
        }

        public override void OnEnter()
        {
            Time.timeScale = 1;
            _isProcessing = false;

            int shuffleCount = 0;
            while (!MatchService.CanMove(BoardSize, BoardService) && shuffleCount < 10)
            {
                BoardService.BoardShuffle(BoardSize);
                shuffleCount++;
            }

            if (shuffleCount == 10)
            {
                WinPanel.SetActive(true);
            }

            var score = ScoreManager.Ins.Score;
            var scoreToIncrease = ScoreManager.Ins.ScoreToIncreaseHard;

            if (score >= scoreToIncrease * (_brickCount + 1))
            {
                Controller.SpawnBrick();
                _brickCount++;
            }

            var matched = MatchService.FindAllMatches(BoardSize, BoardService);
            if (matched.Count > 0)
            {
                _isProcessing = true;
                BoardService.RemoveMatch(matched).OnComplete(() => { StateMachine.ChangeState(BoardState.Fall); });
            }
        }

        public override void OnUpdate()
        {
            if (_isProcessing) return;

            if (InputController.IsClick())
            {
                var startPos = InputController.StartPosition;
                RaycastHit2D hit2D = Physics2D.Raycast(startPos, Vector2.zero);
                if (hit2D.collider != null)
                {
                    var candy = hit2D.collider.gameObject;
                    var comp  = candy.GetComponent<CandyController>();

                    // Kích hoạt ngay nếu là kẹo đặc biệt
                    if (BoardService.IsColorBomb(comp.EnumType))
                    {
                        _isProcessing = true;
                        int targetColor = BoardService.GetMostCommonColor(BoardSize);
                        var area = BoardService.GetActivationArea(candy, BoardSize, targetColor);
                        area.Add(comp.Position); // xóa cả chính nó
                        BoardService.RemoveMatch(area, canDestroyBrick: true)
                            .OnComplete(() => StateMachine.ChangeState(BoardState.Fall));
                        return;
                    }

                    if (BoardService.IsSpecial(comp.EnumType))
                    {
                        _isProcessing = true;
                        var area = BoardService.GetActivationArea(candy, BoardSize);
                        area.Add(comp.Position);
                        BoardService.RemoveMatch(area, canDestroyBrick: true)
                            .OnComplete(() => StateMachine.ChangeState(BoardState.Fall));
                        return;
                    }

                    // Kẹo thường → chọn để drag
                    FirstCandy = candy;
                }
            }

            if (InputController.IsDrag())
            {
                StateMachine.ChangeState(BoardState.Swap);
            }
        }

        public override void OnFixedUpdate()
        {
        }

        public override void OnExit()
        {
        }
    }
}