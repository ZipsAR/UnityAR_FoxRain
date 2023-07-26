/* UltimateRadialButtonInfo.cs */
/* Written by Kaz Crowe */
using System;
using UnityEngine;

[Serializable]
public class UltimateRadialButtonInfo
{
	public UltimateRadialMenu.UltimateRadialButton radialButton;
	bool selected = false;
	/// <summary>
	/// Updates the selected state of this button info.
	/// </summary>
	public bool Selected
	{
		get
		{
			// Return the selected bool.
			return selected;
		}
		set
		{
			// Set selected to equal the value sent.
			selected = value;

			// If this info has not been assigned to a radial button, then return WITHOUT informing the user that the selected state cannot be applied.
			if( RadialButtonError_NoWarning )
				return;

			// If this info is selected, select the radial button.
			if( selected )
				radialButton.OnSelect();
			// Else deselect it.
			else
				radialButton.OnDeselect();
		}
	}
	public bool Registered
	{
		get
		{
			// If the button is assigned, then return true that this information is attached.
			if( radialButton != null && radialButton.radialMenu != null )
				return true;

			// Otherwise return false since there is no button attached to this information.
			return false;
		}
	}

	public string key;
	public int id;

	public string name;
	public string description;
	public Sprite icon;

	/// <summary>
	/// Returns the index that the radial menu button is assigned.
	/// </summary>
	public int GetButtonIndex
	{
		get
		{
			// If there is a button error, then just return 0.
			if( RadialButtonError )
				return 0;

			// Return the radial button's index.
			return radialButton.buttonIndex;
		}
	}
	
	/// <summary>
	/// Applies a new string to the radial button's text component.
	/// </summary>
	/// <param name="newText">The new string to apply to the radial button.</param>
	public void UpdateText ( string newText )
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;
		
		// Call the UpdateText function on the associated button.
		radialButton.UpdateText( newText );
	}

	/// <summary>
	/// Assigns a new sprite to the radial button's icon image.
	/// </summary>
	/// <param name="newIcon">The new sprite to assign as the icon for the radial button.</param>
	public void UpdateIcon ( Sprite newIcon )
	{
		// Assign the new icon.
		icon = newIcon;

		// If the radial button is null, then the user hasn't registered this button info yet, so just return.
		if( RadialButtonError_NoWarning )
			return;
		
		// Update the associated button with the new icon sprite.
		radialButton.UpdateIcon( newIcon );
	}

	/// <summary>
	/// Updates the radial button with a new name.
	/// </summary>
	/// <param name="newName">The new string to apply to the radial button's name.</param>
	public void UpdateName ( string newName )
	{
		// Assign the new name.
		name = newName;

		// If the radial button is null, then the user hasn't registered this button info yet, so just return.
		if( RadialButtonError_NoWarning )
			return;

		// Call the UpdateName function on the associated button.
		radialButton.UpdateName( newName );
	}

	/// <summary>
	/// Updates the radial button with a new description.
	/// </summary>
	/// <param name="newDescription">The new string to apply to the radial button's description.</param>
	public void UpdateDescription ( string newDescription )
	{
		// Assign the new description.
		description = newDescription;

		// If the radial button is null, then the user hasn't registered this button info yet, so just return.
		if( RadialButtonError_NoWarning )
			return;

		// Send the new description to the associated button.
		radialButton.UpdateDescription( newDescription );
	}

	/// <summary>
	/// Selects the radial button that this info is attached to.
	/// </summary>
	/// <param name="exclusive">[OPTIONAL] If set to true, will ensure that this button is the only one selected on the radial menu.</param>
	public void SelectButton ( bool exclusive = false )
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// Set selected to true.
		Selected = true;

		// If the user wants to select this button exclusively...
		if( exclusive )
		{
			// Loop through all the radial menu buttons...
			for( int i = 0; i < radialButton.radialMenu.UltimateRadialButtonList.Count; i++ )
			{
				// If this index is the same as our index, then skip this index.
				if( i == radialButton.buttonIndex )
					continue;

				// If the radial button is selected, deselect it.
				if( radialButton.radialMenu.UltimateRadialButtonList[ i ].Selected )
					radialButton.radialMenu.UltimateRadialButtonList[ i ].OnDeselect();
			}
		}
	}

	/// <summary>
	/// Deselects the radial button that this info is attached to.
	/// </summary>
	public void DeselectButton ()
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// Set selected to false.
		Selected = false;
	}

	/// <summary>
	/// Toggles the selection state of the radial button that this information is attached to.
	/// </summary>
	public void ToggleSelect ()
	{
		if( RadialButtonError )
			return;
		
		// If the user has some automatic selection settings...
		if( radialButton.radialMenu.selectButtonOnInteract || ( radialButton.radialMenu.selectButtonOnInteract && radialButton.radialMenu.toggleSelection ) )
		{
			// Inform the user that this will cause unwanted behavior and return.
			Debug.LogWarning( FormatDebug( "You are trying to toggle selection of the radial buttons while the Ultimate Radial Menu has some Automatic Selection settings. This will counteract each other so that nothing will happen", "Please use only one method of selecting buttons", "Unknown (user script)" ) );
			return;
		}

		// Flip the selected value.
		Selected = !Selected;

		// If the input is still on the selected button, then enter it after it has been deselected so that it looks correct.
		if( !Selected && radialButton.radialMenu.IsEnabled && radialButton.IsInAngle( radialButton.radialMenu.GetCurrentInputAngle ) )
			radialButton.OnEnter();
	}

	/// <summary>
	/// Enables the radial menu button.
	/// </summary>
	public void EnableButton ()
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// Call the EnableButton() function all the radial button.
		radialButton.OnEnable();
	}

	/// <summary>
	/// Disables the radial menu button.
	/// </summary>
	public void DisableButton ()
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// Call the DisableButton() function all the radial button.
		radialButton.OnDisable();
	}
	
	/// <summary>
	/// Removes this button from the radial menu.
	/// </summary>
	public void RemoveFromMenu ()
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// Remove the radial button from the menu.
		radialButton.radialMenu.RemoveButton( radialButton.buttonIndex );
		radialButton = null;
	}

	/// <summary>
	/// Removes this buttonInfo from the associated button on the radial menu. The reason to use this function over RemoveFromMenu is that the button itself will still occupy space on the radial menu, instead of deleting the object and repositioning the menu.
	/// </summary>
	public void RemoveInfoFromButton ()
	{
		// If the radial button is null, notify the user and return.
		if( RadialButtonError )
			return;

		// Clear the button information on the associated button, and set the stored radialButton to null.
		radialButton.ClearButtonInformation();
		radialButton = null;
	}

	/// <summary>
	/// [Internal] This function is subscribed to the OnClearButtonInformation callback on the radial button that this is assigned to.
	/// </summary>
	public void OnClearButtonInformation ()
	{
		// Reset this information since the button information was cleared.
		radialButton = null;
	}

	/// <summary>
	/// [INTERNAL] This function is subscribed to the OnSelectedStateChanged callback on the radial button that this is assigned to.
	/// </summary>
	public void OnSelectedStateChanged ( bool selected )
	{
		// Copy the selected state of the radial button.
		this.selected = selected;
	}

	/// <summary>
	/// Returns true if the radial button is not assigned.
	/// </summary>
	bool RadialButtonError_NoWarning
	{
		get
		{
			// If the radial button is null return true for there being an error.
			if( radialButton == null || radialButton.radialMenu == null )
				return true;
			
			// Else there is no problem, so return false.
			return false;
		}
	}

	/// <summary>
	/// Returns true if the radial button is not assigned and displays an error.
	/// </summary>
	bool RadialButtonError
	{
		get
		{
			// If the radial button is null...
			if( radialButton == null || radialButton.radialMenu == null )
			{
				// Inform the user that there is no radial button and return true for there being an error.
				Debug.LogWarning( FormatDebug( "No UltimateRadialButton has been assigned to this button info", "Please register this button info to the radial menu using the RegisterButton function", "Unknown (user script)" ) );
				return true;
			}
			return false;
		}
	}

	string FormatDebug ( string error, string solution, string objectName )
	{
		return "<b>Ultimate Radial Button Info</b>\n" +
			"<color=red><b>×</b></color> <i><b>Error:</b></i> " + error + ".\n" +
			"<color=green><b>√</b></color> <i><b>Solution:</b></i> " + solution + ".\n" +
			"<color=cyan><b>∙</b></color> <i><b>Object:</b></i> " + objectName + "\n";
	}

	// --------------------------------< OBSOLETE FUNCTIONS AND PROPERTIES >-------------------------------- //
	[Obsolete( "This bool is obsolete. Please use the Selected property instead." )]
	public bool IsSelected
	{
		get
		{
			if( RadialButtonError )
				return false;

			return radialButton.Selected;
		}
	}

	[Obsolete( "Please use the RemoveFromMenu function." )]
	public void RemoveRadialButton ()
	{
		if( RadialButtonError )
			return;

		radialButton.radialMenu.RemoveButton( radialButton.buttonIndex );
		radialButton = null;
	}

	[Obsolete( "Please use the RemoveFromMenu function." )]
	public void RemoveInfoFromRadialButton ()
	{
		if( RadialButtonError )
			return;

		radialButton.ClearButtonInformation();
		radialButton = null;
	}

	[Obsolete( "Please use the Registered property instead." )]
	public bool ExistsOnRadialMenu ()
	{
		if( radialButton != null && radialButton.radialMenu != null )
			return true;

		return false;
	}
	// ------------------------------< END OBSOLETE FUNCTIONS AND PROPERTIES >------------------------------ //
}