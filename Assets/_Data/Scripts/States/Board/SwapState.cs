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
            if (sequence == null)
            {
                StateMachine.ChangeState(BoardState.Idle);
                return;
            }

            sequence.OnComplete(() =>
            {
                if (isMatch)
                {
                    SecondCandy = BoardService.Second;
                    ExecuteMatch(FirstCandy, SecondCandy);
                }
                else
                {
                    StateMachine.ChangeState(BoardState.Idle);
                }
            });
        }

        void ExecuteMatch(GameObject first, GameObject second)
        {
            var firstComp  = first.GetComponent<CandyController>();
            var secondComp = second.GetComponent<CandyController>();
            int firstType  = firstComp.EnumType;
            int secondType = secondComp.EnumType;

            var allPos = new HashSet<Vector2Int>();

            var find1 = MatchService.FindMatch(first,  BoardService.BitBoardData, out var spawnInfo1);
            var find2 = MatchService.FindMatch(second, BoardService.BitBoardData, out var spawnInfo2);
            foreach (var p in find1) allPos.Add(p);
            foreach (var p in find2) allPos.Add(p);
            if (BoardService.IsColorBomb(firstType))
            {
                int targetColor = BoardService.IsSpecial(secondType)
                    ? BoardService.GetMostCommonColor(BoardSize)
                    : BoardService.GetBaseColor(secondType);
                var area = BoardService.GetActivationArea(first, BoardSize, targetColor);
                area.Add(firstComp.Position);
                foreach (var p in area) allPos.Add(p);
            }
            else if (BoardService.IsSpecial(firstType))
            {
                var area = BoardService.GetActivationArea(first, BoardSize);
                area.Add(firstComp.Position);
                foreach (var p in area) allPos.Add(p);
            }

            if (BoardService.IsColorBomb(secondType))
            {
                int targetColor = BoardService.IsSpecial(firstType)
                    ? BoardService.GetMostCommonColor(BoardSize)
                    : BoardService.GetBaseColor(firstType);
                var area = BoardService.GetActivationArea(second, BoardSize, targetColor);
                area.Add(secondComp.Position);
                foreach (var p in area) allPos.Add(p);
            }
            else if (BoardService.IsSpecial(secondType))
            {
                var area = BoardService.GetActivationArea(second, BoardSize);
                area.Add(secondComp.Position);
                foreach (var p in area) allPos.Add(p);
            }

            var bestSpawn = MatchService.PickBestSpawnInfo(spawnInfo1, spawnInfo2);

            BoardService.RemoveMatch(new List<Vector2Int>(allPos), canDestroyBrick: true)
                .OnComplete(() =>
                {
                    BoardService.SpawnSpecialCandy(bestSpawn, Holder, BoardSize);
                    StateMachine.ChangeState(BoardState.Fall);
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
    }
}