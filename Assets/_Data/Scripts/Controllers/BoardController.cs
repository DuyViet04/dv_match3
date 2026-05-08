using System;
using System.Collections.Generic;
using _Data.Scripts.Services.Board;
using _Data.Scripts.States.Board;
using Base.Core.Architecture;
using Base.Core.StateMachine;
using UnityEngine;

namespace _Data.Scripts.Controllers
{
    public class BoardController : BaseController
    {
        [SerializeField] private InputController inputController;

        [Header("Board Settings")] [SerializeField]
        private int boardSize;

        [SerializeField] private Transform holder;

        private StateMachine<BoardState> _stateMachine;
        private GameObject _firstCandy;
        private GameObject _secondCandy;

        private readonly BoardService _boardService = new BoardService();
        private readonly MatchService _matchService = new MatchService();

        public InputController InputController => inputController;
        public int BoardSize => boardSize;
        public Transform Holder => holder;
        public BoardService BoardService => _boardService;
        public MatchService MatchService => _matchService;

        public GameObject FirstCandy
        {
            get => _firstCandy;
            set => _firstCandy = value;
        }

        public GameObject SecondCandy
        {
            get => _secondCandy;
            set => _secondCandy = value;
        }

        protected override void Awake()
        {
            base.Awake();
            InitState();
        }

        private void Update()
        {
            _stateMachine.UpdateState();
        }

        void InitState()
        {
            _stateMachine = new StateMachine<BoardState>();
            _stateMachine.AddState(BoardState.Start, new StartState(this, _stateMachine));
            _stateMachine.AddState(BoardState.Idle, new IdleState(this, _stateMachine));
            _stateMachine.AddState(BoardState.Swap, new SwapState(this, _stateMachine));
            _stateMachine.AddState(BoardState.Fall, new FallState(this, _stateMachine));
            _stateMachine.AddState(BoardState.Fill, new FillState(this, _stateMachine));
            _stateMachine.SetInitState(BoardState.Start);
        }
    }
}