using _Data.Scripts.Controllers;
using Base.Core.StateMachine;

namespace _Data.Scripts.States.Board
{
    public class StartState : BaseBoardState
    {
        public StartState(BoardController controller, StateMachine<BoardState> stateMachine) : base(controller,
            stateMachine)
        {
        }

        public override void OnEnter()
        {
            BoardService.GenerateBoard(BoardSize, Holder);
            StateMachine.ChangeState(BoardState.Idle);
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