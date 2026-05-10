using System;
using Base.Core.Singleton;
using UnityEngine;

namespace _Data.Scripts.Manager
{
    public class ScoreManager : VyesSingleton<ScoreManager>
    {
        private int _score;
        private int _scoreToIncreaseHard = 10000;

        public event Action<int> OnScoreChanged;

        public int Score => _score;

        public int ScoreToIncreaseHard => _scoreToIncreaseHard;

        public void AddScore(int matchFind)
        {
            _score += 100 * (int)Mathf.Pow((matchFind - 2), 2);
            OnScoreChanged?.Invoke(_score);
        }
    }
}