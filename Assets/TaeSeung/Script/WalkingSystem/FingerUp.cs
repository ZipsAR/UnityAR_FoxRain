using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace QCHT.Interactions.Core.UP
{
    public struct FingerUpState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('Q', 'C', 'H', 'T');

        [Preserve, InputControl(name = "MiddlefingerUp", layout = "Button", aliases = new[] { "MiddleUp", "MiddlefingerUp" })]
        public bool Middlefingerup;
    }


#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(displayName = "Qualcomm Hand", commonUsages = new[] { "LeftHand", "RightHand" },
        stateType = typeof(FingerUpState))]
    public class HandFingerUp : XRController
    {
        public ButtonControl middlefingerup { get; private set; }
        public const string kDeviceName = "Qualcomm Hand";

        static HandFingerUp()
        {
            InputSystem.RegisterLayout<HandFingerUp>(matches: new InputDeviceMatcher().WithProduct(kDeviceName));
        }

        protected override void FinishSetup()
        {
            base.FinishSetup();
            middlefingerup = GetChildControl<ButtonControl>("MiddlefingerUp");
            
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInPlayer()
        {
        }
    }
}