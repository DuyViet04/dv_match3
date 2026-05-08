using _Data.Scripts.Controllers;
using Base.Core.StateMachine;

namespace _Data.Scripts.States.Board
{
    public class FillState : BaseBoardState
    {
        public FillState(BoardController controller, StateMachine<BoardState> stateMachine) : base(controller,
            stateMachine)
        {
        }

        public override void OnEnter()
        {
            BoardService.Fill(BoardSize, Holder, MatchService);
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