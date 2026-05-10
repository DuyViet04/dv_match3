using _Data.Scripts.Enum;
using Base.Core.Architecture;
using Base.Systems.Sound;
using UnityEngine;
using VyesBase.Assets.Base.Systems.Game;

namespace _Data.Scripts.Controllers.Game
{
    public class MainMenuController : BaseController
    {
        [SerializeField] private GameObject soundSetting;

        private bool _isSoundSettingOpen;

        protected override void Awake()
        {
            base.Awake();
            SoundManager.Ins.PlayMusic(nameof(SoundEnum.Music));
            soundSetting.SetActive(false);
            _isSoundSettingOpen = soundSetting.activeSelf;
        }

        public void OnPlayButtonClick()
        {
            GameManager.Ins.LoadSceneAsync(GameState.GamePlay);
        }

        public void OnSettingsButtonClick()
        {
            if (!_isSoundSettingOpen)
            {
                soundSetting.SetActive(true);
                _isSoundSettingOpen = true;
            }
            else
            {
                soundSetting.SetActive(false);
                _isSoundSettingOpen = false;
            }
        }

        public void OnQuitButtonClick()
        {
            Application.Quit();
            Debug.Log("Quit");
        }
    }
}