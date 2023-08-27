//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/TaeSeung/Test/ZipsarHandAction.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @ZipsarHandAction : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @ZipsarHandAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""ZipsarHandAction"",
    ""maps"": [
        {
            ""name"": ""PlayerGesture"",
            ""id"": ""ed407994-8af7-45eb-85db-b18af2aeb848"",
            ""actions"": [
                {
                    ""name"": ""HandMotion"",
                    ""type"": ""Value"",
                    ""id"": ""76f6ed30-a7aa-435c-967a-590c4d4733eb"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Grab"",
                    ""type"": ""Button"",
                    ""id"": ""892fe823-31aa-4dfa-bdab-3326a85f4ebd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pinch"",
                    ""type"": ""Button"",
                    ""id"": ""f019f73b-a536-4050-aee9-08bd25ace5b9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DevicePositionTracking"",
                    ""type"": ""Value"",
                    ""id"": ""538ef577-ac8e-42b8-8fee-b0ad75c88a1c"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Gesture"",
                    ""id"": ""49afb09c-95d5-41e3-9f8a-77a271199c4f"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HandMotion"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""38a0cd05-5c6d-43d5-a3ad-821d9fb89c64"",
                    ""path"": ""<HandTrackingDevice>/triggerPressed"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HandMotion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""35395e71-7303-4ecd-be67-089723856116"",
                    ""path"": ""<HandTrackingDevice>/gripPressed"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HandMotion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""1ccef163-93a9-4201-861b-0f46808c0ed8"",
                    ""path"": ""<HandTrackingDevice>/gripPressed"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Grab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03d7e296-b47a-464d-85c4-3f6436f41c59"",
                    ""path"": ""<HandTrackingDevice>/triggerPressed"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pinch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1832f19c-7eb8-4b47-9834-c13e67d2f61c"",
                    ""path"": ""<HandTrackingDevice>/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DevicePositionTracking"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Hand"",
            ""bindingGroup"": ""Hand"",
            ""devices"": [
                {
                    ""devicePath"": ""<HandheldARInputDevice>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PlayerGesture
        m_PlayerGesture = asset.FindActionMap("PlayerGesture", throwIfNotFound: true);
        m_PlayerGesture_HandMotion = m_PlayerGesture.FindAction("HandMotion", throwIfNotFound: true);
        m_PlayerGesture_Grab = m_PlayerGesture.FindAction("Grab", throwIfNotFound: true);
        m_PlayerGesture_Pinch = m_PlayerGesture.FindAction("Pinch", throwIfNotFound: true);
        m_PlayerGesture_DevicePositionTracking = m_PlayerGesture.FindAction("DevicePositionTracking", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerGesture
    private readonly InputActionMap m_PlayerGesture;
    private IPlayerGestureActions m_PlayerGestureActionsCallbackInterface;
    private readonly InputAction m_PlayerGesture_HandMotion;
    private readonly InputAction m_PlayerGesture_Grab;
    private readonly InputAction m_PlayerGesture_Pinch;
    private readonly InputAction m_PlayerGesture_DevicePositionTracking;
    public struct PlayerGestureActions
    {
        private @ZipsarHandAction m_Wrapper;
        public PlayerGestureActions(@ZipsarHandAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @HandMotion => m_Wrapper.m_PlayerGesture_HandMotion;
        public InputAction @Grab => m_Wrapper.m_PlayerGesture_Grab;
        public InputAction @Pinch => m_Wrapper.m_PlayerGesture_Pinch;
        public InputAction @DevicePositionTracking => m_Wrapper.m_PlayerGesture_DevicePositionTracking;
        public InputActionMap Get() { return m_Wrapper.m_PlayerGesture; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerGestureActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerGestureActions instance)
        {
            if (m_Wrapper.m_PlayerGestureActionsCallbackInterface != null)
            {
                @HandMotion.started -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnHandMotion;
                @HandMotion.performed -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnHandMotion;
                @HandMotion.canceled -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnHandMotion;
                @Grab.started -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnGrab;
                @Grab.performed -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnGrab;
                @Grab.canceled -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnGrab;
                @Pinch.started -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnPinch;
                @Pinch.performed -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnPinch;
                @Pinch.canceled -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnPinch;
                @DevicePositionTracking.started -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnDevicePositionTracking;
                @DevicePositionTracking.performed -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnDevicePositionTracking;
                @DevicePositionTracking.canceled -= m_Wrapper.m_PlayerGestureActionsCallbackInterface.OnDevicePositionTracking;
            }
            m_Wrapper.m_PlayerGestureActionsCallbackInterface = instance;
            if (instance != null)
            {
                @HandMotion.started += instance.OnHandMotion;
                @HandMotion.performed += instance.OnHandMotion;
                @HandMotion.canceled += instance.OnHandMotion;
                @Grab.started += instance.OnGrab;
                @Grab.performed += instance.OnGrab;
                @Grab.canceled += instance.OnGrab;
                @Pinch.started += instance.OnPinch;
                @Pinch.performed += instance.OnPinch;
                @Pinch.canceled += instance.OnPinch;
                @DevicePositionTracking.started += instance.OnDevicePositionTracking;
                @DevicePositionTracking.performed += instance.OnDevicePositionTracking;
                @DevicePositionTracking.canceled += instance.OnDevicePositionTracking;
            }
        }
    }
    public PlayerGestureActions @PlayerGesture => new PlayerGestureActions(this);
    private int m_HandSchemeIndex = -1;
    public InputControlScheme HandScheme
    {
        get
        {
            if (m_HandSchemeIndex == -1) m_HandSchemeIndex = asset.FindControlSchemeIndex("Hand");
            return asset.controlSchemes[m_HandSchemeIndex];
        }
    }
    public interface IPlayerGestureActions
    {
        void OnHandMotion(InputAction.CallbackContext context);
        void OnGrab(InputAction.CallbackContext context);
        void OnPinch(InputAction.CallbackContext context);
        void OnDevicePositionTracking(InputAction.CallbackContext context);
    }
}
