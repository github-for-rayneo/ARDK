#if ENABLE_INPUT_SYSTEM

using FfalconXR.InputModule;
using UnityEngine.InputSystem;

namespace FfalconXR.InputModule
{
    public class NewInputSystemKeyHandler : BaseInputKeyHandler
    {
        public override void InputUpdate(out bool pointerDown, out bool pointerUp)
        {
#if UNITY_EDITOR
            pointerDown = Mouse.current.leftButton.wasPressedThisFrame;
            pointerUp = Mouse.current.leftButton.wasReleasedThisFrame;
#else
            pointerDown = UnityEngine.InputSystem.Touchscreen.current.press.wasPressedThisFrame;
            pointerUp   = UnityEngine.InputSystem.Touchscreen.current.press.wasReleasedThisFrame;
#endif
        }
    }
}
#endif

