using _Data.Scripts.Controllers;
using _Data.Scripts.Services.Board;
using Base.Core.StateMachine;
using UnityEngine;

namespace _Data.Scripts.States.Board
{
    public abstract class BaseBoardState : IState
    {
        protected readonly BoardController Controller;
        protected readonly InputController InputController;
        protected readonly int BoardSize;
        protected readonly Transform Holder;
        protected readonly StateMachine<BoardState> StateMachine;
        protected readonly BoardService BoardService;
        protected readonly MatchService MatchService;

        protected GameObject FirstCandy
        {
            get => Controller.FirstCandy;
            set => Controller.FirstCandy = value;
        }

        protected GameObject SecondCandy
        {
            get => Controller.SecondCandy;
            set => Controller.SecondCandy = value;
        }

        protected BaseBoardState(BoardController controller, StateMachine<BoardState> stateMachine)
        {
            Controller = controller;
            InputController = controller.InputController;
            BoardSize = controller.BoardSize;
            Holder = controller.Holder;
            StateMachine = stateMachine;
            BoardService = controller.BoardService;
            MatchService = controller.MatchService;
        }

        public abstract void OnEnter();
        public abstract void OnUpdate();
        public abstract void OnFixedUpdate();
        public abstract void OnExit();
    }
}