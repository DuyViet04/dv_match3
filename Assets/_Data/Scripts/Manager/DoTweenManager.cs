using Base.Core.Singleton;
using DG.Tweening;

namespace _Data.Scripts.Manager
{
    public class DoTweenManager : VyesPersistentSingleton<DoTweenManager>
    {
        protected override void Awake()
        {
            base.Awake();
            DOTween.SetTweensCapacity(500, 50);
        }
    }
}