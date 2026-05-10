using System.Collections.Generic;
using Base.Core.Architecture;
using UnityEngine;

namespace _Data.Scripts.Views
{
    public class BackgroundView : BaseView
    {
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private List<Sprite> backgroundSprites;

        protected override void Awake()
        {
            base.Awake();
            var roll = Random.Range(0, backgroundSprites.Count);
            background.sprite = backgroundSprites[roll];
        }
    }
}