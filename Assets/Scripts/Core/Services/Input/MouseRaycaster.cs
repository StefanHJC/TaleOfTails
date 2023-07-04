using System;
using Core.Logic;
using UnityEngine;
using Zenject;
using Cursor = Core.Logic.Cursor;

namespace Core.Services.Input
{
    public class MouseRaycaster : MonoBehaviour, IRaycastService
    {
        [SerializeField] private Texture2D _arrow;

        private const string PointerColliderLayerName = "Pointer Colliders";

        private Cell _focusedCell;
        private Cursor _cursor;
        private int _layer;
        private bool _isEnabled;
        public Cell FocusedCell => _focusedCell;

        public event Action<Cell> NewCellFocused;
        public event Action<Cell> CellDefocused;


        [Inject]
        public void Construct(Cursor cursor)
        {
            _cursor = cursor;
        }

        public void Enable()
        {
            _isEnabled = true;
            _focusedCell = null;
        }

        public void Disable()
        {
            _isEnabled = false;
            _focusedCell = null;
        }

        public bool CastRayFromCamera(Vector3 to, LayerMask layer, out RaycastHit hitInfo)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(to);
            hitInfo = new RaycastHit();

            if (Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity, layer))
            {
                hitInfo = hit;
                return true;
            }

            return false;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Disable();
        }

        private void Start() => _layer = 1 << LayerMask.NameToLayer(PointerColliderLayerName);

        private void OnGUI()
        {
            if (_cursor.IsReady)
                RenderCursor();
        }

        private void FixedUpdate()
        {
            if (_isEnabled == false)
                return;

            if (TryGetFocusedCell(out Cell cell))
            {
                if (_focusedCell != cell)
                {
                    if (_focusedCell != null)
                    {
                        CellDefocused?.Invoke(_focusedCell);
                        _focusedCell = cell;

                        NewCellFocused?.Invoke(_focusedCell);
                        return;
                    }

                    _focusedCell = cell;
                    NewCellFocused?.Invoke(_focusedCell);
                }
            }
            else
            {
                if (_focusedCell != null)
                {
                    CellDefocused?.Invoke(_focusedCell);
                    _focusedCell = null;
                }
            }
        }

        private void OnLevelWasLoaded()
        {
            _focusedCell = null;
            _isEnabled = false;
        }

        private bool TryGetFocusedCell(out Cell focused)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100f, Color.red);

            if (Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity, _layer))
            {
                focused = hit.collider.GetComponentInParent<Cell>();

                return true;
            }

            focused = null;

            return false;
        }

        private void RenderCursor()
        {
            float screenSizeX = Screen.width / 1920f;
            float screenSizeY = Screen.height / 1080f;
            float mouseX = Event.current.mousePosition.x;
            float mouseY = Event.current.mousePosition.y;
            GUI.DrawTexture(new Rect(mouseX, mouseY, 64 * screenSizeX, 64 * screenSizeY), _cursor.Sprite);

            if (_cursor.State == CursorState.MeleeAttack)
            {
                GUIUtility.RotateAroundPivot(_cursor.Rotation, new Vector2(mouseX, mouseY));
                GUI.DrawTexture(new Rect(mouseX * 1.05f, mouseY, 64 * screenSizeX, 64 * screenSizeY), _arrow);
            }
        }
    }
}