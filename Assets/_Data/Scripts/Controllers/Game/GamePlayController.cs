using Base.Core.Architecture;
using VyesBase.Assets.Base.Systems.Game;

namespace _Data.Scripts.Controllers.Game
{
    public class GamePlayController : BaseController
    {
        public void Quit()
        {
            GameManager.Ins.LoadScene(GameState.MainMenu);
        }
    }
}