using UnityEngine;
using UnityEngine.EventSystems;

public class ItemReceiverExample : MonoBehaviour, IDragHandler, IPointerUpHandler
{
	RectTransform myTransform;
	Vector3 originalPosition;

	public UltimateRadialButtonInfo newRadialButtonInfo;

	int itemCount = 0;
	
	public Sprite placeholderIcon;


	void Start ()
	{
		// Store this transform.
		myTransform = GetComponent<RectTransform>();

		// Store the starting position so that it can return to it after being released.
		originalPosition = myTransform.localPosition;
	}

	public void OnDrag ( PointerEventData eventData )
	{
		// When the user drags to pointer, move this transform with the pointer.
		myTransform.position = eventData.position;
	}

	public void OnPointerUp ( PointerEventData eventData )
	{
		// When the pointer is released, get the Ultimate Radial Menu's current button index.
		int index = UltimateRadialMenu.ReturnComponent( "ItemWheelExample" ).CurrentButtonIndex;

		// If the index is greater than zero ( meaning that the input was on the radial menu ), and this button information does not currently exist on the menu, then add this button info to the radial menu.
		if( index >= 0 && !newRadialButtonInfo.Registered && !UltimateRadialMenu.ReturnComponent( "ItemWheelExample" ).UltimateRadialButtonList[ index ].Registered )
			UltimateRadialMenu.RegisterButton( "ItemWheelExample", UseItem, newRadialButtonInfo, index );

		// Increase this items count.
		itemCount++;

		// If this button does exist on the radial menu, then update the text of the button.
		if( newRadialButtonInfo.Registered )
			newRadialButtonInfo.UpdateText( itemCount.ToString() );

		// Update this transform back to the original position.
		myTransform.localPosition = originalPosition;
	}

	void UseItem ()
	{
		// Decrease the item count.
		itemCount--;

		// Update the text of the button.
		newRadialButtonInfo.UpdateText( itemCount.ToString() );

		// If the count is less than 0, then the item is all used up...
		if( itemCount <= 0 )
		{
			// Temporarily store the associated radial button.
			UltimateRadialMenu.UltimateRadialButton radialButton = newRadialButtonInfo.radialButton;

			// Remove the button from the list.
			newRadialButtonInfo.RemoveInfoFromButton();

			// Reset the radial buttons text and icon to look default.
			radialButton.UpdateText( "Text" );
			radialButton.UpdateIcon( placeholderIcon );

		}
	}
}