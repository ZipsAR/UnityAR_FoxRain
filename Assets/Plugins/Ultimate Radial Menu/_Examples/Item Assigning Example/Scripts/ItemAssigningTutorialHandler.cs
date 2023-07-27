using UnityEngine;
using UnityEngine.UI;

public class ItemAssigningTutorialHandler : MonoBehaviour
{
	public Text tutorialText;

	void Start ()
	{
		UltimateRadialMenu.ReturnComponent( "ItemWheelExample" ).OnMenuEnabled += OnRadialMenuEnabled;
		UltimateRadialMenu.ReturnComponent( "ItemWheelExample" ).OnMenuDisabled += OnRadialMenuDisabled;
	}

	void OnRadialMenuEnabled ()
	{
		tutorialText.text = "Click and drag the items on the right to the radial menu to assign them to a button.\n\nClicking on the item will increase the count of that item.";
	}

	void OnRadialMenuDisabled ()
	{
		tutorialText.text = "Press SPACE BAR to open your radial menu.";
	}
}