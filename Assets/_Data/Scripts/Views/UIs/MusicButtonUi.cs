using Base.Core.Architecture;
using Base.Systems.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace _Data.Scripts.Views.UIs
{
    public class MusicButtonUi : BaseView
    {
        [SerializeField] private Slider slider;

        private void OnEnable()
        {
            slider.onValueChanged.AddListener(SoundManager.Ins.UpdateMusicVolume);
        }

        private void OnDisable()
        {
            if (SoundManager.HasInstance)
            {
                slider.onValueChanged.RemoveListener(SoundManager.Ins.UpdateMusicVolume);
            }
        }
    }
}