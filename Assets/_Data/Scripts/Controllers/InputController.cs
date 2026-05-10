using Base.Core.Architecture;
using Base.Systems.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Data.Scripts.Controllers
{
    public class InputController : BaseController
    {
        [SerializeField] private Camera mainCamera;
        private Vector2 _startPos;
        private Vector2 _endPos;
        private Vector2Int _dragDirection;

        public Vector2 StartPosition => _startPos;
        public Vector2 EndPosition => _endPos;
        public Vector2Int DragDirection => _dragDirection;

        public bool IsClick()
        {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                _startPos = mainCamera.ScreenToWorldPoint(InputManager.Ins.Mouse);
                return true;
            }

            return false;
        }

        public bool IsDrag()
        {
            if (Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
            {
                _endPos = mainCamera.ScreenToWorldPoint(InputManager.Ins.Mouse);

                if (Vector2.Distance(_startPos, _endPos) > 0.3f)
                {
                    _dragDirection = DragDirectionCalculate(_startPos, _endPos);
                    return true;
                }
            }

            return false;
        }

        Vector2Int DragDirectionCalculate(Vector2 startPos, Vector2 endPos)
        {
            var dir = (endPos - startPos).normalized;
            var rad = Mathf.Atan2(dir.y, dir.x);
            var angle = Mathf.Rad2Deg * rad;
            if (angle < 0) angle += 360;

            if ((0 <= angle && angle < 45) || (315 <= angle && angle <= 360))
            {
                return Vector2Int.right;
            }
            else if (45 <= angle && angle < 135)
            {
                return Vector2Int.up;
            }
            else if (135 <= angle && angle < 225)
            {
                return Vector2Int.left;
            }
            else if (225 <= angle && angle < 315)
            {
                return Vector2Int.down;
            }

            return Vector2Int.zero;
        }
    }
}