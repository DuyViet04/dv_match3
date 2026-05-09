using _Data.Scripts.Controllers;
using Base.Core.StateMachine;
using UnityEngine;

namespace _Data.Scripts.States.Board
{
    public class IdleState : BaseBoardState
    {
        public IdleState(BoardController controller, StateMachine<BoardState> stateMachine) : base(controller,
            stateMachine)
        {
        }

        public override void OnEnter()
        {
            Time.timeScale = 1;
            while (!MatchService.CanMove(BoardSize, BoardService))
            {
                BoardService.BoardShuffle(BoardSize);
            }

            MatchService.FindCombo(BoardSize, Holder, BoardService);
        }

        public override void OnUpdate()
        {
            if (InputController.IsClick())
            {
                var startPos = InputController.StartPosition;
                RaycastHit2D hit2D = Physics2D.Raycast(startPos, Vector2.zero);
                if (hit2D.collider != null)
                {
                    FirstCandy = hit2D.collider.gameObject;
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