using System.Collections.Generic;
using _Data.Scripts.Controllers;
using Base.Core.StateMachine;
using DG.Tweening;
using UnityEngine;

namespace _Data.Scripts.States.Board
{
    public class SwapState : BaseBoardState
    {
        public SwapState(BoardController controller, StateMachine<BoardState> stateMachine) : base(controller,
            stateMachine)
        {
        }

        public override void OnEnter()
        {
            if (FirstCandy == null)
            {
                StateMachine.ChangeState(BoardState.Idle);
                return;
            }

            var dragPos = InputController.DragDirection;
            var sequence = BoardService.TrySwap(FirstCandy, dragPos, MatchService, out bool isMatch);

            sequence.OnComplete(() =>
            {
                if (isMatch)
                {
                    SecondCandy = BoardService.Second;
                    var totalRemove = FindAll(FirstCandy, SecondCandy, BoardService.BitBoardData);
                    BoardService.RemoveMatch(totalRemove).OnComplete(() =>
                    {
                        StateMachine.ChangeState(BoardState.Fall);
                    });
                }
                else
                {
                    StateMachine.ChangeState(BoardState.Idle);
                }
            });
        }

        public override void OnUpdate()
        {
        }

        public override void OnFixedUpdate()
        {
        }

        public override void OnExit()
        {
        }

        List<Vector2Int> FindAll(GameObject firstCandy, GameObject secondCandy, int[][] bitBoardData)
        {
            var hs = new HashSet<Vector2Int>();
            var firstFind = MatchService.FindMatch(firstCandy, bitBoardData);
            var secondFind = MatchService.FindMatch(secondCandy, bitBoardData);

            for (int i = 0; i < firstFind.Count; i++)
            {
                hs.Add(firstFind[i]);
            }

            for (int i = 0; i < secondFind.Count; i++)
            {
                hs.Add(secondFind[i]);
            }

            return new List<Vector2Int>(hs);
        }
    }
}