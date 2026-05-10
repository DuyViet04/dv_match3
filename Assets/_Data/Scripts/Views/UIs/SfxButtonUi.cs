using Base.Core.Architecture;
using Base.Systems.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace _Data.Scripts.Views.UIs
{
    public class SfxButtonUi : BaseView
    {
        [SerializeField] private Slider slider;

        private void OnEnable()
        {
            slider.onValueChanged.AddListener(SoundManager.Ins.UpdateSfxVolume);
        }

        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(SoundManager.Ins.UpdateSfxVolume);
        }
    }
}