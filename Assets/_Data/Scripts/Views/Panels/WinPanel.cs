using _Data.Scripts.Manager;
using Base.Core.Architecture;
using TMPro;
using UnityEngine;

namespace _Data.Scripts.Views.Panels
{
    public class WinPanel : BaseView
    {
        [SerializeField] private TMP_Text score;

        private void OnEnable()
        {
            score.text = $"{ScoreManager.Ins.Score}";
        }
    }
}