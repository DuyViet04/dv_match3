using _Data.Scripts.Controllers;
using Base.Core.StateMachine;

namespace _Data.Scripts.States.Board
{
    public class FallState : BaseBoardState
    {
        public FallState(BoardController controller, StateMachine<BoardState> stateMachine) : base(controller,
            stateMachine)
        {
        }

        public override void OnEnter()
        {
            BoardService.Fall(BoardSize);
            StateMachine.ChangeState(BoardState.Fill);
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