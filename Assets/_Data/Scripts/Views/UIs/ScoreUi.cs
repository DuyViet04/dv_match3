using _Data.Scripts.Manager;
using Base.Core.Architecture;
using TMPro;
using UnityEngine;

namespace _Data.Scripts.Views.UIs
{
    public class ScoreUi : BaseView
    {
        [SerializeField] private TMP_Text scoreText;

        private void OnEnable()
        {
            ScoreManager.Ins.OnScoreChanged += UpdateScoreText;
        }

        private void OnDisable()
        {
            ScoreManager.Ins.OnScoreChanged -= UpdateScoreText;
        }

        private void UpdateScoreText(int score)
        {
            scoreText.text = $"{score}";
        }
    }
}