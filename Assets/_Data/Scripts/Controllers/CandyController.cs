using Base.Core.Architecture;
using Base.Systems.Pool;
using UnityEngine;

namespace _Data.Scripts.Controllers
{
    public class CandyController : BaseController, IPoolable
    {
        [SerializeField] private Vector2Int position;
        [SerializeField] private int enumType;

        public Vector2Int Position => position;
        public int EnumType => enumType;

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
        }

        public void SetPosition(Vector2Int position)
        {
            this.position = position;
        }

        public void SetEnumType(int enumType)
        {
            this.enumType = enumType;
        }
    }
}