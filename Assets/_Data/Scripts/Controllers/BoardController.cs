using _Data.Scripts.Enum;
using _Data.Scripts.Services.Board;
using _Data.Scripts.States.Board;
using Base.Core.Architecture;
using Base.Core.StateMachine;
using Base.Systems.Pool;
using DG.Tweening;
using UnityEngine;

namespace _Data.Scripts.Controllers
{
    public class BoardController : BaseController
    {
        [SerializeField] private InputController inputController;
        [Header("Board Settings")] 
        [SerializeField] private int boardSize;
        [SerializeField] private Transform holder;
        [Header("Other Settings")]
        [SerializeField] private GameObject winPanel;

        private StateMachine<BoardState> _stateMachine;

        private readonly BoardService _boardService = new BoardService();
        private readonly MatchService _matchService = new MatchService();

        public InputController InputController => inputController;
        public int BoardSize => boardSize;
        public Transform Holder => holder;
        public GameObject WinPanel => winPanel;
        public BoardService BoardService => _boardService;
        public MatchService MatchService => _matchService;
        public GameObject FirstCandy { get; set; }
        public GameObject SecondCandy { get; set; }

        protected override void Awake()
        {
            base.Awake();
            InitState();
        }

        private void Update()
        {
            _stateMachine.UpdateState();
        }

        public void SpawnBrick()
        {
            int randX = -1, randY = -1;
            
            int maxAttempts = 100;
            while (maxAttempts > 0)
            {
                randX = Random.Range(0, boardSize);
                randY = Random.Range(0, boardSize);
                if (_boardService.BitBoardData[randX][randY] > 0) break;
                maxAttempts--;
            }

            if (maxAttempts <= 0) return;

            Sequence s = DOTween.Sequence();
            GameObject oldCandy = _boardService.BoardData[randX][randY];

            s.Append(oldCandy.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad))
                .OnComplete(() => { PoolManager.Ins.Despawn(oldCandy); });

            float offset = _boardService.GetOffset(boardSize);
            var brick = PoolManager.Ins.Spawn(nameof(CandyEnum.Brick),
                _boardService.GetSpawnPos(randX, randY, offset), Quaternion.identity);

            brick.transform.SetParent(holder);

            var comp = brick.GetComponent<CandyController>();
            comp.SetPosition(new Vector2Int(randX, randY));
            comp.SetEnumType(-1);

            _boardService.BoardData[randX][randY] = brick;
            _boardService.BitBoardData[randX][randY] = -1;

            s.Append(brick.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack));
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