/* UltimateRadialMenu.cs */
/* Written by Kaz Crowe */
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent( typeof( CanvasGroup ) )]
[AddComponentMenu( "Ultimate Radial Menu/Ultimate Radial Menu" )]
public class UltimateRadialMenu : MonoBehaviour
{
	// INTERNAL CALCULATIONS //
	/// <summary>
	/// The parent canvas component that this radial menu is a child of.
	/// </summary>
	public UnityEngine.Canvas ParentCanvas
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns if this is a World Space radial menu or not.
	/// </summary>
	public bool IsWorldSpaceRadialMenu
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the angle of each button, determined by the menu button count.
	/// </summary>
	public float GetAnglePerButton
	{
		get
		{
			return overallAngle / UltimateRadialButtonList.Count;
		}
	}
	/// <summary>
	/// Returns the current input angle so that other scripts can access the current input of this radial menu.
	/// </summary>
	public float GetCurrentInputAngle
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the current distance of the input from the center of the menu.
	/// </summary>
	public float GetCurrentInputDistance
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the calculated minimum range of the radial menu.
	/// </summary>
	public float CalculatedMinRange
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the calculated maximum range of the radial menu.
	/// </summary>
	public float CalculatedMaxRange
	{
		get;
		private set;
	}
	/// <summary>
	/// The base RectTransform component.
	/// </summary>
	public RectTransform BaseTransform
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the position of the base transform.
	/// </summary>
	public Vector3 BasePosition
	{
		get
		{
			if( BaseTransform == null )
				return Vector3.zero;

			return BaseTransform.position;
		}
	}
	/// <summary>
	/// Returns the index of the button currently being interacted with.
	/// </summary>
	public int CurrentButtonIndex
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the current state of the radial menu being enabled or disabled visually.
	/// </summary>
	public bool IsEnabled
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the current state of the input on this radial menu.
	/// </summary>
	public bool InputInRange
	{
		get;
		private set;
	}
	/// <summary>
	/// The current state of being able to interact with the radial menu.
	/// </summary>
	public bool Interactable
	{
		get;
		set;
	}
	bool inputInRangeLastFrame = false;
	int buttonIndexOnInputDown = -1;
	Vector3 defaultPosition;
	RectTransform canvasRectTrans;
	Vector2 parentCanvasSize;

	// RADIAL MENU POSITIONING //
	public enum ScalingAxis
	{
		Width,
		Height
	}
	public ScalingAxis scalingAxis = ScalingAxis.Height;
	public float menuSize = 5.0f;
	public float horizontalPosition = 50.0f, verticalPosition = 50.0f;
	public float depthPosition = 0.0f;
	public float menuButtonSize = 0.25f;
	public float radialMenuButtonRadius = 1.0f;
	public float startingAngle = 0.0f;
	public float overallAngle = 360.0f;
	public float centerAngle = 0.0f;
	public enum AngleOffset
	{
		Centered,
		OffCenter,
		OnlyEven,
		OnlyOdd,
	}
	public AngleOffset angleOffset = AngleOffset.Centered;
	public bool followOrbitalRotation = true;
	public float minRange = 0.25f, maxRange = 1.5f;
	public bool infiniteMaxRange = false;
	public float buttonInputAngle = 0.0f;
	
	// RADIAL MENU SETTINGS //
	public UltimateRadialMenuStyle radialMenuStyle;
	int currentStyleIndex;
	public Sprite normalSprite;
	public Color normalColor = Color.white;
	// Menu Toggle //
	public enum InitialState
	{
		Enabled,
		Disabled
	}
	public InitialState initialState = InitialState.Enabled;
	[SerializeField] [Tooltip( "Determines if the menu should toggle itself from enabled and disabled over time or not." )]
	private bool menuToggleOverTime = false;
	[SerializeField] [Tooltip( "Should the menu toggle using the canvas group alpha?" )]
	private bool menuToggleAlpha = true;
	[SerializeField] [Tooltip( "Should the menu toggle by scaling the buttons?" )]
	private bool menuToggleScale = false;
	[SerializeField] [Tooltip( "The time in seconds to enable the menu." )]
	private float toggleInDuration = 0.25f;
	[SerializeField] [Tooltip( "The time in seconds to disable the menu." )]
	private float toggleOutDuration = 0.25f;
	CanvasGroup canvasGroup;
	bool toggleIn = false, toggleOut = false;
	float toggleLerpValue = 0.0f;
	float toggleAlphaValue, toggleScaleValue;
	float toggleInSpeed = 0.0f, toggleOutSpeed = 0.0f;
	
	// Pointer //
	/// <summary>
	/// The current visible state of the pointer image.
	/// </summary>
	public bool PointerActive
	{
		get;
		private set;
	}
	bool pointerFadeIn = false, pointerFadeOut = false;
	float pointerLerpValue = 0.0f;
	Color pointerFadeColor = Color.white;
	float fadeInSpeed = 0.0f, fadeOutSpeed = 0.0f;
	[SerializeField] [Tooltip( "Should an image be used to point to the current radial button?" )]
	private bool usePointer = false;
	[SerializeField ][Tooltip( "The image to be used for the pointer." )]
	private Image pointerImage = null;
	RectTransform pointerTransform;
	[SerializeField] [Tooltip( "The size of the pointer image." )] [Range( 0.0f, 2.0f )]
	private float pointerSize = 1.0f;
	[SerializeField] [Tooltip( "The time in seconds for the pointer to target the new button." )]
	private float pointerTargetTime = 0.25f;
	float pointerTargetSpeed = 5.0f;
	Quaternion pointerTargetRotation;
	public enum PointerSnapOption
	{
		Instant,
		Smooth,
		Free
	}
	[SerializeField] [Tooltip( "Determines how the pointer will interact with the input and the buttons." )]
	private PointerSnapOption pointerSnapOption = PointerSnapOption.Smooth;
	[SerializeField] [Tooltip( "The rotation offset to apply to the pointer image." )]
	private float pointerRotationOffset = 90;
	[SerializeField] [Tooltip( "Determines if the pointer should change color or not." )]
	private bool pointerColorChange = false;
	[SerializeField] [Tooltip( "Should the pointer change color over time or instantly?" )]
	private bool changeOverTime = false;
	[SerializeField] [Tooltip( "The time in seconds that the pointer should fade in." )]
	private float fadeInDuration = 0.25f;
	[SerializeField] [Tooltip( "The time in seconds that the pointer should fade out." )]
	private float fadeOutDuration = 0.5f;
	[SerializeField] [Tooltip( "The default color of the pointer image." )]
	private Color pointerNormalColor = Color.white;
	[SerializeField] [Tooltip( "The active color of the pointer image." )]
	private Color pointerActiveColor = Color.white;
	public enum SetSiblingIndex
	{
		Disabled,
		First,
		Last
	}
	[SerializeField] [Tooltip( "Determines if the sibling index should be set for the pointer object or not, and where it should be placed." )]
	private SetSiblingIndex pointerSiblingIndex = SetSiblingIndex.Disabled;
	[SerializeField] [Tooltip( "Should this pointer use a pointer style for changing the sprite according to the button count?" )]
	private bool usePointerStyle = false;
	[Serializable]
	public class PointerStyle
	{
		public int buttonCount;
		public Sprite pointerSprite;
	}
	public List<PointerStyle> PointerStyles = new List<PointerStyle>();

	// Menu Text //
	public bool displayButtonName = false;
	public Text nameText;
	public float nameTextRatioX = 1.0f, nameTextRatioY = 1.0f, nameTextSize = 0.25f;
	public float nameTextHorizontalPosition = 50.0f, nameTextVerticalPosition = 50.0f;
	public bool displayButtonDescription = false;
	public Text descriptionText;
	public float descriptionTextRatioX = 1.0f, descriptionTextRatioY = 1.0f, descriptionTextSize = 0.25f;
	public float descriptionTextHorizontalPosition = 50.0f, descriptionTextVerticalPosition = 50.0f;
	// Button Icon //
	public bool useButtonIcon = false;
	public float iconSize = 0.25f, iconRotation = 0.0f;
	public float iconHorizontalPosition = 50.0f, iconVerticalPosition = 50.0f;
	public bool iconLocalRotation = false;
	public bool iconSmartRotation = false;
	public Color iconNormalColor = Color.white;
	// Button Text //
	public bool useButtonText = false;
	public float textAreaRatioX = 1.0f, textAreaRatioY = 0.25f, textSize = 0.25f;
	public float textHorizontalPosition = 50.0f, textVerticalPosition = 50.0f;
	public bool displayNameOnButton = true;
	public bool textLocalPosition = true;
	public bool textLocalRotation = true;
	public Color textNormalColor = Color.white;
	public bool relativeToIcon = false;
	// Text Settings //
	public Font nameFont;
	public Font descriptionFont;
	public Font buttonTextFont;
	public bool nameOutline = false;
	public bool descriptionOutline = false;
	public bool buttonTextOutline = false;
	public Color buttonTextOutlineColor = Color.white;

	// BUTTON INTERACTION //
	public bool spriteSwap = false;
	public bool colorChange = true;
	public bool scaleTransform = false;
	public bool iconColorChange = false;
	public bool iconScaleTransform = false;
	public bool textColorChange = false;
	// Highlighted //
	public Sprite highlightedSprite;
	public Color highlightedColor = Color.white;
	public float highlightedScaleModifier = 1.1f;
	public float positionModifier = 0.0f;
	public Color iconHighlightedColor = Color.white;
	public float iconHighlightedScaleModifier = 1.1f;
	public Color textHighlightedColor = Color.white;
	// Pressed //
	public Sprite pressedSprite;
	public Color pressedColor = Color.white;
	public float pressedScaleModifier = 1.05f;
	public float pressedPositionModifier = 0.0f;
	public Color iconPressedColor = Color.white;
	public float iconPressedScaleModifier = 1.0f;
	public Color textPressedColor = Color.white;
	// Selected //
	public bool selectButtonOnInteract = false;
	public bool toggleSelection = false;
	public bool allowMultipleSelected = false;
	public Sprite selectedSprite;
	public Color selectedColor = Color.white;
	public float selectedScaleModifier = 1.0f;
	public float selectedPositionModifier = 0.0f;
	public Color iconSelectedColor = Color.white;
	public float iconSelectedScaleModifier = 1.0f;
	public Color textSelectedColor = Color.white;
	// Disabled //
	public Sprite disabledSprite;
	public Color disabledColor = Color.white;
	public float disabledScaleModifier = 1.0f;
	public float disabledPositionModifier = 0.0f;
	public Color iconDisabledColor = Color.white;
	public float iconDisabledScaleModifier = 1.0f;
	public Color textDisabledColor = Color.white;
	
	// RADIAL BUTTON LIST //
	[Serializable]
	public class UltimateRadialButton
	{
		/// <summary>
		/// Returns the state of having information registered to this button.
		/// </summary>
		public bool Registered
		{
			get
			{
				if( registered )
					return true;

				if( unityEvent != null && unityEvent.GetPersistentEventCount() > 0 )
					return true;

				return false;
			}
		}
		bool registered = false;
		/// <summary>
		/// Returns the current state of being selected.
		/// </summary>
		public bool Selected
		{
			get;
			private set;
		}

		// BASIC VARIABLES //
		public UltimateRadialMenu radialMenu;
		public RectTransform buttonTransform;
		public Image radialImage;
		public bool buttonDisabled = false;
		public string name;
		public string description;
		public int buttonIndex = -1;

		// INPUT VARIABLES //
		public float angle;
		public float angleRange;

		// ICON SETTINGS //
		public RectTransform iconTransform;
		public Image icon;
		public bool useIconUnique = false;
		public float iconSize = 0.0f;
		public float iconHorizontalPosition = 0.0f, iconVerticalPosition = 0.0f;
		public float iconRotation = 0.0f;
		public bool invertScaleY = false;
		public Vector3 iconNormalScale;

		// TEXT SETTINGS //
		public Text text;

		// TRANSFORM INTERACTION SETTINGS //
		public Vector3 normalPosition, highlightedPosition, pressedPosition, selectedPosition, disabledPosition;

		// BASIC CALLBACK INFORMATION //
		public string key;
		public int id;

		// CALLBACKS //
		public event Action OnRadialButtonInteract;
		public event Action<int> OnRadialButtonInteractWithId;
		public event Action<string> OnRadialButtonInteractWithKey;
		public event Action OnClearButtonInformation;
		public UnityEvent unityEvent;
		event Action<bool> OnSelectedStateChanged;

		
		/// <summary>
		/// Returns true if the angle is within the range of this button.
		/// </summary>
		/// <param name="inputAngle">The current input angle.</param>
		public bool IsInAngle ( float inputAngle )
		{
			// If the angle is within this buttons range, then return true.
			if( Mathf.Abs( inputAngle - angle ) <= angleRange || Mathf.Abs( ( inputAngle - 360f ) - angle ) <= angleRange || Mathf.Abs( inputAngle - ( angle - 360f ) ) <= angleRange )
				return true;

			// Else return false.
			return false;
		}

		/// <summary>
		/// [INTERNAL] Invokes the functionality for when the input enters the button.
		/// </summary>
		public void OnEnter ()
		{
			// If the button is disabled, then return.
			if( buttonDisabled )
				return;

			// If this button is NOT selected...
			if( !Selected )
			{
				// If the user wants to swap sprites when the radial button is highlighted...
				if( radialMenu.spriteSwap )
				{
					// If the highlighted sprite is assigned, then apply that sprite to the image.
					if( radialMenu.highlightedSprite != null )
						radialImage.sprite = radialMenu.highlightedSprite;
					// Else the highlighted sprite is null, so apply the normal sprite to the image.
					else
						radialImage.sprite = radialMenu.normalSprite;
				}

				// If the user wants to change the color of the radial button, then apply the highlighted color.
				if( radialMenu.colorChange && radialImage.sprite != null )
					radialImage.color = radialMenu.highlightedColor;

				// If the user wants to scale the transform when the radial button is being hovered over...
				if( radialMenu.scaleTransform )
				{
					// Modify the scale and position of the radial button.
					buttonTransform.localScale = Vector3.one * radialMenu.highlightedScaleModifier;
					buttonTransform.localPosition = highlightedPosition;
				}

				// If the user wants to use the icon and it is assigned...
				if( radialMenu.useButtonIcon && icon != null && icon.sprite != null )
				{
					// If the user wants to change color, then apply the highlighted color.
					if( radialMenu.iconColorChange )
						icon.color = radialMenu.iconHighlightedColor;

					// If the user wants to scale the transform of the icon, then apply the highlighted scale.
					if( radialMenu.iconScaleTransform )
						iconTransform.localScale = iconNormalScale * radialMenu.iconHighlightedScaleModifier;
				}

				// If the user wants to use text and it is assigned...
				if( radialMenu.useButtonText && text != null )
				{
					// If the user wants to change color, then apply the highlighted color.
					if( radialMenu.textColorChange )
						text.color = radialMenu.textHighlightedColor;
				}
			}
		}

		/// <summary>
		/// [INTERNAL] Invokes the functionality for when the input exits the button.
		/// </summary>
		public void OnExit ()
		{
			// If this button is disabled, then return...
			if( buttonDisabled )
				return;

			// If the application is not playing OR the current selected index is not this button...
			if( !Application.isPlaying || !Selected )
			{
				// If the user wants to swap sprites, then apply the normal sprite to the image.
				if( radialMenu.spriteSwap && radialMenu.normalSprite != null )
					radialImage.sprite = radialMenu.normalSprite;

				// If the user wants to change the color of the image, then apply the normal color.
				if( radialMenu.colorChange && radialImage.sprite != null )
					radialImage.color = radialMenu.normalColor;

				// If the user wants to scale the transform, then apply the normal scale and position.
				if( radialMenu.scaleTransform )
				{
					radialImage.GetComponent<RectTransform>().localScale = Vector3.one;
					radialImage.GetComponent<RectTransform>().localPosition = normalPosition;
				}

				// If the user wants to use the icon and it is assigned...
				if( radialMenu.useButtonIcon && icon != null && icon.sprite != null )
				{
					// If the user wants to change color, then apply the normal color.
					if( radialMenu.iconColorChange )
						icon.color = radialMenu.iconNormalColor;

					// If the user wants to scale the transform of the icon, then apply the normal scale.
					if( radialMenu.iconScaleTransform )
						iconTransform.localScale = iconNormalScale;
				}

				// If the user wants to use text and it is assigned...
				if( radialMenu.useButtonText && text != null )
				{
					// If the user wants to change color, then apply the normal color.
					if( radialMenu.textColorChange )
						text.color = radialMenu.textNormalColor;
				}
			}
		}

		/// <summary>
		/// [INTERNAL] Invokes the functionality for when the input is down on the button.
		/// </summary>
		public void OnInputDown ()
		{
			// If the button is disabled, then return.
			if( buttonDisabled )
				return;

			// If the user wants to swap sprites and the pressed sprite isn't null, then apply the sprite.
			if( radialMenu.spriteSwap && radialMenu.pressedSprite != null )
				radialImage.sprite = radialMenu.pressedSprite;

			// If the user wants to change the color, then apply the pressed color to the image.
			if( radialMenu.colorChange && radialImage.sprite != null )
				radialImage.color = radialMenu.pressedColor;

			// If the user wants to scale the transform...
			if( radialMenu.scaleTransform )
			{
				// Then apply the scale modifier and position.
				radialImage.GetComponent<RectTransform>().localScale = Vector3.one * radialMenu.pressedScaleModifier;
				radialImage.GetComponent<RectTransform>().localPosition = pressedPosition;
			}

			// If the user wants to use the icon and it is assigned...
			if( radialMenu.useButtonIcon && icon != null && icon.sprite != null )
			{
				// If the user wants to change color, then apply the pressed color.
				if( radialMenu.iconColorChange )
					icon.color = radialMenu.iconPressedColor;

				// If the user wants to scale the transform of the icon, then apply the normal scale multiplied by the pressed mod.
				if( radialMenu.iconScaleTransform )
					iconTransform.localScale = iconNormalScale * radialMenu.iconPressedScaleModifier;
			}

			// If the user wants to use text and it is assigned...
			if( radialMenu.useButtonText && text != null )
			{
				// If the user wants to change color, then apply the pressed color.
				if( radialMenu.textColorChange )
					text.color = radialMenu.textPressedColor;
			}
		}

		/// <summary>
		/// [INTERNAL] Invokes the functionality for when the input is release on the button.
		/// </summary>
		public void OnInputUp ()
		{
			// If the button is disabled, then return.
			if( buttonDisabled )
				return;

			// If this button is still the currently selected button, then return to the OnEnter() function.
			if( buttonIndex == radialMenu.CurrentButtonIndex )
				OnEnter();
			// Else call the OnExit() function because the input has left this button.
			else
				OnExit();
		}

		/// <summary>
		/// [INTERNAL] Invokes the functionality for when the input interacts with the button.
		/// </summary>
		public void OnInteract ()
		{
			// If this button is disabled, then return.
			if( buttonDisabled )
				return;

			// If the user wants to select the button when interacted with...
			if( radialMenu.selectButtonOnInteract )
			{
				// If the user wants to toggle the selected state and this button is currently selected, then call the Deselect function.
				if( radialMenu.toggleSelection && Selected )
					OnDeselect();
				// Else just select this button.
				else
					OnSelect();
			}

			// If the user has assigned a unity event to call, then call it.
			if( unityEvent != null )
				unityEvent.Invoke();

			// If the user has subscribed to the default callback then invoke it.
			if( OnRadialButtonInteract != null )
				OnRadialButtonInteract();

			// If the user has subscribed to the ID callback then invoke it with the assigned integer ID.
			else if( OnRadialButtonInteractWithId != null )
				OnRadialButtonInteractWithId( id );

			// If the user has subscribed to the string key callback then invoke it with the assigned key.
			else
				OnRadialButtonInteractWithKey?.Invoke( key );

			// Inform any subscribers that this button has been interacted with.
			radialMenu.OnButtonInteract?.Invoke( buttonIndex );
			radialMenu.OnRadialButtonInteract?.Invoke( buttonIndex );
		}

		/// <summary>
		/// [INTERNAL] Invokes the functionality for when this button is selected.
		/// </summary>
		public void OnSelect ()
		{
			// If the button is disabled or is already selected, then return.
			if( buttonDisabled || Selected )
				return;

			// Set selected to true for reference.
			Selected = true;

			// If the user only wants one button selected at a time...
			if( radialMenu.selectButtonOnInteract && !radialMenu.allowMultipleSelected )
			{
				// Loop through all the radial menu buttons...
				for( int i = 0; i < radialMenu.UltimateRadialButtonList.Count; i++ )
				{
					// If this index is the same as our index, then skip this index.
					if( i == buttonIndex )
						continue;

					// If the radial button is selected, deselect it.
					if( radialMenu.UltimateRadialButtonList[ i ].Selected )
						radialMenu.UltimateRadialButtonList[ i ].OnDeselect();
				}
			}

			// If the user wants to swap sprites, then apply the selected sprite to the image.
			if( radialMenu.spriteSwap && radialMenu.selectedSprite != null )
				radialImage.sprite = radialMenu.selectedSprite;

			// If the user wants to change the color of the radial button, then apply the selected color.
			if( radialMenu.colorChange && radialImage.sprite != null )
				radialImage.color = radialMenu.selectedColor;

			// If the user wants to scale the transform, then apply the scale and position.
			if( radialMenu.scaleTransform )
			{
				buttonTransform.localScale = Vector3.one * radialMenu.selectedScaleModifier;
				buttonTransform.localPosition = selectedPosition;
			}

			// If the user wants to use the icon and it is assigned...
			if( radialMenu.useButtonIcon && icon != null && icon.sprite != null )
			{
				// If the user wants to change color, then apply the selected color.
				if( radialMenu.iconColorChange )
					icon.color = radialMenu.iconSelectedColor;

				// If the user wants to scale the transform of the icon...
				if( radialMenu.iconScaleTransform )
					iconTransform.localScale = iconNormalScale * radialMenu.iconSelectedScaleModifier;
			}

			// If the user wants to use text and it is assigned...
			if( radialMenu.useButtonText && text != null )
			{
				// If the user wants to change color, then apply the selected color.
				if( radialMenu.textColorChange )
					text.color = radialMenu.textSelectedColor;
			}

			// Inform any subscribers that this button has been selected.
			radialMenu.OnButtonSelected?.Invoke( buttonIndex );
			radialMenu.OnRadialButtonSelected?.Invoke( buttonIndex );

			// Inform any subscribers that the selected state has changed.
			OnSelectedStateChanged?.Invoke( Selected );
		}

		/// <summary>
		/// [INTERNAL] Invokes the functionality for when this button is not selected anymore.
		/// </summary>
		public void OnDeselect ()
		{
			// If the button is disabled or is already deselected, then return.
			if( buttonDisabled || !Selected )
				return;

			// Set Selected to false.
			Selected = false;
			
			// Call OnExit to reset the button.
			OnExit();

			// If the current input angle is within range of this button, and the distance is within range, enter the button so it looks correct.
			if( IsInAngle( radialMenu.GetCurrentInputAngle ) && radialMenu.GetCurrentInputDistance < radialMenu.CalculatedMaxRange && radialMenu.GetCurrentInputDistance > radialMenu.CalculatedMinRange )
				OnEnter();

			// Inform any subscribers that the selected state has changed.
			OnSelectedStateChanged?.Invoke( Selected );
		}
		
		/// <summary>
		/// [INTERNAL] Invokes the functionality for when button is enabled.
		/// </summary>
		public void OnEnable ()
		{
			// If the image is unassigned, then return.
			if( radialImage == null )
				return;

			// Set the disable button to false so that calculations can continue on this button.
			buttonDisabled = false;

			// Call OnExit to reset the button.
			OnExit();
		}

		/// <summary>
		/// [INTERNAL] Invokes the functionality for when button is disabled.
		/// </summary>
		public void OnDisable ()
		{
			// If the image is unassigned, then return.
			if( radialImage == null )
				return;

			// Set the disable button to true so that nothing will be calculated on this button.
			buttonDisabled = true;

			// Set Selected to false since the button is now disabled.
			Selected = false;

			// If the user wants to use a disabled sprite then apply the sprite.
			if( radialMenu.spriteSwap && radialMenu.disabledSprite != null )
				radialImage.sprite = radialMenu.disabledSprite;

			// If the use wants to change the color, then apply the color.
			if( radialMenu.colorChange && radialImage.sprite != null )
				radialImage.color = radialMenu.disabledColor;

			// If the user is scaling the transform, then reset the scale and position.
			if( radialMenu.scaleTransform )
			{
				radialImage.GetComponent<RectTransform>().localScale = Vector3.one * radialMenu.disabledScaleModifier;
				radialImage.GetComponent<RectTransform>().localPosition = disabledPosition;
			}

			// If the user wants to use the icon and it is assigned...
			if( radialMenu.useButtonIcon && icon != null && icon.sprite != null )
			{
				// If the user wants to change color, then apply the disabled color.
				if( radialMenu.iconColorChange )
					icon.color = radialMenu.iconDisabledColor;

				// If the user wants to scale the transform of the icon, then apply the normal scale.
				if( radialMenu.iconScaleTransform )
					iconTransform.localScale = iconNormalScale * radialMenu.iconDisabledScaleModifier;
			}

			// If the user wants to use text, it is assigned, and the user wants to change the color, then apply the color.
			if( radialMenu.useButtonText && text != null && radialMenu.textColorChange )
				text.color = radialMenu.textDisabledColor;
		}

		/// <summary>
		/// Updates the icon image with the new sprite.
		/// </summary>
		/// <param name="newIcon">The new icon sprite to use.</param>
		public void UpdateIcon ( Sprite newIcon )
		{
			// If the icon is unassigned, log error and return.
			if( icon == null )
			{
				Debug.LogError( FormatDebug( "The button's icon image component is not assigned", "Please make sure that the radial menu has the <b>Button Icon</b> option enabled", radialMenu.gameObject.name ) );
				return;
			}

			// Assign the sprite to the provided sprite.
			icon.sprite = newIcon;

			// If the sprite is null, then change the color to clear to make it invisible.
			if( icon.sprite == null )
				icon.color = Color.clear;
			// Else the sprite is assigned, so apply the normal icon color.
			else
				icon.color = radialMenu.iconNormalColor;
		}

		/// <summary>
		/// Updates the text associated with this button to display the provided string.
		/// </summary>
		/// <param name="newText">The new string to apply to the text.</param>
		public void UpdateText ( string newText )
		{
			// If the text component is unassigned, then log error and return.
			if( text == null )
			{
				Debug.LogError( FormatDebug( "The button's text component is not assigned", "Please make sure that the radial menu has the <b>Button Text</b> option enabled", radialMenu.gameObject.name ) );
				return;
			}

			// Assign the text string.
			text.text = newText;
		}

		/// <summary>
		/// Updates the name of this button. Additionally, updates the display of anything that uses the name for text.
		/// </summary>
		/// <param name="newName">The new name to apply to this button.</param>
		public void UpdateName ( string newName )
		{
			// Assign the new name.
			name = newName;

			// If the radial menu is set to display the name and the text component is assigned, then apply the name.
			if( radialMenu.displayNameOnButton && text != null )
				text.text = name;

			// If the user has center text displaying the name of the button, then refresh the name text if this button is currently selected.
			if( radialMenu.displayButtonName && radialMenu.nameText != null && radialMenu.CurrentButtonIndex == buttonIndex )
				radialMenu.nameText.text = !buttonDisabled ? name : "";
		}
		
		/// <summary>
		/// Updates the description of this button and updates the needed text if it is the current button.
		/// </summary>
		/// <param name="newDescription">The new description to apply to this button.</param>
		public void UpdateDescription ( string newDescription )
		{
			// Assign the new description.
			description = newDescription;

			// If the user has center text displaying the description of the button, then refresh the text if this button is currently selected.
			if( radialMenu.displayButtonDescription && radialMenu.descriptionText != null && radialMenu.CurrentButtonIndex == buttonIndex )
				radialMenu.descriptionText.text = !buttonDisabled ? description : "";
		}

		/// <summary>
		/// Subscribes the provided function to the radial button interaction event.
		/// </summary>
		/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
		public void AddCallback ( Action ButtonCallback )
		{
			OnRadialButtonInteract += ButtonCallback;
		}

		/// <summary>
		/// Subscribes the provided function to the radial button interaction event.
		/// </summary>
		/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
		public void AddCallback ( Action<int> ButtonCallback )
		{
			OnRadialButtonInteractWithId += ButtonCallback;
		}

		/// <summary>
		/// Subscribes the provided function to the radial button interaction event.
		/// </summary>
		/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
		public void AddCallback ( Action<string> ButtonCallback )
		{
			OnRadialButtonInteractWithKey += ButtonCallback;
		}

		/// <summary>
		/// [Internal] Registers the information to the radial button.
		/// </summary>
		/// <param name="ButtonCallback">The Callback function to call when interacted with.</param>
		/// <param name="buttonInfo">The information to apply to the button.</param>
		public void RegisterButtonInfo ( UltimateRadialButtonInfo buttonInfo )
		{
			// Set registered to true since we are assigning the information.
			registered = true;

			// Assign this radial button to the buttonInfo so that it can reference this button.
			buttonInfo.radialButton = this;

			// If the button info ID is assigned, then copy the information over. Else copy the current button data to the buttonInfo.
			if( buttonInfo.id == 0 )
				buttonInfo.id = id;
			else
				id = buttonInfo.id;

			// If the button info key is assigned, then copy the information over. Else copy the current button data to the buttonInfo.
			if( buttonInfo.key == string.Empty )
				buttonInfo.key = key;
			else
				key = buttonInfo.key;

			// If the button info name is assigned, then copy the information over. Else copy the current button data to the buttonInfo.
			if( buttonInfo.name == string.Empty )
				buttonInfo.name = name;
			else
				name = buttonInfo.name;

			// If the button info description is assigned, then copy the information over. Else copy the current button data to the buttonInfo.
			if( buttonInfo.description == string.Empty )
				buttonInfo.description = description;
			else
				description = buttonInfo.description;
			
			// If the icon image is assigned and the user wants to show the icon...
			if( icon != null && radialMenu.useButtonIcon )
			{
				// If the provided icon sprite is assigned, then assign the sprite and set the color.
				if( buttonInfo.icon != null )
				{
					icon.sprite = buttonInfo.icon;
					icon.color = radialMenu.iconNormalColor;
				}
				// Else if the icon of the button is assigned, then assign this data to the button info.
				else if( icon.sprite != null )
				{
					buttonInfo.icon = icon.sprite;
					icon.color = radialMenu.iconNormalColor;
				}
				// Else just set the color of the icon image to clear.
				else
					icon.color = Color.clear;
			}

			// If the text isn't null then assign the radial info text.
			if( text != null && radialMenu.displayNameOnButton )
				text.text = buttonInfo.name;

			// If the button info is selected, then select this button.
			if( buttonInfo.Selected )
				OnSelect();

			// Subscribe the button info selected state changed function to the OnSelectedStateChanged callback.
			OnSelectedStateChanged += buttonInfo.OnSelectedStateChanged;

			// Subscribe the buttonInfo clear function so that it can be notified when this radial button is cleared.
			OnClearButtonInformation += buttonInfo.OnClearButtonInformation;
		}
		 
		/// <summary>
		/// Clears the button information.
		/// </summary>
		public void ClearButtonInformation ()
		{
			// Set registered and disabled to false since the button is being cleared.
			registered = false;
			buttonDisabled = false;

			// Reset the stored values.
			key = "";
			id = -1;
			name = "";
			description = "";
			Selected = false;

			// If the user has spriteSwap enabled, then change back the sprite.
			if( radialMenu.spriteSwap )
				radialImage.sprite = radialMenu.normalSprite;

			// Reset the button color.
			if( radialImage.sprite != null )
				radialImage.color = radialMenu.normalColor;

			// Reset the scale.
			if( radialMenu.scaleTransform )
				buttonTransform.localScale = Vector2.one;

			// Reset the button icon.
			if( icon != null )
			{
				icon.sprite = null;
				icon.color = Color.clear;
			}

			// If the button was applying unique positioning to the icon, then reset it so that it doesn't effect the next button information.
			if( useIconUnique )
				useIconUnique = false;

			// If the text component is assigned, then reset the text.
			if( text != null )
				text.text = "";
			
			// Notify any button information subscribers that this button has been cleared.
			OnClearButtonInformation?.Invoke();

			// Reset callbacks.
			OnRadialButtonInteract = null;
			OnRadialButtonInteractWithId = null;
			OnRadialButtonInteractWithKey = null;
			unityEvent = null;
			OnClearButtonInformation = null;
			OnSelectedStateChanged = null;
		}
		
		[Obsolete( "Please use the OnEnable function" )]
		public void EnableButton ()
		{
			OnEnable();
		}

		[Obsolete( "Please use the OnDisable function" )]
		public void DisableButton ()
		{
			OnDisable();
		}
	}
	public List<UltimateRadialButton> UltimateRadialButtonList = new List<UltimateRadialButton>();
	List<UltimateRadialButton> UltimateRadialButtonPool = new List<UltimateRadialButton>();
	public UltimateRadialMenuInputManager inputManager;

	// SCRIPT REFERENCE //
	static Dictionary<string, UltimateRadialMenu> UltimateRadialMenus = new Dictionary<string, UltimateRadialMenu>();
	public string radialMenuName = string.Empty;

	// ACTION SUBSCRIPTIONS //
	/// <summary>
	/// This callback will be called when a radial button has been entered.
	/// </summary>
	public event Action<int> OnButtonEnter;
	/// <summary>
	/// This callback will be called when a radial button has been exited.
	/// </summary>
	public event Action<int> OnButtonExit;
	/// <summary>
	/// This callback will be called when the input has been pressed on a radial button.
	/// </summary>
	public event Action<int> OnButtonInputDown;
	/// <summary>
	/// This callback will be called when the input has been released on a radial button.
	/// </summary>
	public event Action<int> OnButtonInputUp;
	/// <summary>
	/// This callback will be called when a radial button has been interacted with.
	/// </summary>
	public event Action<int> OnButtonInteract;
	/// <summary>
	/// This callback will be called when a radial button has been selected.
	/// </summary>
	public event Action<int> OnButtonSelected;
	/// <summary>
	/// This callback will be called when the radial menu has lost focus.
	/// </summary>
	public event Action OnMenuLostFocus;
	/// <summary>
	/// This callback will be called when the radial menu has been enabled.
	/// </summary>
	public event Action OnMenuEnabled;
	/// <summary>
	/// This callback will be called when the radial menu has been disabled.
	/// </summary>
	public event Action OnMenuDisabled;
	/// <summary>
	/// This callback will be called when the radial menu's positioning has been updated.
	/// </summary>
	public event Action OnUpdatePositioning;
	/// <summary>
	/// This callback will be called when a radial button is added or subtracted from the Radial Menu during this frame. This is useful for swapping sprites and positioning for a new count.
	/// </summary>
	public event Action<int> OnButtonCountModified;
	/// <summary>
	/// This callback relays the information that is provided from the Ultimate Radial Menu Input Manager script according to the users settings.
	/// </summary>
	public event Action<Vector2, float, bool, bool> OnProcessInput;

	#if UNITY_EDITOR
	#pragma warning disable CS0414
	// EDITOR //
	[SerializeField] private Sprite IconPlaceholderSprite = null;
	#pragma warning restore CS0414
	#endif


	void Awake ()
	{
		// If the application is not playing, then return.
		if( !Application.isPlaying )
			return;

		// If the name is assigned...
		if( radialMenuName != string.Empty )
		{
			// Check to see if the dictionary already contains this name, and if so, remove the current one.
			if( UltimateRadialMenus.ContainsKey( radialMenuName ) )
				UltimateRadialMenus.Remove( radialMenuName );

			// Register this UltimateRadialMenu into the dictionary.
			UltimateRadialMenus.Add( radialMenuName, GetComponent<UltimateRadialMenu>() );
		}

		// Assign the canvas group component.
		canvasGroup = GetComponent<CanvasGroup>();

		// Reset the stored indexes. 
		CurrentButtonIndex = -1;
		buttonIndexOnInputDown = -1;

		// Reset the menu.
		ResetRadialMenu();

		// Store the base transform.
		BaseTransform = GetComponent<RectTransform>();

		// Disable the radial menu immediately if the user wants it disabled.
		if( initialState == InitialState.Disabled )
		{
			IsEnabled = false;
			Interactable = false;
			DisableImmediate();
		}
		// Else set the radial menu as active and able to be interacted with.
		else
		{
			IsEnabled = true;
			Interactable = true;
		}

		// Make sure to store the reference of this radial menu to all the buttons. This is to avoid errors from the users trying to store buttonInfo instantly before the radial menu can get set up properly.
		for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
			UltimateRadialButtonList[ i ].radialMenu = this;
	}

	void Start ()
	{
#if ENABLE_INPUT_SYSTEM && UNITY_EDITOR
		// If the user has the new Input System and there is still an old input system component on the event system...
		if( FindObjectOfType<EventSystem>() && FindObjectOfType<EventSystem>().gameObject.GetComponent<StandaloneInputModule>() )
		{
			if( Application.isPlaying )
			{
				// Destroy the old component and add the new one so there will be no errors.
				DestroyImmediate( FindObjectOfType<EventSystem>().gameObject.GetComponent<StandaloneInputModule>() );
				FindObjectOfType<EventSystem>().gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
			}
#if UNITY_EDITOR
			else
			{
				UnityEditor.Undo.DestroyObjectImmediate( FindObjectOfType<EventSystem>().gameObject.GetComponent<StandaloneInputModule>() );
				UnityEditor.Undo.AddComponent( FindObjectOfType<EventSystem>().gameObject, typeof( UnityEngine.InputSystem.UI.InputSystemUIInputModule ) );
			}
#endif
		}
#endif

		// If the game is running, then return.
		if( !Application.isPlaying )
			return;

#if UNITY_EDITOR
		if( UnityEditor.EditorSettings.enterPlayModeOptionsEnabled && UnityEditor.EditorSettings.enterPlayModeOptions.HasFlag( UnityEditor.EnterPlayModeOptions.DisableSceneReload ) )
			Awake();
#endif

		// If there is an Input Manager script on this gameObject, then the input must be unique, so add this radial menu to it's list.
		if( GetComponent<UltimateRadialMenuInputManager>() )
			GetComponent<UltimateRadialMenuInputManager>().AddRadialMenuToList( this, ref inputManager );
		// Else, find the master Input Manager and add this radial menu to that list.
		else
		{
			// If there is no input manager, then add a input manager component to the event system.
			if( FindObjectOfType<EventSystem>() && !FindObjectOfType<EventSystem>().gameObject.GetComponent<UltimateRadialMenuInputManager>() )
			{
				Debug.LogWarning( FormatDebug( "There was no Ultimate Radial Menu Input Manager on the EventSystem in your scene. Adding a default Ultimate Radial Menu Input Manager to avoid errors", "Please exit play mode and ensure that you have an Ultimate Radial Menu Input Manager on your EventSystem so that you can customize the settings", FindObjectOfType<EventSystem>().gameObject.name + "(No Ultimate Radial Menu Input Manager component)" ) );
				
				FindObjectOfType<EventSystem>().gameObject.AddComponent<UltimateRadialMenuInputManager>().AddRadialMenuToList( this, ref inputManager );
			}
			// Else there is a input manager in the scene, so update the radial menu list.
			else
				UltimateRadialMenuInputManager.Instance.AddRadialMenuToList( this, ref inputManager );
		}

		// If the parent canvas is null...
		if( ParentCanvas == null )
		{
			// Update the ParentCanvas component.
			UpdateParentCanvas();

			// If it is still null, then warn the user and return to avoid errors.
			if( ParentCanvas == null )
			{
				Debug.LogError( FormatDebug( "This component is not with a Canvas object. Disabling this component to avoid any errors", "Please exit play mode and make sure that this Ultimate Radial Menu object is placed within a Canvas in your scene", gameObject.name ) );
				enabled = false;
				return;
			}
		}

		// If the user wants to display a pointer, but does not want the pointer to change color over time, then set PointerActive to true since it will never disappear.
		if( usePointer && !pointerColorChange )
			PointerActive = true;

		// Update the positioning of the Ultimate Radial Menu.
		UpdatePositioning();
	}

#if UNITY_EDITOR
	void Update ()
	{
		// If the application is not playing (in edit mode of Unity) update the positioning.
		if( !Application.isPlaying )
			UpdatePositioning();
	}

	void Reset ()
	{
		// If this game object does not have a RectTransform component, then replace the normal Transform component with a RectTranform.
		if( !GetComponent<RectTransform>() )
		{
			if( Application.isPlaying )
				gameObject.AddComponent<RectTransform>();
			else
				UnityEditor.Undo.AddComponent( gameObject, typeof( RectTransform ) );
		}
	}
#endif

	/// <summary>
	/// This function is called by Unity when the parent of this transform changes.
	/// </summary>
	void OnTransformParentChanged ()
	{
		UpdateParentCanvas();
	}

	/// <summary>
	/// Updates the parent canvas if it has changed.
	/// </summary>
	public void UpdateParentCanvas ()
	{
		// Store the parent of this object.
		Transform parent = transform.parent;

		// If the parent is null, then just return.
		if( parent == null )
			return;

		// While the parent is assigned...
		while( parent != null )
		{
			// If the parent object has a Canvas component, then assign the ParentCanvas and transform.
			if( parent.transform.GetComponent<UnityEngine.Canvas>() )
			{
				ParentCanvas = parent.transform.GetComponent<UnityEngine.Canvas>();
				canvasRectTrans = ParentCanvas.GetComponent<RectTransform>();
				return;
			}

			// If the parent does not have a canvas, then store it's parent to loop again.
			parent = parent.transform.parent;
		}
	}

	/// <summary>
	/// Processes the input and calculates the information for use within the Ultimate Radial Menu.
	/// </summary>
	/// <param name="input">The current input values.</param>
	/// <param name="distance">The distance of the input from the center of the radial menu.</param>
	/// <param name="inputDown">The state of the input being pressed this frame.</param>
	/// <param name="inputUp">The state of the input being released this frame.</param>
	public void ProcessInput ( Vector2 input, float distance, bool inputDown, bool inputUp )
	{
		// If the menu is not in world space, then cast the input to be relative to this transform. This is needed for if the radial menu is rotated like in a carousel type menu.
		if( !IsWorldSpaceRadialMenu )
			input = BaseTransform.InverseTransformPoint( BasePosition + ( Vector3 )input );

		// If the stored canvas size is not the same as the current canvas size, then update positioning.
		if( parentCanvasSize != canvasRectTrans.sizeDelta )
			UpdatePositioning();
		
		// If the radial menu is inactive or not interactable...
		if( !IsEnabled || !Interactable )
		{
			// Process the pointer and toggle.
			ProcessPointer();
			ProcessToggle();

			// If there are any subscribers to the OnRadialMenuProcessInput callback, notify them.
			OnProcessInput?.Invoke( input, distance, inputDown, inputUp );
			OnRadialMenuProcessInput?.Invoke( input, distance, inputDown, inputUp );

			// Then return.
			return;
		}

		// Set the InputInRange bool to false by default.
		InputInRange = false;

		// Calculate the angle of the input and convert it to degrees.
		float angle = Mathf.Atan2( input.y, input.x ) * Mathf.Rad2Deg;

		// If the angle is negative, then add 360 to make it positive.
		if( angle < 0 )
			angle += 360;

		// Store the current angle and distance so that other scripts can get it.
		GetCurrentInputAngle = angle;
		GetCurrentInputDistance = distance;

		// Loop through all of the radial menu buttons...
		for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
		{
			// If the distance of the input exceeds the boundaries of the radial menu...
			if( distance < CalculatedMinRange || distance > CalculatedMaxRange )
			{
				// If the input was in range the last frame...
				if( inputInRangeLastFrame )
				{
					// Reset the radial menu.
					ResetRadialMenu();

					// Notify any subscriptions that the radial menu has lost focus.
					OnMenuLostFocus?.Invoke();
					OnRadialMenuLostFocus?.Invoke();
				}

				// Break the loop.
				break;
			}

			// If there was a current button with input on it, and the input is still within the angle of that button, then set InputInRange to true and break the loop.
			if( CurrentButtonIndex >= 0 && CurrentButtonIndex < UltimateRadialButtonList.Count && UltimateRadialButtonList[ CurrentButtonIndex ].IsInAngle( angle ) )
			{
				InputInRange = true;
				break;
			}

			// If the angle is within the range of the current radial menu button...
			if( UltimateRadialButtonList[ i ].IsInAngle( angle ) )
			{
				// Set the InputInRange to true.
				InputInRange = true;
				
				// If the current index is greater than -1, then exit the current button.
				if( CurrentButtonIndex >= 0 && CurrentButtonIndex < UltimateRadialButtonList.Count )
				{
					buttonIndexOnInputDown = -1;	
					UltimateRadialButtonList[ CurrentButtonIndex ].OnExit();

					// Inform and subscribers that this button has been exited.
					OnButtonExit?.Invoke( CurrentButtonIndex );
					OnRadialButtonExit?.Invoke( CurrentButtonIndex );
				}

				// Assign the current button index to this index.
				CurrentButtonIndex = i;

				// Call the OnEnter function on the current button.
				UltimateRadialButtonList[ i ].OnEnter();

				// If the invoke action is set to being when the button is entered, then call the OnInteract() function on the current button.
				if( inputManager.invokeAction == UltimateRadialMenuInputManager.InvokeAction.OnButtonEnter )
					UltimateRadialButtonList[ CurrentButtonIndex ].OnInteract();

				// Inform any subscribers that this button has been entered.
				OnButtonEnter?.Invoke( i );
				OnRadialButtonEnter?.Invoke( i );

				// If the user wants to display the pointer, and it is assigned...
				if( usePointer && pointerTransform != null )
				{
					// Store the target rotation to being the angle of the radial button at the index.
					pointerTargetRotation = Quaternion.Euler( 0, 0, UltimateRadialButtonList[ CurrentButtonIndex ].angle - pointerRotationOffset );

					// If the radial menu has not been focused yet, or the user wants to apply the rotation instantly, then simply apply the rotation.
					if( !PointerActive || pointerSnapOption == PointerSnapOption.Instant )
						pointerTransform.localRotation = pointerTargetRotation;
				}

				// If the user wants to display the name of the button and the text is assigned, then apply the name.
				if( displayButtonName && nameText != null )
					nameText.text = !UltimateRadialButtonList[ i ].buttonDisabled ? UltimateRadialButtonList[ i ].name : "";

				// If the user wants to display the description of the button and the text is assigned, then display the description.
				if( displayButtonDescription && descriptionText != null )
					descriptionText.text = !UltimateRadialButtonList[ i ].buttonDisabled ? UltimateRadialButtonList[ i ].description : "";
				
				// Break the loop.
				break;
			}

			// If this loop has reached the end of the button list and no buttons have been found worthy...
			if( i == UltimateRadialButtonList.Count - 1 )
			{
				// Reset the radial menu.
				ResetRadialMenu();

				// Inform any subscribers that the radial menu has lost focus.
				OnMenuLostFocus?.Invoke();
				OnRadialMenuLostFocus?.Invoke();
			}
		}

		// If the last frame caught input, but now the radial button list is zero...
		if( inputInRangeLastFrame && UltimateRadialButtonList.Count == 0 )
		{
			// Inform any subscribers that the radial menu has lost focus.
			OnMenuLostFocus?.Invoke();
			OnRadialMenuLostFocus?.Invoke();

			// Reset the radial menu.
			ResetRadialMenu();
		}

		// If the input is down on this frame, the input is in range, and the current index is within range...
		if( CurrentButtonIndex >= 0 && !UltimateRadialButtonList[ CurrentButtonIndex ].buttonDisabled && inputDown && InputInRange )
		{
			// Call the OnInputDown() function on the current button.
			if( !UltimateRadialButtonList[ CurrentButtonIndex ].Selected )
				UltimateRadialButtonList[ CurrentButtonIndex ].OnInputDown();

			// If the invoke action is set to being when the button is down...
			if( inputManager.invokeAction == UltimateRadialMenuInputManager.InvokeAction.OnButtonDown )
			{
				// Call the OnInteract() function on the current button.
				UltimateRadialButtonList[ CurrentButtonIndex ].OnInteract();
				
				// If the input manager wants to disable the radial menu when interacted with, then disable the menu.
				if( inputManager.disableOnInteract && !IsWorldSpaceRadialMenu )
					Disable();
			}

			// Set the button index to the buttonIndexOnInputDown so that the button up can be calculated.
			buttonIndexOnInputDown = CurrentButtonIndex;

			// Inform and subscribers that this button has received down input.
			OnButtonInputDown?.Invoke( CurrentButtonIndex );
			OnRadialButtonInputDown?.Invoke( CurrentButtonIndex );
		}

		// If the input is up on this frame, the input is in range, and the current index is within range...
		if( CurrentButtonIndex >= 0 && !UltimateRadialButtonList[ CurrentButtonIndex ].buttonDisabled && inputUp && InputInRange )
		{
			// Call the OnInputUp() function on the current button.
			if( !UltimateRadialButtonList[ CurrentButtonIndex ].Selected )
				UltimateRadialButtonList[ CurrentButtonIndex ].OnInputUp();
			
			// If the invoke action is set to being when the button has been clicked, and the current button index is the same as when the buttonIndexOnInputDown was calculated...
			if( ( inputManager.invokeAction == UltimateRadialMenuInputManager.InvokeAction.OnButtonClick && CurrentButtonIndex == buttonIndexOnInputDown ) || inputManager.invokeAction == UltimateRadialMenuInputManager.InvokeAction.OnButtonUp )
			{
				// Call the OnInteract() function on the current button.
				UltimateRadialButtonList[ CurrentButtonIndex ].OnInteract();
				
				// If the input manager wants to disable the radial menu when interacted with, then disable the menu.
				if( inputManager.disableOnInteract && !IsWorldSpaceRadialMenu )
					Disable();
			}

			// Reset the buttonIndexOnInputDown.
			buttonIndexOnInputDown = -1;

			// Inform and subscribers that this button has received up input.
			OnButtonInputUp?.Invoke( CurrentButtonIndex );
			OnRadialButtonInputUp?.Invoke( CurrentButtonIndex );
		}

		// Process the pointer before the inputInRangeLastFrame has been updated.
		ProcessPointer();

		// Process the toggle of the radial menu.
		ProcessToggle();

		// If there are any subscribers to the OnRadialMenuProcessInput callback, notify them.
		OnProcessInput?.Invoke( input, distance, inputDown, inputUp );
		OnRadialMenuProcessInput?.Invoke( input, distance, inputDown, inputUp );

		// Store the InputInRange value for the next calculation.
		inputInRangeLastFrame = InputInRange;
	}

	/// <summary>
	/// [INTERNAL] Resets the Ultimate Radial Menu Buttons and all the enabled options to their default state.
	/// </summary>
	void ResetRadialMenu ()
	{
		// If the current index within range of the list count...
		if( CurrentButtonIndex >= 0 && CurrentButtonIndex < UltimateRadialButtonList.Count )
		{
			// Exit the current button.
			UltimateRadialButtonList[ CurrentButtonIndex ].OnExit();

			// If there are any subscribers to the exit callback, inform them.
			OnButtonExit?.Invoke( CurrentButtonIndex );
			OnRadialButtonExit?.Invoke( CurrentButtonIndex );
		}

		// Reset all the stored accessors.
		CurrentButtonIndex = -1;
		buttonIndexOnInputDown = -1;
		GetCurrentInputAngle = 0.0f;
		GetCurrentInputDistance = 0.0f;

		// If the user is wanting to display the current selection on the radial menu, reset the text.
		if( displayButtonName && nameText != null )
			nameText.text = "";

		// If the user is wanting to display the description of the selected button, then reset the text here.
		if( displayButtonDescription && descriptionText != null )
			descriptionText.text = "";
	}

	/// <summary>
	/// [INTERNAL] Processes the pointer according to the users options.
	/// </summary>
	void ProcessPointer ()
	{
		// If the user wants to apply the rotation instantly ( which has already been done when the new button was found ) or the pointer transform is null, then return.
		if( !usePointer || pointerSnapOption == PointerSnapOption.Instant || pointerTransform == null )
			return;

		// If the user wants the color of the pointer to change...
		if( pointerColorChange )
		{
			// If the last known input is different than the current input in range...
			if( inputInRangeLastFrame != InputInRange )
			{
				// If the current input is within range of the radial menu...
				if( InputInRange )
				{
					// If the user does not want to change the pointer color over time, OR the fade in duration is zero...
					if( !changeOverTime || fadeInDuration <= 0.0f )
					{
						// Set PointerActive to true for reference.
						PointerActive = true;

						// Set the pointer color to the active color.
						pointerImage.color = pointerActiveColor;

						// If the fade out duration is set and the pointer is currently fading out...
						if( fadeOutDuration > 0.0f && pointerFadeOut )
						{
							// Set pointerFadeOut to false so it won't fade, and reset the lerp value.
							pointerFadeOut = false;
							pointerLerpValue = 0.0f;
						}
					}
					// Else the user either does want to fade over time, and the fade in duration is set...
					else
					{
						// Set pointerFadeIn to true so it will run, and store the current color of the pointer.
						pointerFadeIn = true;
						pointerFadeColor = pointerImage.color;

						// Calculate the fade in speed and reset the lerp value.
						fadeInSpeed = 1.0f / fadeInDuration;
						pointerLerpValue = 0.0f;

						// If the pointer is currently fading out, set pointerFadeOut to false so that it won't continue to fade out.
						if( pointerFadeOut )
							pointerFadeOut = false;
					}
				}
				// Else the current input is not in range of the menu...
				else
				{
					// If the user doesn't want to change the pointer color over time, or the fade out duration is not assigned...
					if( !changeOverTime || fadeOutDuration <= 0.0f )
					{
						// Set PointerActive to false for reference.
						PointerActive = false;

						// Set the pointer image color to the normal color.
						pointerImage.color = pointerNormalColor;

						// If the fade in duration is set, and the pointer is currently fading in...
						if( fadeInDuration > 0.0f && pointerFadeIn )
						{
							// Set the fade in controller to false and reset the lerp value.
							pointerFadeIn = false;
							pointerLerpValue = 0.0f;
						}
					}
					// Else the user doesn't want to fade over time, and the fade out duration is set...
					else
					{
						// Set pointerFadeOut to true so it will run, and store the current color.
						pointerFadeOut = true;
						pointerFadeColor = pointerImage.color;

						// Calculate the speed and reset the lerp value.
						fadeOutSpeed = 1.0f / fadeOutDuration;
						pointerLerpValue = 0.0f;

						// If the pointer is currently fading in, set it to false so it won't run.
						if( pointerFadeIn )
							pointerFadeIn = false;
					}
				}
			}

			// If the pointer needs to be faded in...
			if( pointerFadeIn )
			{
				// Set PointerActive to true for reference.
				PointerActive = true;
				
				// Lerp the pointer value for the color over time.
				pointerLerpValue += Time.unscaledDeltaTime * fadeInSpeed;

				// Transition the color from normal to active by pointerLerpValue.
				pointerImage.color = Color.Lerp( pointerFadeColor, pointerActiveColor, pointerLerpValue );

				// If the pointer is still focused then apply the final color.
				if( pointerLerpValue >= 1.0f )
				{
					pointerImage.color = pointerActiveColor;
					pointerFadeIn = false;
					pointerLerpValue = 0.0f;
				}
			}
			// Else if the pointer needs to be faded out...
			else if( pointerFadeOut )
			{
				// Lerp the pointer value.
				pointerLerpValue += Time.unscaledDeltaTime * fadeOutSpeed;

				// Transition the color from the current color to normal by the pointerLerpValue.
				pointerImage.color = Color.Lerp( pointerFadeColor, pointerNormalColor, pointerLerpValue );

				// If the lerp value is over 1.0f...
				if( pointerLerpValue >= 1.0f )
				{
					// Set pointerFadeOut to false since it has completed.
					pointerFadeOut = false;

					// Apply the final color.
					pointerImage.color = pointerNormalColor;

					// Reset the lerp value and set PointerActive to false for reference.
					pointerLerpValue = 0.0f;
					PointerActive = false;
				}
			}
		}
		
		// If the pointer is not active and visible, then just return.
		if( !PointerActive )
			return;

		// If the snapping option is set to free, then transition the rotation to the current angle of the input.
		if( pointerSnapOption == PointerSnapOption.Free )
			pointerTransform.localRotation = Quaternion.Slerp( pointerTransform.localRotation, Quaternion.Euler( 0, 0, GetCurrentInputAngle - pointerRotationOffset ), Time.unscaledDeltaTime * pointerTargetSpeed );
		// Else transition the rotation to the target rotation of the currently selected button.
		else
			pointerTransform.localRotation = Quaternion.Slerp( pointerTransform.localRotation, pointerTargetRotation, Time.unscaledDeltaTime * pointerTargetSpeed );
	}
	
	/// <summary>
	/// [INTERNAL] Processes the toggle of the menu over time.
	/// </summary>
	void ProcessToggle ()
	{
		// If the user doesn't want to toggle the menu over time, then return.
		if( !menuToggleOverTime )
			return;

		// If the menu needs to be toggled in...
		if( toggleIn )
		{
			// Lerp the toggle value over time according to the users settings.
			toggleLerpValue += Time.unscaledDeltaTime * toggleInSpeed;

			// If the user wants to use the canvas group to fade the menu, then transition the alpha from current to full by the lerp value.
			if( menuToggleAlpha )
				canvasGroup.alpha = Mathf.Lerp( toggleAlphaValue, 1.0f, toggleLerpValue );

			// If the user wants to have the scale of the menu updated, then lerp the scale.
			if( menuToggleScale )
				BaseTransform.localScale = Vector3.one * Mathf.Lerp( toggleScaleValue, 1.0f, toggleLerpValue );

			// If the menu is still focused and the lerp value is complete...
			if( IsEnabled && toggleLerpValue >= 1.0f )
			{
				// Set toggleIn and the lerp value to default.
				toggleIn = false;
				toggleLerpValue = 0.0f;
				Interactable = true;

				// If the user wants use the canvas group alpha, then apply the full alpha.
				if( menuToggleAlpha )
					canvasGroup.alpha = 1.0f;

				// If the user wants to scale the menu, apply the full scale.
				if( menuToggleScale )
					BaseTransform.localScale = Vector3.one;
			}
		}
		// Else if the menu needs to be toggled out...
		else if( toggleOut )
		{
			// Lerp the toggle value.
			toggleLerpValue += Time.unscaledDeltaTime * toggleOutSpeed;

			// If the user wants to fade the alpha, then transition the alpha from current to zero by the lerp value.
			if( menuToggleAlpha )
				canvasGroup.alpha = Mathf.Lerp( toggleAlphaValue, 0.0f, toggleLerpValue );

			// If the user wants to scale the menu, then lerp the scale by the lerp value.
			if( menuToggleScale )
				BaseTransform.localScale = Vector3.one * Mathf.Lerp( toggleScaleValue, 0.0f, toggleLerpValue );

			// If the lerp value is over 1.0f...
			if( toggleLerpValue >= 1.0f )
			{
				// Set toggleOut and the lerp value to default.
				toggleOut = false;
				toggleLerpValue = 0.0f;

				// If the user wants use the canvas group alpha, then apply the zero alpha.
				if( menuToggleAlpha )
					canvasGroup.alpha = 0.0f;

				// If the user wants to scale the menu, apply the zero scale.
				if( menuToggleScale )
					BaseTransform.localScale = Vector3.zero;
			}
		}
	}
	
	/// <summary>
	/// Returns the ratio of the targeted sprite.
	/// </summary>
	/// <param name="sprite">The sprite to calculate the ratio of.</param>
	Vector2 GetImageAspectRatio ( Sprite sprite )
	{
		Vector2 ratio = Vector2.one;
		
		// Store the raw values of the sprites ratio so that a smaller value can be configured.
		Vector2 rawRatio = new Vector2( sprite.rect.width, sprite.rect.height );

		// Temporary float to store the largest side of the sprite.
		float maxValue = rawRatio.x > rawRatio.y ? rawRatio.x : rawRatio.y;

		// Now configure the ratio based on the above information.
		ratio.x = rawRatio.x / maxValue;
		ratio.y = rawRatio.y / maxValue;

		return ratio;
	}

	/// <summary>
	/// Creates a button at the specified index.
	/// </summary>
	/// <param name="buttonIndex">The targeted button index to create the button at.</param>
	void CreateRadialButtonAtIndex ( int buttonIndex )
	{
		// If the stored button index is within range, then exit the button since the menu is shifting.
		if( CurrentButtonIndex >= 0 && CurrentButtonIndex < UltimateRadialButtonList.Count )
			UltimateRadialButtonList[ CurrentButtonIndex ].OnExit();

		// If the button index is less than zero, then make is zero to avoid errors.
		if( buttonIndex < 0 )
			buttonIndex = 0;

		// If the button index is higher than the button list count, then assign it to the end of the list.
		if( buttonIndex > UltimateRadialButtonList.Count )
			buttonIndex = UltimateRadialButtonList.Count;

		// If the radial menu style is assigned and the count is greater than the max button count for this style, then warn the user.
		if( radialMenuStyle != null && UltimateRadialButtonList.Count >= radialMenuStyle.maxButtonCount )
			Debug.LogWarning( FormatDebug( "The current button count is out of range for this style. The buttons may look strange because there is no corresponding button sprite to use with this count", "To ensure that the buttons look good on the menu, please limit the number of buttons to correspond to the style you are using. Otherwise, consider using a radial menu that doesn't have a set number of buttons to use", gameObject.name ) );

		// If there is a radial button in the pool list, then insert the first radial button from the pool list.
		if( UltimateRadialButtonPool.Count > 0 )
			UltimateRadialButtonList.Insert( buttonIndex, GetRadialButtonFromPool() );
		// Else there is no pooled buttons, so create a new one.
		else
		{
			// Insert a new radial button at the targeted index.
			UltimateRadialButtonList.Insert( buttonIndex, new UltimateRadialButton() );

			// Assign the radial menu variable.
			UltimateRadialButtonList[ buttonIndex ].radialMenu = this;

			// Create a new base game object and add a RectTransform component.
			GameObject newGameObject = new GameObject();
			newGameObject.AddComponent<RectTransform>();
			newGameObject.AddComponent<CanvasRenderer>();
			newGameObject.AddComponent<Image>();

			// Create the button game object, set the parent, and change the name.
			GameObject radialButton = Instantiate( newGameObject, Vector3.zero, Quaternion.identity );

			if( BaseTransform == null )
				BaseTransform = GetComponent<RectTransform>();

			radialButton.transform.SetParent( BaseTransform );
			radialButton.gameObject.name = "Radial Menu Button";
			radialButton.transform.SetAsLastSibling();

			// Store the RectTransform component and modify it.
			UltimateRadialButtonList[ buttonIndex ].buttonTransform = radialButton.GetComponent<RectTransform>();
			UltimateRadialButtonList[ buttonIndex ].buttonTransform.anchorMin = new Vector2( 0.5f, 0.5f );
			UltimateRadialButtonList[ buttonIndex ].buttonTransform.anchorMax = new Vector2( 0.5f, 0.5f );
			UltimateRadialButtonList[ buttonIndex ].buttonTransform.pivot = new Vector2( 0.5f, 0.5f );
			UltimateRadialButtonList[ buttonIndex ].buttonTransform.localScale = Vector3.one;

			// Store the image component and update the sprite and color.
			UltimateRadialButtonList[ buttonIndex ].radialImage = radialButton.GetComponent<Image>();
			UltimateRadialButtonList[ buttonIndex ].radialImage.sprite = normalSprite;
			if( UltimateRadialButtonList[ buttonIndex ].radialImage.sprite != null )
				UltimateRadialButtonList[ buttonIndex ].radialImage.color = normalColor;
			else
				UltimateRadialButtonList[ buttonIndex ].radialImage.color = Color.clear;

			// If the user wants to display a icon on the button...
			if( useButtonIcon )
			{
				// Create the button icon game object. Then set the parent as the baseButton's transform, and change the name.
				GameObject buttonIcon = Instantiate( newGameObject, Vector3.zero, Quaternion.identity );
				buttonIcon.transform.SetParent( UltimateRadialButtonList[ buttonIndex ].buttonTransform );
				buttonIcon.gameObject.name = "Icon " + buttonIndex.ToString( "00" );

				// Store the RectTransform component and set it to fill up the parent transform.
				UltimateRadialButtonList[ buttonIndex ].iconTransform = buttonIcon.GetComponent<RectTransform>();
				UltimateRadialButtonList[ buttonIndex ].buttonTransform.anchorMin = new Vector2( 0.5f, 0.5f );
				UltimateRadialButtonList[ buttonIndex ].buttonTransform.anchorMax = new Vector2( 0.5f, 0.5f );
				UltimateRadialButtonList[ buttonIndex ].buttonTransform.pivot = new Vector2( 0.5f, 0.5f );
				UltimateRadialButtonList[ buttonIndex ].iconTransform.localScale = Vector3.one;

				// Store the buttonIcon image component and clear the sprite and update the color to clear.
				UltimateRadialButtonList[ buttonIndex ].icon = buttonIcon.GetComponent<Image>();
				UltimateRadialButtonList[ buttonIndex ].icon.sprite = null;
				UltimateRadialButtonList[ buttonIndex ].icon.color = Color.clear;
			}

			// If the user wants to use text on the button...
			if( useButtonText )
			{
				// Create a new game object for the text, add a RectTransform and a CanvasRenderer component.
				GameObject newTextObject = new GameObject();
				newTextObject.AddComponent<RectTransform>();
				newTextObject.AddComponent<CanvasRenderer>();

				// Create the cooldown text object, set the parent and name.
				GameObject buttonText = Instantiate( newTextObject, Vector3.zero, Quaternion.identity );
				buttonText.transform.SetParent( UltimateRadialButtonList[ buttonIndex ].buttonTransform );
				buttonText.gameObject.name = "Text " + buttonIndex.ToString( "00" );

				// Store the text component and modify the settings.
				UltimateRadialButtonList[ buttonIndex ].text = buttonText.AddComponent<Text>();
				UltimateRadialButtonList[ buttonIndex ].text.text = "";
				UltimateRadialButtonList[ buttonIndex ].text.alignment = TextAnchor.MiddleCenter;
				UltimateRadialButtonList[ buttonIndex ].text.resizeTextForBestFit = true;
				UltimateRadialButtonList[ buttonIndex ].text.resizeTextMinSize = 0;
				UltimateRadialButtonList[ buttonIndex ].text.resizeTextMaxSize = 300;
				if( buttonTextFont != null )
					UltimateRadialButtonList[ buttonIndex ].text.font = buttonTextFont;
				UltimateRadialButtonList[ buttonIndex ].text.color = textNormalColor;
				UltimateRadialButtonList[ buttonIndex ].text.rectTransform.localScale = Vector3.one;

				// If the user wants a text outline on the button text...
				if( buttonTextOutline )
				{
					// Add a outline component and update the color.
					UnityEngine.UI.Outline textOutline = buttonText.AddComponent<UnityEngine.UI.Outline>();
					textOutline.effectColor = buttonTextOutlineColor;
				}

				// Destroy the temporary text object that was created.
				Destroy( newTextObject );
			}

			// Destroy the temporary game object.
			Destroy( newGameObject );
		}
		
		// Calculate the new information since the count has been modified.
		RadialMenuButtonCountModified();
	}

	/// <summary>
	/// Finds the radial button index according to the provided integer index. This function will "fix" the index to make sure that it does what the user wants it to do.
	/// </summary>
	/// <param name="buttonIndex">The button index to modify.</param>
	void FindRadialButtonIndex ( ref int buttonIndex )
	{
		// If the button index is unassigned, then find the next available button.
		if( buttonIndex < 0 )
		{
			// Loop through each button.
			for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
			{
				// If the button in not currently registered to anything, then assign this index.
				if( !UltimateRadialButtonList[ i ].Registered && !UltimateRadialButtonList[ i ].buttonDisabled )
				{
					buttonIndex = i;
					return;
				}
			}

			// If we have reached this part of code without finding any available buttons, then create a new button.
			CreateRadialButtonAtIndex( 1000 );

			// Set the index to the new button.
			buttonIndex = UltimateRadialButtonList.Count - 1;
			return;
		}

		// If the button index is greater than the count...
		if( buttonIndex >= UltimateRadialButtonList.Count )
		{
			// Create a radial button at the end of the list.
			CreateRadialButtonAtIndex( 1000 );

			// Update the button index to the last button in the list.
			buttonIndex = UltimateRadialButtonList.Count - 1;
			return;
		}

		// If the targeted button is registered or if is it disabled then insert a button at this index.
		if( UltimateRadialButtonList[ buttonIndex ].Registered || UltimateRadialButtonList[ buttonIndex ].buttonDisabled )
			CreateRadialButtonAtIndex( buttonIndex );
	}
	
	/// <summary>
	/// Returns the first radial button in the pool and removes it from the list.
	/// </summary>
	UltimateRadialButton GetRadialButtonFromPool ()
	{
		// Grab the first pooled button, enable it, and remove it from the pool.
		UltimateRadialButton radialButton = UltimateRadialButtonPool[ 0 ];
		radialButton.buttonTransform.gameObject.SetActive( true );
		UltimateRadialButtonPool.Remove( radialButton );

		// Return the radial button.
		return radialButton;
	}

	/// <summary>
	/// Sends the radial button to the pool and removes it from the list of active buttons.
	/// </summary>
	/// <param name="buttonIndex">The targeted button index to send to the pool.</param>
	void SendRadialButtonToPool ( int buttonIndex )
	{
		// Add the targeted button to the pool list.
		UltimateRadialButtonPool.Add( UltimateRadialButtonList[ buttonIndex ] );
		
		// Set the game object to not active.
		UltimateRadialButtonList[ buttonIndex ].buttonTransform.gameObject.SetActive( false );

		// Remove this button from the list.
		UltimateRadialButtonList.RemoveAt( buttonIndex );
	}

	/// <summary>
	/// This updates the radial button images to fit the new number of buttons according to any styles that the user has set.
	/// </summary>
	void RadialMenuButtonCountModified ()
	{
		// If the user has set a style to use...
		if( radialMenuStyle != null )
		{
			// Loop through each style and check for the corresponding button count.
			for( int i = 0; i < radialMenuStyle.RadialMenuStyles.Count; i++ )
			{
				// Configure the angle value of this style button count.
				float styleAngle = 360.0f / radialMenuStyle.RadialMenuStyles[ i ].buttonCount;

				// If the calculated angle is less than or equal to the current needed angle...
				if( styleAngle <= GetAnglePerButton )
				{
					// Store this index and break the loop.
					currentStyleIndex = i;
					break;
				}
			}
			
			// Apply the new style normal sprite for the buttons.
			normalSprite = radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].normalSprite;

			// If the user is wanting to use the Sprite Swap feature, then apply the sprites from this style.
			if( spriteSwap )
			{
				highlightedSprite = radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].highlightedSprite ?? normalSprite;
				pressedSprite = radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].pressedSprite ?? normalSprite;
				selectedSprite = radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].selectedSprite ?? normalSprite;
				disabledSprite = radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].disabledSprite ?? normalSprite;
			}

			// Loop through each radial button and update the image sprite to the new normal sprite.
			for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
			{
				// If the button is disabled, then assign the disabled sprite.
				if( UltimateRadialButtonList[ i ].buttonDisabled && spriteSwap )
					UltimateRadialButtonList[ i ].radialImage.sprite = disabledSprite;
				// Else if the button is selected, then assign the selected sprite.
				else if( UltimateRadialButtonList[ i ].Selected && spriteSwap )
					UltimateRadialButtonList[ i ].radialImage.sprite = selectedSprite;
				// Else just assign the normal sprite.
				else
					UltimateRadialButtonList[ i ].radialImage.sprite = normalSprite;
			}
		}

		// If the user wants to have a pointer image, and it is assigned...
		if( usePointer && pointerImage != null )
		{
			// If the user wants to use a pointer style setup...
			if( usePointerStyle )
			{
				// Loop through each style and check for the corresponding button count.
				for( int i = 0; i < PointerStyles.Count; i++ )
				{
					// If this button count is lower than the pointer style button count...
					if( UltimateRadialButtonList.Count <= PointerStyles[ i ].buttonCount )
					{
						// Assign this sprite and break the loop.
						pointerImage.sprite = PointerStyles[ i ].pointerSprite;
						break;
					}
				}
			}

			// If the user wants the sibling index set...
			if( pointerSiblingIndex != SetSiblingIndex.Disabled )
			{
				// If the user wants this pointer as the last sibling, then set that.
				if( pointerSiblingIndex == SetSiblingIndex.Last )
					pointerImage.transform.SetAsLastSibling();
				// Else set this transform as the first sibling.
				else if( pointerSiblingIndex == SetSiblingIndex.First && pointerImage.transform.GetSiblingIndex() > 0 )
					pointerImage.transform.SetAsFirstSibling();
			}
		}

		// Inform any subscribers that the count has been modified.
		OnButtonCountModified?.Invoke( UltimateRadialButtonList.Count );
		OnRadialMenuButtonCountModified?.Invoke( UltimateRadialButtonList.Count );

		// Update the positioning.
		UpdatePositioning();
	}

	// -------------------------------------------------- < PUBLIC FUNCTIONS FOR THE USER > -------------------------------------------------- //
	/// <summary>
	/// Updates the positioning of the radial menu according to the user's options.
	/// </summary>
	public void UpdatePositioning ()
	{
		// If the parent canvas is null, then try to get the parent canvas component.
		if( ParentCanvas == null )
			UpdateParentCanvas();
		
		// If it is still null, then log a error and return.
		if( ParentCanvas == null )
		{
			Debug.LogError( FormatDebug( "There is no parent canvas object", "Please make sure that the Ultimate Radial Menu is placed within a canvas", gameObject.name ) );
			return;
		}

		// Store the canvas size.
		parentCanvasSize = canvasRectTrans.sizeDelta;

		// Set the current reference size for scaling.
		float referenceSize = scalingAxis == ScalingAxis.Height ? canvasRectTrans.sizeDelta.y : canvasRectTrans.sizeDelta.x;
		
		// Configure the target size for the graphic.
		float textureSize = referenceSize * ( menuSize / 5 );

		// If baseTrans is null, store this object's RectTrans so that it can be positioned.
		if( BaseTransform == null )
			BaseTransform = GetComponent<RectTransform>();
		
		// Apply the scale of 1 for calculations.
		BaseTransform.localScale = Vector3.one;

		// If the pivot is not 0.5, then set the pivot.
		if( BaseTransform.pivot != Vector2.one / 2 )
			BaseTransform.pivot = Vector2.one / 2;

		// First, fix the positioning to be a value between -0.5f and 0.5f.
		Vector2 fixedPositioning = new Vector2( horizontalPosition - 50, verticalPosition - 50 ) / 100;
		
		// Apply the positioning to the baseTransform.
		BaseTransform.localPosition = ( Vector3 )( canvasRectTrans.sizeDelta * fixedPositioning ) - ( Vector3 )( ( Vector2.one * textureSize ) * fixedPositioning );

		// Apply the texture size to the baseTransform.
		BaseTransform.sizeDelta = new Vector2( textureSize, textureSize );

		// Set the local rotation to zero.
		BaseTransform.localRotation = Quaternion.identity;
		
		// Store the default position of the radial menu.
		defaultPosition = BaseTransform.position;
		
		// Calculate the minimum range.
		CalculatedMinRange = ( BaseTransform.sizeDelta.x / 2 ) * minRange;
		
		// If the user wants to have an infinite max range then apply that, otherwise calculate the max range by the baseTransform's size delta.
		if( infiniteMaxRange )
			CalculatedMaxRange = Mathf.Infinity;
		else
			CalculatedMaxRange = ( BaseTransform.sizeDelta.x / 2 ) * maxRange;

		// If the render mode is not set to screen space overlay...
		if( ParentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
		{
			// Set IsWorldSpaceRadialMenu to true so that calculations can be made for world space.
			IsWorldSpaceRadialMenu = true;

			// If the base gameObject does not have a box collider, then add one.
			if( !BaseTransform.GetComponent<BoxCollider>() )
			{
				if( Application.isPlaying )
					BaseTransform.gameObject.AddComponent<BoxCollider>();
#if UNITY_EDITOR
				else
					UnityEditor.Undo.AddComponent( BaseTransform.gameObject, typeof( BoxCollider ) );
#endif
			}

			// Set the collider to trigger so that it doesn't have an effect on any physics.
			BaseTransform.GetComponent<BoxCollider>().isTrigger = true;

			// Apply the size of the box collider.
			BaseTransform.GetComponent<BoxCollider>().size = new Vector3( BaseTransform.sizeDelta.x * maxRange, BaseTransform.sizeDelta.y * maxRange, 0.001f );

			// Modify the position by the depth set by the user.
			BaseTransform.localPosition += new Vector3( 0, 0, depthPosition );
		}
		// Else the render mode is set to screen space.
		else
		{
			// Set IsWorldSpaceRadialMenu to false for calculations.
			IsWorldSpaceRadialMenu = false;

			// If there is a box collider on this object, then remove the component.
			if( BaseTransform.GetComponent<BoxCollider>() )
			{
				if( Application.isPlaying )
					DestroyImmediate( BaseTransform.GetComponent<BoxCollider>() );
#if UNITY_EDITOR
				else
					UnityEditor.Undo.DestroyObjectImmediate( BaseTransform.GetComponent<BoxCollider>() );
#endif
			}
		}

		// -------------------------- < CENTER TEXT POSITIONING > -------------------------- //
		// If the user wants to display the radial button name, and the text is assigned...
		if( displayButtonName && nameText != null )
		{
			// Configure text position and size.
			Vector2 textPosition = Vector2.zero;
			textPosition.x += BaseTransform.sizeDelta.x * ( ( nameTextHorizontalPosition - 50 ) / 100 );
			textPosition.y += BaseTransform.sizeDelta.y * ( ( nameTextVerticalPosition - 50 ) / 100 );
			nameText.rectTransform.sizeDelta = new Vector2( BaseTransform.sizeDelta.x * nameTextSize, BaseTransform.sizeDelta.x * nameTextSize ) * new Vector2( nameTextRatioX, nameTextRatioY );
			nameText.rectTransform.localPosition = textPosition;
			nameText.rectTransform.localRotation = Quaternion.identity;
		}

		// If the user wants to display the description of the button...
		if( displayButtonDescription && descriptionText != null )
		{
			// Configure text position and size.
			Vector2 textPosition = Vector2.zero;
			textPosition.x += BaseTransform.sizeDelta.x * ( ( descriptionTextHorizontalPosition - 50 ) / 100 );
			textPosition.y += BaseTransform.sizeDelta.y * ( ( descriptionTextVerticalPosition - 50 ) / 100 );
			descriptionText.rectTransform.sizeDelta = new Vector2( BaseTransform.sizeDelta.x * descriptionTextSize, BaseTransform.sizeDelta.x * descriptionTextSize ) * new Vector2( descriptionTextRatioX, descriptionTextRatioY );
			descriptionText.rectTransform.localPosition = textPosition;
			descriptionText.rectTransform.localRotation = Quaternion.identity;
		}

		// ------------------------- < RADIAL BUTTON POSITIONING > ------------------------- //
		// Configure the angle per button.
		float angle = GetAnglePerButton;

		// Convert the angle into radians. Here we are applying the angle as negative since we want the buttons to go clockwise.
		float angleInRadians = -angle * Mathf.Deg2Rad;
		
		// Configure the center angle offset according to the users options.
		float centerAngleOffset = 0.0f;

		// If the overall angle is exactly 360 degrees...
		if( overallAngle == 360.0f )
		{
			// Switch for the angle offset setting.
			switch( angleOffset )
			{
				// If it's default or off center, then just leave center angle offset at 0.
				default:
				case AngleOffset.OffCenter:
				break;
				// Else if the buttons are supposed to be centered, get half the angle per button.
				case AngleOffset.Centered:
				{
					centerAngleOffset = GetAnglePerButton / 2;
				}
				break;
				// Else if the buttons are only supposed to be centered on the even number of buttons...
				case AngleOffset.OnlyEven:
				{
					// If the list is divisible by 2, then configure half the angle per button.
					if( UltimateRadialButtonList.Count % 2 == 0 )
						centerAngleOffset = GetAnglePerButton / 2;
				}
				break;
				// Else if the button are only centered on odd number of buttons...
				case AngleOffset.OnlyOdd:
				{
					// Then if the list is NOT divisible by 2, configure for half the angle per button.
					if( UltimateRadialButtonList.Count % 2 != 0 )
						centerAngleOffset = GetAnglePerButton / 2;
				}
				break;
			}
		}
		
		// Configure how much to offset the rotation of the button by.
		float rotationOffset = -startingAngle + centerAngleOffset - ( GetAnglePerButton / 2 );

		// Configure the starting rotation based on the users startingAngle setting.
		float startingRotation = ( 90 * Mathf.Deg2Rad ) + ( -startingAngle * Mathf.Deg2Rad );

		// If the overall angle is less than a full circle, the rotation offset and starting rotation need to be adjusted...
		if( overallAngle < 360.0f )
		{
			// Change the rotation offset to using the center angle.
			rotationOffset = -centerAngle + ( overallAngle / 2 ) - ( GetAnglePerButton / 2 );

			// Change the starting rotation to using the center angle minus the overall angle of the menu divided by two.
			startingRotation = ( 90 - ( centerAngle - ( overallAngle / 2 ) ) ) * Mathf.Deg2Rad;
		}

		// Store the buttons size.
		Vector2 buttonImageSize = ( BaseTransform.sizeDelta * menuButtonSize );

		// If the normal sprite is assigned then multiply the button size by the ratio of the sprite.
		if( normalSprite != null )
			buttonImageSize *= GetImageAspectRatio( normalSprite );

		// Configure the button radius. ( half of the base size * by the radius ) minus ( half of the button's Y size ).
		float buttonRadius = ( ( BaseTransform.sizeDelta.x / 2 ) * radialMenuButtonRadius ) - ( buttonImageSize.y / 2 );

		// Loop through all of the radial buttons.
		for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
		{
			UltimateRadialButtonList[ i ].buttonIndex = i;

			// If the radial image is null then try to find it.
			if( UltimateRadialButtonList[ i ].radialImage == null )
				UltimateRadialButtonList[ i ].radialImage = UltimateRadialButtonList[ i ].buttonTransform.GetComponent<Image>();

			// Apply the size to the button transform.
			UltimateRadialButtonList[ i ].buttonTransform.sizeDelta = buttonImageSize;

			// Configure a new position for the button.
			Vector3 normalPosition = Vector3.zero;

			// This code may seem like a bunch of voodoo magic, ( but that's what math is, am I right!? ) but essentially it is just adding together all of the angle options that were set and multiplying it by the button radius.
			normalPosition.x += ( Mathf.Cos( ( angleInRadians * i ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * buttonRadius );
			normalPosition.y += ( Mathf.Sin( ( angleInRadians * i ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * buttonRadius );

			// Apply the new position to the transform, as well as a default scale of one.
			UltimateRadialButtonList[ i ].buttonTransform.localPosition = normalPosition;
			UltimateRadialButtonList[ i ].buttonTransform.localScale = scaleTransform && UltimateRadialButtonList[ i ].buttonDisabled ? Vector3.one * disabledScaleModifier : Vector3.one;

			// If the user wants to scale the transform when interacted with...
			if( scaleTransform )
			{
				// Store the new position as the normal position to return to.
				UltimateRadialButtonList[ i ].normalPosition = normalPosition;

				// If the highlighted position modifier is assigned, then calculate the position.
				if( positionModifier != 0 )
					UltimateRadialButtonList[ i ].highlightedPosition = normalPosition + ( normalPosition.normalized * ( buttonRadius * positionModifier ) );
				// Else assign the normal position.
				else
					UltimateRadialButtonList[ i ].highlightedPosition = normalPosition;

				// If the pressed position modifier is assigned, then calculate the position.
				if( pressedPositionModifier != 0 )
					UltimateRadialButtonList[ i ].pressedPosition = normalPosition + ( normalPosition.normalized * ( buttonRadius * pressedPositionModifier ) );
				// Else assign the normal position.
				else
					UltimateRadialButtonList[ i ].pressedPosition = normalPosition;

				// If the selected position modifier is assigned, then calculate the position.
				if( selectedPositionModifier != 0 )
					UltimateRadialButtonList[ i ].selectedPosition = normalPosition + ( normalPosition.normalized * ( buttonRadius * selectedPositionModifier ) );
				// Else assign the normal position.
				else
					UltimateRadialButtonList[ i ].selectedPosition = normalPosition;

				// If the disabled position modifier is assigned...
				if( disabledPositionModifier != 0 )
				{
					// Calculate the disabled position.
					UltimateRadialButtonList[ i ].disabledPosition = normalPosition + ( normalPosition.normalized * ( buttonRadius * disabledPositionModifier ) );

					// If this button is currently disabled, then apply the position.
					if( UltimateRadialButtonList[ i ].buttonDisabled )
						UltimateRadialButtonList[ i ].buttonTransform.localPosition = UltimateRadialButtonList[ i ].disabledPosition;
				}
				// Else assign the normal position.
				else
					UltimateRadialButtonList[ i ].disabledPosition = normalPosition;
			}

			// Store the angle of the radial button. This starts at 90 degrees to start the menu straight up. After that just add/subtract the angle information.
			UltimateRadialButtonList[ i ].angle = 90 + -startingAngle + centerAngleOffset + ( -angle * i ) - ( angle / 2 );

			// If the overall angle is less than a full 360 degrees, then the calculated angle needs to use the center angle to figure the actual starting angle to go off of.
			if( overallAngle < 360.0f )
				UltimateRadialButtonList[ i ].angle = 90 + ( -centerAngle + ( overallAngle / 2 ) ) + centerAngleOffset + ( -angle * i ) - ( angle / 2 );
			
			// Configure the angle range for calculations.
			UltimateRadialButtonList[ i ].angleRange = ( angle / 2 ) * buttonInputAngle;

			// If the value is less than -360, then increase by 360 to try to get a positive number. This is needed for the Starting Angle variable if the user sets it to a high value.
			if( UltimateRadialButtonList[ i ].angle <= -360 )
				UltimateRadialButtonList[ i ].angle += 360;

			// If the calculated angle is negative, then add 360 to try and make the value more readable and easy to work with.
			if( UltimateRadialButtonList[ i ].angle < 0 )
				UltimateRadialButtonList[ i ].angle += 360;
			
			// Configure the new rotation to apply to the button.
			Vector3 newRotation = Vector3.zero;

			// If the user wants to follow the orbital rotation of the menu, then calculate the rotation plus the rotation offset.
			if( followOrbitalRotation )
				newRotation = new Vector3( 0, 0, ( -angle * i ) + rotationOffset );

			// Apply the rotation to the button transform.
			UltimateRadialButtonList[ i ].buttonTransform.localRotation = Quaternion.Euler( newRotation );

			// -------------------------- < ICON POSITIONING > -------------------------- //
			if( useButtonIcon && UltimateRadialButtonList[ i ].icon != null )
			{
				// Store the positioning information so that it can modified if need be.
				float horizontalPos = iconHorizontalPosition;
				float verticalPos = iconVerticalPosition;
				float sizeMod = iconSize;
				float rotationMod = iconRotation;

				// If the user wants to use this icon with unique positioning...
				if( UltimateRadialButtonList[ i ].useIconUnique )
				{
					// Modify the positioning information with the unique information.
					horizontalPos = UltimateRadialButtonList[ i ].iconHorizontalPosition;
					verticalPos = UltimateRadialButtonList[ i ].iconVerticalPosition;
					sizeMod = UltimateRadialButtonList[ i ].iconSize;
					rotationMod = UltimateRadialButtonList[ i ].iconRotation;
				}

				// Configure the position for the icon.
				Vector2 iconPosition = Vector3.zero;
				iconPosition.x += ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x * ( horizontalPos / 100 ) ) - ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x / 2 );
				iconPosition.y += ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y * ( verticalPos / 100 ) ) - ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y / 2 );

				// Apply the size and position to the icon.
				UltimateRadialButtonList[ i ].icon.rectTransform.sizeDelta = new Vector2( BaseTransform.sizeDelta.x * sizeMod, BaseTransform.sizeDelta.x * sizeMod ) * ( UltimateRadialButtonList[ i ].icon.sprite == null ? Vector2.one : GetImageAspectRatio( UltimateRadialButtonList[ i ].icon.sprite ) );
				UltimateRadialButtonList[ i ].icon.rectTransform.localPosition = iconPosition;

				// If the transform is unassigned, then assign it.
				if( UltimateRadialButtonList[ i ].iconTransform == null )
					UltimateRadialButtonList[ i ].iconTransform = UltimateRadialButtonList[ i ].icon.rectTransform;
				
				// If the user wants to use local rotation then increase the rotation mod by the current button's rotation.
				if( iconLocalRotation )
				{
					// Store the image rotation.
					float imageRotation = UltimateRadialButtonList[ i ].radialImage.rectTransform.localRotation.eulerAngles.z;

					// If the rotation is less than zero then add 360 to get a positive number.
					if( imageRotation < 0 )
						imageRotation += 360;
					
					// If the rotation is more than 90 degrees and less than 270...
					if( imageRotation > 90 && imageRotation < 270 )
					{
						// If the user has SmartRotation enabled and the image rotation is less than 180 degrees, then increase the rotation by 180 to flip the icon.
						if( iconSmartRotation && imageRotation < 180 )
							rotationMod += 180;
					}
					// Else the rotation is the other direction...
					else
					{
						// If the user has SmartRotation enabled and the rotation is less than 270 but greater than 0, flip the icon.
						if( iconSmartRotation && imageRotation < 270 && imageRotation > 0 )
							rotationMod += 180;
					}
				}
				// Else the user wants world space rotation so store the rotation modifier as the buttons rotation.
				else
					rotationMod = -UltimateRadialButtonList[ i ].buttonTransform.localRotation.eulerAngles.z + ( UltimateRadialButtonList[ i ].useIconUnique ? UltimateRadialButtonList[ i ].iconRotation : -iconRotation );

				// Store the normal scale.
				UltimateRadialButtonList[ i ].iconNormalScale = UltimateRadialButtonList[ i ].invertScaleY ? new Vector3( 1, -1, 1 ) : new Vector3( 1, 1, 1 );
				UltimateRadialButtonList[ i ].iconTransform.localScale = UltimateRadialButtonList[ i ].buttonDisabled ? UltimateRadialButtonList[ i ].iconNormalScale * iconDisabledScaleModifier : UltimateRadialButtonList[ i ].iconNormalScale;

				// Apply the rotation to the icon.
				UltimateRadialButtonList[ i ].icon.rectTransform.localRotation = Quaternion.Euler( new Vector3( 0, 0, rotationMod ) );
			}

			// -------------------------- < TEXT POSITIONING > -------------------------- //
			if( UltimateRadialButtonList[ i ].text != null )
			{
				// Apply the size to the text transform.
				UltimateRadialButtonList[ i ].text.rectTransform.sizeDelta = new Vector2( BaseTransform.sizeDelta.x * textSize, BaseTransform.sizeDelta.x * textSize ) * new Vector2( textAreaRatioX, textAreaRatioY );

				// If the user wants to position the text in local position to the button...
				if( textLocalPosition )
				{
					// Store a vector2 for the text position.
					Vector2 textPosition = Vector2.zero;

					// Since the user might want to increase the are at which they can position the text, this Vector2 will be a larger area to position.
					Vector2 modifiedRefSizeForText = new Vector2( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x, UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y ) * 1.25f;

					if( relativeToIcon && UltimateRadialButtonList[ i ].iconTransform != null )
						modifiedRefSizeForText = new Vector2( UltimateRadialButtonList[ i ].iconTransform.sizeDelta.x, UltimateRadialButtonList[ i ].iconTransform.sizeDelta.y ) * 1.25f;

					textPosition.x += ( modifiedRefSizeForText.x * ( textHorizontalPosition / 100 ) ) - ( modifiedRefSizeForText.x / 2 );
					textPosition.y += ( modifiedRefSizeForText.y * ( textVerticalPosition / 100 ) ) - ( modifiedRefSizeForText.y / 2 );
					
					// Apply the local position to the text.
					UltimateRadialButtonList[ i ].text.rectTransform.localPosition = textPosition;

					// If the user wants the text to have a local rotation with the button...
					if( textLocalRotation )
					{
						// Store the image rotation.
						float imageRotation = UltimateRadialButtonList[ i ].radialImage.rectTransform.localRotation.eulerAngles.z;

						// If the rotation is less than zero then add 360 to get a positive number.
						if( imageRotation < 0 )
							imageRotation += 360;

						// If the rotation is more than 90 degrees and less than 270, then increase the rotation by 180 to flip the text so it is readable.
						if( imageRotation > 90 && imageRotation < 270 )
							UltimateRadialButtonList[ i ].text.rectTransform.localRotation = Quaternion.Euler( 0, 0, 180 );
						// Else just set the local rotation to zero.
						else
							UltimateRadialButtonList[ i ].text.rectTransform.localRotation = Quaternion.identity;
					}
					// Else the user does not want the rotation to be local, so apply the inverse rotation of the button.
					else
						UltimateRadialButtonList[ i ].text.rectTransform.localRotation = Quaternion.Euler( new Vector3( 0, 0, -UltimateRadialButtonList[ i ].buttonTransform.localRotation.eulerAngles.z ) );
				}
				// Else the text positioning is set to global...
				else
				{
					// Store the position of this button in world space. Everything will be calculated off of this.
					Vector3 buttonWorldPosition = Vector3.zero;
					
					// Modify that position by the options of the user.
					Vector2 modifiedRefSizeForText = ( new Vector2( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x, UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y ) ) * 1.25f;

					if( relativeToIcon && UltimateRadialButtonList[ i ].iconTransform != null )
						modifiedRefSizeForText = ( new Vector2( UltimateRadialButtonList[ i ].iconTransform.sizeDelta.x, UltimateRadialButtonList[ i ].iconTransform.sizeDelta.y ) ) * 1.25f;

					buttonWorldPosition.x += ( modifiedRefSizeForText.x * ( textHorizontalPosition / 100 ) ) - ( modifiedRefSizeForText.x / 2 );
					buttonWorldPosition.y += ( modifiedRefSizeForText.y * ( textVerticalPosition / 100 ) ) - ( modifiedRefSizeForText.y / 2 );

					// If this canvas is in world space...
					if( IsWorldSpaceRadialMenu )
					{
						// Convert the position from local to world space.
						buttonWorldPosition = BaseTransform.transform.TransformPoint( buttonWorldPosition );

						// Modify the calculated position by subtracting the base transforms position.
						buttonWorldPosition -= BaseTransform.position;
					}

					if( relativeToIcon && UltimateRadialButtonList[ i ].iconTransform != null )
						UltimateRadialButtonList[ i ].text.rectTransform.position = UltimateRadialButtonList[ i ].iconTransform.position + buttonWorldPosition;
					// Apply the position of the text to being the button position plus the calculated world position.
					else
						UltimateRadialButtonList[ i ].text.rectTransform.position = UltimateRadialButtonList[ i ].buttonTransform.position + buttonWorldPosition;

					// Set the rotation of the text to being the inverse rotation of the button.
					UltimateRadialButtonList[ i ].text.rectTransform.localRotation = Quaternion.Euler( 0, 0, -UltimateRadialButtonList[ i ].buttonTransform.localRotation.eulerAngles.z );
				}
			}

			// If the game is running and the user wants to scale transform, and if this button is currently selected then apply the position and scale.
			if( Application.isPlaying && scaleTransform && UltimateRadialButtonList[ i ].Selected )
			{
				UltimateRadialButtonList[ i ].buttonTransform.localScale = Vector3.one * selectedScaleModifier;
				UltimateRadialButtonList[ i ].buttonTransform.localPosition = UltimateRadialButtonList[ i ].selectedPosition;
			}
		}

		// Reset the current button index since the menu has likely shifted in some way. This will allow the ProcessInput function to recalculate where the buttons are.
		CurrentButtonIndex = -1;
		
		// Apply the starting scale.
		BaseTransform.localScale = menuToggleOverTime && menuToggleScale && !IsEnabled && Application.isPlaying ? Vector3.zero : Vector3.one;

		// If the user wants a pointer to be used, and the image is assigned...
		if( usePointer && pointerImage != null )
		{
			// If the pointer transform is not assigned yet, assign it.
			if( pointerTransform == null )
				pointerTransform = pointerImage.rectTransform;
			
			// If the pivot is not 0.5, then set the pivot.
			if( pointerTransform.pivot != Vector2.one / 2 )
				pointerTransform.pivot = Vector2.one / 2;

			// Apply the size and position to the pointer transform.
			pointerTransform.sizeDelta = Vector2.one * ( BaseTransform.sizeDelta.x * pointerSize );
			pointerTransform.localPosition = Vector3.zero;

			// If the game is not running, then apply the rotation to look at the first button plus the rotation offset that the user has set.
			if( !Application.isPlaying && UltimateRadialButtonList.Count > 0 )
				pointerTransform.localRotation = Quaternion.Euler( 0, 0, UltimateRadialButtonList[ 0 ].angle - pointerRotationOffset );

			// Calculate the pointer target speed.
			pointerTargetSpeed = 1.0f / pointerTargetTime;
		}

		// Inform any subscribers that the update positioning function has run.
		OnUpdatePositioning?.Invoke();
	}

	/// <summary>
	/// Register the provided information to the Ultimate Radial Menu.
	/// </summary>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no value is provided, the information will be registered to the first available button.</param>
	public void RegisterButton ( Action ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// Find the actual index of the radial button depending on what the user passed as the buttonIndex parameter.
		FindRadialButtonIndex( ref buttonIndex );

		// Register the button information.
		UltimateRadialButtonList[ buttonIndex ].RegisterButtonInfo( buttonInfo );

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		UltimateRadialButtonList[ buttonIndex ].AddCallback( ButtonCallback );
	}

	/// <summary>
	/// Register the provided information to the Ultimate Radial Menu.
	/// </summary>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no value is provided, the information will be registered to the first available button.</param>
	public void RegisterButton ( Action<int> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// Find the actual index of the radial button depending on what the user passed as the buttonIndex parameter.
		FindRadialButtonIndex( ref buttonIndex );

		// Register the button information.
		UltimateRadialButtonList[ buttonIndex ].RegisterButtonInfo( buttonInfo );

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		UltimateRadialButtonList[ buttonIndex ].AddCallback( ButtonCallback );
	}

	/// <summary>
	/// Register the provided information to the Ultimate Radial Menu.
	/// </summary>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no value is provided, the information will be registered to the first available button.</param>
	public void RegisterButton ( Action<string> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// Find the actual index of the radial button depending on what the user passed as the buttonIndex parameter.
		FindRadialButtonIndex( ref buttonIndex );

		// Register the button information.
		UltimateRadialButtonList[ buttonIndex ].RegisterButtonInfo( buttonInfo );

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		UltimateRadialButtonList[ buttonIndex ].AddCallback( ButtonCallback );
	}

	/// <summary>
	/// Enables the Ultimate Radial Menu so that it can be interacted with.
	/// </summary>
	public void Enable ()
	{
		// If the radial menu is already active, then return.
		if( IsEnabled )
			return;

		// If this gameobject is not active in the scene...
		if( !gameObject.activeInHierarchy )
		{
			// Set this object to active so it can run.
			gameObject.SetActive( true );

			// Call the Awake() and Start() functions to store references.
			Awake();
			Start();
		}

		// Set the state to active.
		IsEnabled = true;

		// If the user wants to toggle the menu over time...
		if( menuToggleOverTime )
		{
			// Set interactable to false so that the menu cannot be interacted with until it is fully toggled.
			Interactable = false;

			// If the fade in duration is zero...
			if( toggleInDuration <= 0.0f )
			{
				// Set the canvas group alpha to full.
				canvasGroup.alpha = 1.0f;

				// If the user wants to scale the menu, then apply the full scale instantly.
				if( menuToggleScale )
					BaseTransform.localScale = Vector3.one;

				// Set interactable to true right away since there won't be a smooth transition.
				Interactable = true;

				// If the fade out duration is set and the menu is currently fading out...
				if( toggleOutDuration > 0.0f && toggleOut )
				{
					// Set toggleFadeOut to false so it won't fade, and reset the lerp value.
					toggleOut = false;
					toggleLerpValue = 0.0f;
				}
			}
			// Else the user either does want to fade over time, and the fade in duration is set...
			else
			{
				// If the user does not want to fade the canvas group, then just apply fully alpha.
				if( !menuToggleAlpha )
					canvasGroup.alpha = 1.0f;

				// Set toggleIn to true so it will run, and store the current alpha and scale of the menu.
				toggleIn = true;
				toggleAlphaValue = canvasGroup.alpha;
				toggleScaleValue = BaseTransform.localScale.x;

				// Calculate the fade in speed and reset the lerp value.
				toggleInSpeed = 1.0f / toggleInDuration;
				toggleLerpValue = 0.0f;

				// If the menu is currently fading out, set toggleOut to false so that it won't continue to fade out.
				if( toggleOut )
					toggleOut = false;
			}
		}
		// Else just set the canvas group so the player can see the menu and set Interactable to true.
		else
		{
			canvasGroup.alpha = 1.0f;
			Interactable = true;
		}

		// Notify any subscribers that the radial menu is now enabled.
		OnMenuEnabled?.Invoke();
		OnRadialMenuEnabled?.Invoke();
	}

	/// <summary>
	/// Disables the Ultimate Radial Menu so that it can not be interacted with.
	/// </summary>
	public void Disable ()
	{
		// If the radial menu is already disabled, then return.
		if( !IsEnabled )
			return;

		// Set the state to inactive.
		IsEnabled = false;

		// Reset the radial menu so that it is ready for the next time it is enabled.
		ResetRadialMenu();

		// If the user wants to toggle the visual state of the menu...
		if( menuToggleOverTime )
		{
			// If the user doesn't want to toggle the menu over time, or the fade out duration is not assigned...
			if( toggleOutDuration <= 0.0f )
			{
				// Set the canvas group alpha to zero.
				canvasGroup.alpha = 0.0f;

				// If the user wants to scale the menu, then apply the zero scale instantly.
				if( menuToggleScale )
					BaseTransform.localScale = Vector3.zero;

				// If the fade in duration is set, and the toggle is currently fading in...
				if( toggleInDuration > 0.0f && toggleIn )
				{
					// Set the fade in controller to false and reset the lerp value.
					toggleIn = false;
					toggleLerpValue = 0.0f;
				}
			}
			// Else the user doesn't want to fade over time, and the fade out duration is set...
			else
			{
				// Set toggleOut to true so it will run, and store the current alpha and scale.
				toggleOut = true;
				toggleAlphaValue = canvasGroup.alpha;
				toggleScaleValue = BaseTransform.localScale.x;

				// Calculate the speed and reset the lerp value.
				toggleOutSpeed = 1.0f / toggleOutDuration;
				toggleLerpValue = 0.0f;

				// If the menu is currently fading in, set it to false so it won't run.
				if( toggleIn )
					toggleIn = false;
			}
		}
		// Else the user doesn't have any menu toggle options set, so set the canvas group alpha to zero and Interactable to false.
		else
		{
			canvasGroup.alpha = 0.0f;
			Interactable = false;
		}

		// If the user has the pointer set...
		if( usePointer && pointerImage != null )
		{
			// Reset all the variables.
			PointerActive = false;
			pointerFadeIn = false;
			pointerFadeOut = false;
			pointerImage.color = pointerNormalColor;
			pointerLerpValue = 0.0f;
		}

		// Set the input in range to false, just so that if the input manager was disabled, then other scripts can know that the input is no longer in range.
		inputInRangeLastFrame = InputInRange = false;

		// Notify any subscribers that the radial menu is disabled.
		OnMenuDisabled?.Invoke();
		OnRadialMenuDisabled?.Invoke();
	}

	/// <summary>
	/// Disables the Ultimate Radial Menu immediately so that it can not be interacted with.
	/// </summary>
	public void DisableImmediate ()
	{
		// Set the state to inactive.
		IsEnabled = false;

		// Reset the radial menu so that it is ready.
		ResetRadialMenu();
		
		// If the user wants to transition the toggle over time...
		if( menuToggleOverTime )
		{
			// Set the canvas group alpha to zero.
			canvasGroup.alpha = 0.0f;

			// If the user wants to scale the menu, then apply the zero scale instantly.
			if( menuToggleScale )
				BaseTransform.localScale = Vector3.zero;

			// If the fade in duration is set, and the toggle is currently fading in...
			if( toggleInDuration > 0.0f && toggleIn )
			{
				// Set the fade in controller to false and reset the lerp value.
				toggleIn = false;
				toggleLerpValue = 0.0f;
			}
		}
		// Else the user doesn't have any menu toggle options set, so set the canvas group alpha to zero.
		else
			canvasGroup.alpha = 0.0f;

		// Notify any subscribers that the radial menu has been disabled.
		OnMenuDisabled?.Invoke();
		OnRadialMenuDisabled?.Invoke();
	}

	/// <summary>
	/// Creates an empty radial button at the end of the radial menu.
	/// </summary>
	public void CreateEmptyButton ()
	{
		// Call the created function with the index of 1000 so it's out of range.
		CreateRadialButtonAtIndex( 1000 );
	}

	/// <summary>
	/// Removes all of the radial buttons from the radial menu. If a value is provided for the buttonCount parameter, the radial menu will ensure that it does not remove more than that count of buttons.
	/// </summary>
	/// <param name="buttonCount">[OPTIONAL] Determines how many buttons the radial menu will leave on the menu.</param>
	public void ClearMenu ( int buttonCount = 0 )
	{
		// If the user has a style assigned, and the button count is less than the minimum button count for the style, set the button count to the minimum.
		if( radialMenuStyle != null && buttonCount < radialMenuStyle.minButtonCount )
			buttonCount = radialMenuStyle.minButtonCount;

		// Loop through the radial button list...
		for( int i = UltimateRadialButtonList.Count - 1; i >= 0; i-- )
		{
			// If this button is currently disabled, enable it before clearing the information so that it will be usable.
			if( UltimateRadialButtonList[ i ].buttonDisabled )
				UltimateRadialButtonList[ i ].OnEnable();

			// Clear button information.
			UltimateRadialButtonList[ i ].ClearButtonInformation();

			// If the index is greater than the minimum button count, then send this button index to the pool.
			if( i >= buttonCount )
				SendRadialButtonToPool( i );
		}

		// Since the button count has changed, call the RadialMenuButtonCountModified function.
		RadialMenuButtonCountModified();
	}

	/// <summary>
	/// Removes the radial button at the targeted index.
	/// </summary>
	/// <param name="buttonIndex">The index to remove the radial button at.</param>
	public void RemoveButton ( int buttonIndex )
	{
		// If the list has no buttons, then just return because there is nothing to remove.
		if( UltimateRadialButtonList.Count <= 0 )
			return;

		// If the button index is greater than the count, then just set it to the max index.
		if( buttonIndex > UltimateRadialButtonList.Count )
			buttonIndex = UltimateRadialButtonList.Count - 1;
		
		// Clear the button information so that any button information will be notified that this button has been destroyed.
		UltimateRadialButtonList[ buttonIndex ].ClearButtonInformation();
		
		// If the user does not have a style assigned OR the count is within the minimum button count of the style...
		if( radialMenuStyle == null || UltimateRadialButtonList.Count > radialMenuStyle.minButtonCount )
		{
			// Send this button to the pool.
			SendRadialButtonToPool( buttonIndex );
			
			// Since the button count has changed, call the RadialMenuButtonCountModified function.
			RadialMenuButtonCountModified();
		}
	}
	
	/// <summary>
	/// Clears the registered button information.
	/// </summary>
	public void ClearButtonInformations ()
	{
		// Loop through the list of radial buttons and clear the button information.
		for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
		{
			// If this button is currently disabled, enable it before clearing the information so that it will be usable.
			if( UltimateRadialButtonList[ i ].buttonDisabled )
				UltimateRadialButtonList[ i ].OnEnable();

			// Clear button information.
			UltimateRadialButtonList[ i ].ClearButtonInformation();
		}

		// Reset the radial menu.
		ResetRadialMenu();
	}

	/// <summary>
	/// Updates the radial menu's position to the new position either in the world or on the screen.
	/// </summary>
	/// <param name="position">The new position either on the screen, or in the world.</param>
	/// <param name="local">[OPTIONAL] Determines whether or not to apply the provided position to local space or not. Defaults to world space.</param>
	public void SetPosition ( Vector3 position, bool local = false )
	{
		// If the base transform is assigned...
		if( BaseTransform != null )
		{
			// If this menu is world space...
			if( IsWorldSpaceRadialMenu )
			{
				// If the parent canvas is unassigned...
				if( ParentCanvas == null )
				{
					// Try to find the parent canvas.
					UpdateParentCanvas();

					// If the parent canvas is still null, then notify the user and return.
					if( ParentCanvas == null )
					{
						Debug.LogError( FormatDebug( "There is no parent canvas object, which means this object is not placed within a Canvas object in your scene", "Please make sure that the Ultimate Radial Menu is placed within a canvas", gameObject.name ) );
						return;
					}
				}

				// Set the position of the parent canvas to the position that the user sent.
				ParentCanvas.GetComponent<RectTransform>().position = position;
			}
			// Else apply the position to the base transform.
			else
			{
				if( local )
					BaseTransform.localPosition = position;
				else
					BaseTransform.position = position;
			}
		}
	}

	/// <summary>
	/// Resets the position of the radial menu back to it's original position.
	/// </summary>
	public void ResetPosition ()
	{
		if( BaseTransform != null )
			BaseTransform.position = defaultPosition;
	}

	/// <summary>
	/// Sets the parent of the Canvas to the provided Transform. This is only suitable for World Space radial menus.
	/// </summary>
	/// <param name="parent">The new Transform to make the Canvas a child of.</param>
	/// <param name="localPosition">The local position of the radial menu.</param>
	/// <param name="localRotation">The local rotation of the radial menu.</param>
	public void SetParent ( Transform parent, Vector3 localPosition, Quaternion localRotation )
	{
		// If the parent canvas is unassigned...
		if( ParentCanvas == null )
		{
			// Try to find the parent canvas.
			UpdateParentCanvas();

			// If the parent canvas is still null, then notify the user and return.
			if( ParentCanvas == null )
			{
				Debug.LogError( FormatDebug( "There is no parent canvas object, which means this object is not placed within a Canvas object in your scene", "Please make sure that the Ultimate Radial Menu is placed within a canvas", gameObject.name ) );
				return;
			}
		}

		// Set the parent transform of the canvas.
		ParentCanvas.transform.SetParent( parent );

		// Set the local position and rotation.
		ParentCanvas.GetComponent<RectTransform>().localRotation = localRotation;
		ParentCanvas.GetComponent<RectTransform>().localPosition = localPosition;
	}

	/// <summary>
	/// Forces the pointer to be in the enabled state. It's important to note that if the radial menu is still interactable, the pointer will not remain enabled unless the input stays on the menu.
	/// </summary>
	public void EnablePointer ()
	{
		// Set the last known input as false.
		inputInRangeLastFrame = false;

		// Set the current input in range as true.
		InputInRange = true;

		// Process the pointer here. It's important that the function is called specifically here because inputInRangeLastFrame will be stored after the OnProcessInput callback, which will cause unwanted behavior before ProcessPointer is called again.
		ProcessPointer();
	}
	// ------------------------------------------------ < END PUBLIC FUNCTIONS FOR THE USER > ------------------------------------------------ //
	
	/// <summary>
	/// [INTERNAL] Formats and sends detailed information to the user.
	/// </summary>
	static string FormatDebug ( string error, string solution, string objectName )
	{
		return "<b>Ultimate Radial Menu</b>\n" +
			"<color=red><b>×</b></color> <i><b>Error:</b></i> " + error + ".\n" +
			"<color=green><b>√</b></color> <i><b>Solution:</b></i> " + solution + ".\n" +
			"<color=cyan><b>∙</b></color> <i><b>Object:</b></i> " + objectName + "\n";
	}

	/// <summary>
	/// Confirms the existence of the radial menu that has been registered with the targeted name.
	/// </summary>
	/// <param name="radialMenuName">The string name that the radial menu has been registered with.</param>
	static bool ConfirmUltimateRadialMenu ( string radialMenuName )
	{
		// If the static radial menu dictionary does not contain the targeted radial menu key, then inform the user and return false.
		if( !UltimateRadialMenus.ContainsKey( radialMenuName ) )
		{
			Debug.LogWarning( FormatDebug( $"There is no Ultimate Radial Menu registered with the name: {radialMenuName} in the scene", $"Please exit play mode and ensure that your Ultimate Radial Menu has the name: {radialMenuName} registered in the <b>Script Reference</b> section", "Unknown (user script)" ) );
			return false;
		}
		return true;
	}

	// ----------------------------------------------------- < PUBLIC STATIC FUNCTIONS > ----------------------------------------------------- //
	/// <summary>
	/// Returns the radial menu that has been registered with the targeted radial menu name.
	/// </summary>
	/// <param name="radialMenuName">The string name that the radial menu has been registered with.</param>
	public static UltimateRadialMenu ReturnComponent ( string radialMenuName )
	{
		// If the radial menu does not exist then return null.
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return null;

		return UltimateRadialMenus[ radialMenuName ];
	}

	/// <summary>
	/// Registered the provided information to the targeted Ultimate Radial Menu.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no value is provided, the information will be registered to the first available button.</param>
	public static void RegisterButton ( string radialMenuName, Action ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return.
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RegisterButton( ButtonCallback, buttonInfo, buttonIndex );
	}

	/// <summary>
	/// Registered the provided information to the targeted Ultimate Radial Menu.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no value is provided, the information will be registered to the first available button.</param>
	public static void RegisterButton ( string radialMenuName, Action<int> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return.
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RegisterButton( ButtonCallback, buttonInfo, buttonIndex );
	}

	/// <summary>
	/// Registered the provided information to the targeted Ultimate Radial Menu.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no value is provided, the information will be registered to the first available button.</param>
	public static void RegisterButton ( string radialMenuName, Action<string> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return.
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RegisterButton( ButtonCallback, buttonInfo, buttonIndex );
	}
	
	/// <summary>
	/// Enables the targeted Ultimate Radial Menu so that it can be interacted with.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void Enable ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].Enable();
	}

	/// <summary>
	/// Disables the targeted Ultimate Radial Menu so that it can not be interacted with.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void Disable ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].Disable();
	}

	/// <summary>
	/// Disables the Ultimate Radial Menu immediately so that it can not be interacted with.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void DisableImmediate ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].DisableImmediate();
	}

	/// <summary>
	/// Creates an empty radial button at the end of the targeted radial menu.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void CreateEmptyButton ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].CreateEmptyButton();
	}

	/// <summary>
	/// Deletes all of the radial menu buttons from the radial menu.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="buttonCount">[OPTIONAL] Determines how many buttons the radial menu will leave on the menu.</param>
	public static void ClearMenu ( string radialMenuName, int buttonCount = 0 )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].ClearMenu( buttonCount );
	}

	/// <summary>
	/// Removes the radial button at the targeted index.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="buttonIndex">The index to remove the radial button at.</param>
	public static void RemoveButton ( string radialMenuName, int buttonIndex )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RemoveButton( buttonIndex );
	}

	/// <summary>
	/// Clears the registered button information.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void ClearButtonInformations ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].ClearButtonInformations();
	}

	/// <summary>
	/// Updates the targeted radial menu's position to the new position on the screen.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="position">The new position on the screen or in the world.</param>
	/// <param name="local">[OPTIONAL] Determines whether or not to apply the provided position to local space or not. Defaults to world space.</param>
	public static void SetPosition ( string radialMenuName, Vector3 position, bool local = false )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		// Call the SetPosition function on the targeted radial menu.
		UltimateRadialMenus[ radialMenuName ].SetPosition( position, local );
	}

	/// <summary>
	/// Resets the position of the radial menu back to it's original position.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void ResetPosition ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].ResetPosition();
	}

	/// <summary>
	/// Sets the parent of the Canvas to the provided Transform. This is only suitable for World Space radial menus.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="parent">The new Transform to make the Canvas a child of.</param>
	/// <param name="localPosition">The local position of the radial menu.</param>
	/// <param name="localRotation">The local rotation of the radial menu.</param>
	public static void SetParent ( string radialMenuName, Transform parent, Vector3 localPosition, Quaternion localRotation )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].SetParent( parent, localPosition, localRotation );
	}
	// --------------------------------------------------- < END PUBLIC STATIC FUNCTIONS > --------------------------------------------------- //

	#region DEPRECIATED FUNCTIONS AND CALLBACKS
	[Obsolete( "Please use the IsEnabled property." )]
	public bool RadialMenuActive
	{
		get
		{
			return IsEnabled;
		}
	}

	[Obsolete( "Please use the RegisterButton function." )]
	public void RegisterToRadialMenu ( Action ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		FindRadialButtonIndex( ref buttonIndex );
		UltimateRadialButtonList[ buttonIndex ].RegisterButtonInfo( buttonInfo );
		UltimateRadialButtonList[ buttonIndex ].AddCallback( ButtonCallback );
	}

	[Obsolete( "Please use the RegisterButton function." )]
	public void RegisterToRadialMenu ( Action<int> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		FindRadialButtonIndex( ref buttonIndex );
		UltimateRadialButtonList[ buttonIndex ].RegisterButtonInfo( buttonInfo );
		UltimateRadialButtonList[ buttonIndex ].AddCallback( ButtonCallback );
	}

	[Obsolete( "Please use the RegisterButton function." )]
	public void RegisterToRadialMenu ( Action<string> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		FindRadialButtonIndex( ref buttonIndex );
		UltimateRadialButtonList[ buttonIndex ].RegisterButtonInfo( buttonInfo );
		UltimateRadialButtonList[ buttonIndex ].AddCallback( ButtonCallback );
	}

	[Obsolete( "Please use the Enable function." )]
	public void EnableRadialMenu ()
	{
		Enable();
	}
	
	[Obsolete( "Please use the Disable function." )]
	public void DisableRadialMenu ()
	{
		Disable();
	}
	
	[Obsolete( "Please use the DisableImmediate function." )]
	public void DisableRadialMenuImmediate ()
	{
		DisableImmediate();
	}

	[Obsolete( "Please use the CreateEmptyButton function." )]
	public void CreateEmptyRadialButton ()
	{
		CreateEmptyButton();
	}
	
	[Obsolete( "Please use the ClearMenu function." )]
	public void RemoveAllRadialButtons ( int buttonCount = 0 )
	{
		ClearMenu( buttonCount );
	}

	[Obsolete( "Please use the RemoveButton function." )]
	public void RemoveRadialButton ( int buttonIndex )
	{
		RemoveButton( buttonIndex );
	}

	[Obsolete( "Please use the ClearButtonInformations function." )]
	public void ClearRadialButtonInformations ()
	{
		ClearButtonInformations();
	}

	[Obsolete( "Please use the ReturnComponent function." )]
	public static UltimateRadialMenu GetUltimateRadialMenu ( string radialMenuName )
	{
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return null;

		return UltimateRadialMenus[ radialMenuName ];
	}

	[Obsolete( "Please use the RegisterButton function." )]
	public static void RegisterToRadialMenu ( string radialMenuName, Action ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RegisterButton( ButtonCallback, buttonInfo, buttonIndex );
	}

	[Obsolete( "Please use the RegisterButton function." )]
	public static void RegisterToRadialMenu ( string radialMenuName, Action<int> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RegisterButton( ButtonCallback, buttonInfo, buttonIndex );
	}

	[Obsolete( "Please use the RegisterButton function." )]
	public static void RegisterToRadialMenu ( string radialMenuName, Action<string> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RegisterButton( ButtonCallback, buttonInfo, buttonIndex );
	}
	
	[Obsolete( "Please use the Enable function." )]
	public static void EnableRadialMenu ( string radialMenuName )
	{
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].Enable();
	}
	
	[Obsolete( "Please use the Disable function." )]
	public static void DisableRadialMenu ( string radialMenuName )
	{
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].Disable();
	}

	[Obsolete( "Please use the DisableImmediate function." )]
	public static void DisableRadialMenuImmediate ( string radialMenuName )
	{
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].DisableImmediate();
	}
	
	[Obsolete( "Please use the CreateEmptyButton function." )]
	public static void CreateEmptyRadialButton ( string radialMenuName )
	{
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].CreateEmptyButton();
	}
	
	[Obsolete( "Please use the ClearMenu function." )]
	public static void RemoveAllRadialButtons ( string radialMenuName )
	{
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].ClearMenu();
	}
	
	[Obsolete( "Please use the RemoveButton function." )]
	public static void RemoveRadialButton ( string radialMenuName, int buttonIndex )
	{
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RemoveButton( buttonIndex );
	}
	
	[Obsolete( "Please use the ClearButtonInformations function." )]
	public static void ClearRadialButtonInformations ( string radialMenuName )
	{
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].ClearButtonInformations();
	}

	[Obsolete( "Please use the OnButtonEnter callback." )]
	public event Action<int> OnRadialButtonEnter;
	[Obsolete( "Please use the OnButtonExit callback." )]
	public event Action<int> OnRadialButtonExit;
	[Obsolete( "Please use the OnButtonInputDown callback." )]
	public event Action<int> OnRadialButtonInputDown;
	[Obsolete( "Please use the OnButtonInputUp callback." )]
	public event Action<int> OnRadialButtonInputUp;
	[Obsolete( "Please use the OnButtonInteract callback." )]
	public event Action<int> OnRadialButtonInteract;
	[Obsolete( "Please use the OnButtonSelected callback." )]
	public event Action<int> OnRadialButtonSelected;
	[Obsolete( "Please use the OnMenuLostFocus callback." )]
	public event Action OnRadialMenuLostFocus;
	[Obsolete( "Please use the OnMenuEnabled callback." )]
	public event Action OnRadialMenuEnabled;
	[Obsolete( "Please use the OnMenuDisabled callback." )]
	public event Action OnRadialMenuDisabled;
	[Obsolete( "Please use the OnButtonCountModified callback." )]
	public event Action<int> OnRadialMenuButtonCountModified;
	[Obsolete( "Please use the OnProcessInput callback." )]
	public event Action<Vector2, float, bool, bool> OnRadialMenuProcessInput;
	#endregion
}