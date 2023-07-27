/* UltimateRadialMenuReadme.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using System.Collections.Generic;

//[CreateAssetMenu( fileName = "README", menuName = "Tank and Healer Studio/Ultimate Radial Menu README File", order = 1 )]
public class UltimateRadialMenuReadme : ScriptableObject
{
	public Texture2D icon;
	public Texture2D scriptReference;
	public Texture2D settings;

	// GIZMO COLORS //
	[HideInInspector]
	public Color colorDefault = Color.black;
	[HideInInspector]
	public Color colorValueChanged = Color.cyan;
	[HideInInspector]
	public Color colorButtonSelected = Color.yellow;
	[HideInInspector]
	public Color colorButtonUnselected = Color.white;
	[HideInInspector]
	public Color colorTextBox = Color.yellow;

	public static int ImportantChange = 3;
	public class VersionHistory
	{
		public string versionNumber = "";
		public string[] changes;
		public bool showMore = false;
	}
	public VersionHistory[] versionHistory = new VersionHistory[]
	{
		// VERSION 2.7.1 //
		new VersionHistory()
		{
			versionNumber = "2.7.1",
			changes = new string[]
			{
				// BUG FIXES //
				"Fixed a small issue when manually deselecting a button immediately after the radial menu had been enabled",
				"Fixed a rare issue that could leave the current button in the 'pressed' state when using the button to add a new button to the menu",
			},
			showMore = true,
		},
		// VERSION 2.7.0 // IMPORTANT CHANGE 3 //
		new VersionHistory()
		{
			versionNumber = "2.7.0",
			changes = new string[]
			{
				// QUALITY OF LIFE //
				"Simplified and improved the toggle functionality of the radial menu, allowing for both Scale and Fade to be used at the same time",
				"Updated the Debugs to have more information and formatted them to be easier to read and understand the solution",
				"Added Drag & Drop functionality for opening sections of the Ultimate Radial Menu inspector. This feature can be disabled in the README settings if needed",
				"Adjusted the Script Reference section of the Ultimate Radial Menu to be more useful even when a Radial Menu Name has not been assigned for easy reference. Now public reference code is displayed in the Example Code Generator",
				// ADDED //
				"Added 2 new options to the input manager component for invoking the button events: OnButtonUp and OnButtonEnter",
				"Added a new function: EnablePointer() to the Ultimate Radial Menu script. This function forces the pointer to go in to an enabled state, which is needed for the sub menu add-on package",
				"Added 4 new functions to the UltimateRadialButton class to correspond with the button info class: UpdateIcon, UpdateText, UpdateName, UpdateDescription",
				// RENAMED //
				"In an attempt to simplify the reference to the Ultimate Radial Menu through code even more, we depreciated and renamed several functions and callbacks, listed below",
				// UltimateRadialMenu
				"Depreciated the RadialMenuActive property. Please use IsEnabled instead",
				"Depreciated the RegisterToRadialMenu function. Please use RegisterButton",
				"Depreciated the EnableRadialMenu function. Please use Enable",
				"Depreciated the DisableRadialMenu function. Please use Disable",
				"Depreciated the DisableRadialMenuImmediate function. Please use DisableImmediate",
				"Depreciated the CreateEmptyRadialButton function. Please use CreateEmptyButton",
				"Depreciated the RemoveAllRadialButtons function. Please use ClearMenu",
				"Depreciated the RemoveRadialButton function. Please use RemoveButton",
				"Depreciated the ClearRadialButtonInformations function. Please use ClearButtonInformations",
				"Depreciated the GetUltimateRadialMenu function. Please use ReturnComponent",
				// UltimateRadialButton
				"Depreciated the EnableButton function of the UltimateRadialButton class. Please use OnEnable if you need to",
				"Depreciated the DisableButton function of the UltimateRadialButton class. Please use OnDisable if you need to",
				// UltimateRadialButtonInfo
				"Depreciated the RemoveRadialButton function of the UltimateRadialButtonInfo class. Please use RemoveFromMenu instead",
				"Depreciated the ExistsOnRadialMenu function of the UltimateRadialButtonInfo class. Please use Registered property instead",
				"Depreciated the RemoveInfoFromRadialButton function of the UltimateRadialButtonInfo class. Please use RemoveInfoFromButton property instead",
				// Events
				"Depreciated the OnRadialButtonEnter event. Please use OnButtonEnter",
				"Depreciated the OnRadialButtonExit event. Please use OnButtonExit",
				"Depreciated the OnRadialButtonInputDown event. Please use OnButtonInputDown",
				"Depreciated the OnRadialButtonInputUp event. Please use OnButtonInputUp",
				"Depreciated the OnRadialButtonInteract event. Please use OnButtonInteract",
				"Depreciated the OnRadialButtonSelected event. Please use OnButtonSelected",
				"Depreciated the OnRadialMenuLostFocus event. Please use OnMenuLostFocus",
				"Depreciated the OnRadialMenuEnabled event. Please use OnMenuEnabled",
				"Depreciated the OnRadialMenuDisabled event. Please use OnMenuDisabled",
				"Depreciated the OnRadialMenuButtonCountModified event. Please use OnButtonCountModified",
				"Depreciated the OnRadialMenuProcessInput event. Please use OnProcessInput",
				"While the above functions and callbacks are 'deprecated', they will be removed in a future update to help keep the code clean and tidy. Please make sure to update your personal code references to the Ultimate Radial Menu to the new methods as soon as possible",
				// BUG FIXES //
				"Fixed a small issue when using the On Menu Release option of the Input Manager while the Enable Menu Setting was set to Toggle",
				// REMOVED //
				"Removed the UltimateRadialMenuScreenSizeUpdater script to help simplify the package",
				// GENERAL //
				"Several tooltip changes and other small editor updates",
				"Renamed several radial menu sprites to help keep them organized when searching sprites in your project",
				"Replaced and updated some old radial menu sprites to look better",
			},
		},
		// VERSION 2.6.1
		new VersionHistory()
		{
			versionNumber = "2.6.1",
			changes = new string[]
			{
				// GENERAL //
				"Fully integrated the Pointer functionality into the base Ultimate Radial Menu class. This makes it easier to add and edit the pointer associated with the radial menu",
				"Along with the above change updated all radial menu prefabs to use the new integrated pointer functionality as well as all the example scene radial menus",
				"Improved the radial menu pointer functionality overall",
				"Added collapsible section buttons to the Display Name and Display Description options in the inspector",
				"Adjusted some of the radial menu images",
				"Changed the radial menu that is used in the Gun Wheel example scene",
				// NEW //
				"Added a new option for the button icon when using the Local Rotation option. This new option is called: Smart Rotation",
				// REMOVED //
				"Removed the UltimateRadialMenuPointer script from the package",
				// BUG FIX //
				"Modified the code used with the new Input System to specify the new Input Systems InputAction class. This fixes a very rare issue that could occur when users had Photon Fusion in their projects",
				"Fixed a very rare issue that could occur when adding a World Space radial menu to the scene, and then trying to Undo the action of creating it",
				"Fixed a small bug that would occur when using the Enter Play Mode Settings inside of Unity",
			}
		},
		// VERSION 2.6.0
		new VersionHistory()
		{
			versionNumber = "2.6.0",
			changes = new string[]
			{
				// GENERAL //
				"Changed the way that selected buttons are calculated. Now this information is stored within the Button Info class so that it retains this information even when the button is removed from the menu",
				"Updated the SelectButton() function inside the UltimateRadialButtonInfo class to have an optional parameter for exclusively selected that button on the menu",
				// NEW
				"Included a radial menu image and prefab for a new style: Circle",
				"Added the ability to create an Ultimate Radial Menu object from scratch using the GameObject menu (or right clicking in the Hierarchy). This option is located in GameObject/UI/Ultimate Radial Menu",
				"Added the ability to define a certain angle for the radial menu to use overall. Also, adjusted the radial button styles code to adapt to the closest available button image automatically if the user is utilizing a style",
				"Along with the overall angle feature, added a new variable to control the center angle of the menu when the overall angle is less than 360 degrees",
				"Added new public function to the UltimateRadialButtonInfo class: ToggleSelect(). This function will toggle the radial buttons selection state",
				"Added new property inside the UltimateRadialButtonInfo class: Selected. This property stores the selected state of the button information, and can be modified to select/deselect the button associated with that button info",
				"Added a new Action callback: OnRadialMenuProcessInput. This callback can be used for add-ons to the Ultimate Radial Menu that need to have access to the same input information",
				// REMOVED
				"Removed the menuButtonCount variable from the Ultimate Radial Menu script. Developers should now reference the UltimateRadialButtonList count if they need to know the current button count",
				"Depreciated the IsSelected property of the UltimateRadialButtonInfo class. Developers should now use the Selected property instead",
				// BUG FIX //
				"Fixed a rare issue that could occur when removing all the buttons if some were disabled",
				"Fixed an issue where the Ultimate Radial Menu Input Manager would still catch controller input in some very rare cases",
				"Fixed an editor issue for the button text color",
			}
		},
		// VERSION 2.5.0
		new VersionHistory()
		{
			versionNumber = "2.5.0",
			changes = new string[]
			{
				// ADDED //
				"Added complete support for the new Input System from Unity",
				"Added new option to the Ultimate Radial Menu Input Manager that gives more control to the user if they want to Hold or Toggle the radial menu state",
				"Added new function: SendRaycastInput to the Input Manager script. This function will take your custom raycast information and project it for input on the radial menus",
				"Added a variable to keep track of the current input device used by the Ultimate Radial Menu Input Manager",
				"Added the ability to create an Ultimate Radial Menu from scratch using the GameObject/UI menu",
				// GENERAL //
				"Updated all example scenes to avoid any conflicts with the new Input System",
				"Put in a simple check to replace any old EventSystem components to avoid any errors when opening old example scenes. This is only done when the user has the new Input System installed",
				"Changed the calculations of the Ultimate Radial Menu Pointer code when using a Pointer Style to better adjust to the number of buttons",
				"Simplified the options for the Input Manager to be easier to work with",
				"Adjusted the link color in the README file to hopefully be easier to look at",
				"Improvements to the Touch Input functionality in the Input Manager",
				"Removed the restriction of not being able to enable world space radial menus using the global Input Manager",
				"Improved the canvas settings when creating a new World Space canvas for a radial menu",
				"Renamed the Virtual Reality Input option to Center Screen Input which defines what it does better. Along with this change, added the ability to assign left and right eye cameras for a VR setup with this input type. Also, expanded the functionality of this input type greatly",
				"Updated the code that unpacks the radial menu prefab to not try to unpack if it isn't the root of the prefab. Instead, a warning will be displayed on the editor when inside a prefab to explain that any unwanted behavior is caused by the prefab",
				"Updated the input manager code to better handle the current Instance to reference",
				// BUG FIXES //
				"Fixed a rare issue that could occur when undoing actions when adding and removing buttons with certain options",
				"Fixed an issue with the OnRadialButtonExit callback not being called sometimes",
				"Fixed an issue that could make the pointer snap back to a default rotation when exiting the radial menu",
				"Added a quick register of all radial menu buttons in Awake to avoid any issues if the user tries to access and modify the buttons individually in their own Start functions",
			}
		},
		// VERSION 2.2.2
		new VersionHistory()
		{
			versionNumber = "2.2.2",
			changes = new string[]
			{
				// GENERAL //
				"Added some code that will unpack the radial menu prefab when adding to a scene. Unity's prefab system was forcing unwanted behavior on the radial menus and reverting button sprites",
				// REMOVED //
				"Removed all the depreciated functions from the UltimateRadialMenu.cs script that were depreciated in version 2.0.0",
				// BUG FIXES //
				"Fixed a rare issue where the radial button would revert back to a null sprite if selecting or deselecting a button",
				"Fixed a very rare issue with the Callback system invoking multiple callbacks when assigning a string with the invoked function",
			}
		},
		// VERSION 2.2.1
		new VersionHistory()
		{
			versionNumber = "2.2.1",
			changes = new string[]
			{
				// BUG FIXES //
				"Fixed the Ultimate Radial Menu Input Manager code to correctly use the unique Interact Settings from the unique input manager if it is being used",
			}
		},
		// VERSION 2.2.0
		new VersionHistory()
		{
			versionNumber = "2.2.0",
			changes = new string[]
			{
				// GENERAL //
				"Improved the input manager to support multiple controller joysticks for individual radial menus if needed",
				"Updated functionality for the radial menu when being used in Screen Space - Camera",
				"Improved the Ultimate Radial Menu Input Manager for the World Space and Screen Space - Camera Canvas options",
				"Updated the radial menu to retain the current selected button even when adding and removing buttons at runtime",
				"Updated the Gun Wheel Example scene to correctly display the currently selected weapon when changing from menu to menu",
				"Removed the Simple World Space Example files and replaced it with a new example named: Look At Input Example which will hopefully be more useful",
				"Moved the Ultimate Radial Menu folder into a root folder named: Tank & Healer Studio",
			}
		},
		// VERSION 2.1.6
		new VersionHistory()
		{
			versionNumber = "2.1.6",
			changes = new string[]
			{
				// GENERAL //
				"Added a check for the radial menu not being in world space before calculating the input in relative position of the menu",
				// BUG FIXES //
				"Fixed an issue with the radial menu not processing input correctly when using a controller for input",
			}
		},
		// VERSION 2.1.5
		new VersionHistory()
		{
			versionNumber = "2.1.5",
			changes = new string[]
			{
				// GENERAL //
				"Improved performance when processing input to a radial menu",
				"Small improvements to the README file",
				// BUG FIXES //
				"Fixed a rare issue that could occur when using one radial menu for sub menus too",
			}
		},
		// VERSION 2.1.4
		new VersionHistory()
		{
			versionNumber = "2.1.4",
			changes = new string[]
			{
				// GENERAL //
				"Small overall improvements to the positioning of the radial menu on the canvas with different canvas options",
				"Updated the SetPosition function to optionally allow for local space as well",
				"Improved the Input Manager script internally to handle input better for different canvas options",
				"Added a new option to the Touch Input section of the Input Manager script. This new option allows the user a more clear way to control the enabled/disabled state when using the Touch Input option",
				// BUG FIXES //
				"Fixed the OnRadialButtonSelected callback from being called multiple times without any new selected buttons",
				"Fixed a rare issue where the Input Manager would throw an error if a menu was deleted at runtime",
			}
		},
		// VERSION 2.1.3
		new VersionHistory()
		{
			versionNumber = "2.1.3",
			changes = new string[]
			{
				// BUG FIXES //
				"Fixed a small error that could occur when loading the README file",
			}
		},
		// VERSION 2.1.2
		new VersionHistory()
		{
			versionNumber = "2.1.2",
			changes = new string[]
			{
				// GENERAL //
				"Updated the input manager script to have a specific option for disabling the ability to toggle the radial menu state from the input manager. This is useful for if the user wants to enable/disable the radial menu through their custom code",
				"Updated the input manager editor script to be easier to work with and more visually appealing. Additionally added the Development Inspector option to the input manager editor",
				"Corrected a reference to a depreciated function in the example code for the README file",
				"Improved the README file to stay on the same page even after Unity compiles scripts",
				// BUG FIXES //
				"Fixed a small error that would occur if the user had a class named Outline which would cause conflicts with Unity's default Outline class",
				"Fixed a small error that could occur if the user added the Ultimate Radial Menu script to a non UI object",
			}
		},
		// VERSION 2.1.1
		new VersionHistory()
		{
			versionNumber = "2.1.1",
			changes = new string[]
			{
				// GENERAL //
				"Added a new prefab for the Dark style to have the center image",
				// ADDED METHODS //
				"Added a function to the Input Manager to update the current camera used for calculations for World Space radial menus. This is useful for when a camera is not tagged as MainCamera or if the user is changing cameras at runtime",
				"Added a new public function to the Input Manager to specifically set the current camera used for calculations for World Space radial menus",
			}
		},
		// VERSION 2.1.0
		new VersionHistory()
		{
			versionNumber = "2.1.0",
			changes = new string[]
			{
				// BUG FIXES //
				"Fixed a small issue when updating the icon from code when registering the button information without an icon assigned",
				"Fixed a issue when using a radial menu without a button sprite assigned",
				"Various performance improvements",
				"Various small bug fixes",
				// GENERAL //
				"Changed the function of the Angle Offset setting to work better with dynamically adding and removing buttons at runtime",
				"Added 2 new scripts to handle a complete visual style for each different button count of the radial menu. These scripts are the: UltimateRadialMenuStyle and UltimateRadialMenuStyleEditor",
				"Revamped existing radial menu images to work with the new style system",
				"Improved the Ultimate Radial Menu Pointer script to handle different styles",
				"Simplified the Ultimate Radial Menu Pointer inspector to be easier to work with",
				"Added object pooling to the radial menu to improve performance when clearing and populating the menu many times during runtime",
				"Expanded the functionality of the RemoveAllRadialButtons function to allow for keeping a certain number of buttons if needed. This can improve performance if clearing the menu just to register new information right after",
				"Changed the position modifier option for the different states to allow for negative values",
				"Removed the enum option for text positioning since there was only two options. Replaced it with a boolean for Local Position",
			}
		},
		// VERSION 2.0.0 // IMPORTANT CHANGE 2 //
		new VersionHistory ()
		{
			versionNumber = "2.0.0",
			changes = new string[]
			{
				// GENERAL //
				"Added positioning support for world space canvas",
				"Improved positioning calculations to support other canvas options",
				"Updated input manager logic to support world space canvas",
				"Added new option to Input Manager: Virtual Reality Input. This uses the center of the screen to interact with radial menu",
				"Updated the editor sections to help be more clear. Some options may have moved to other sections",
				"Updated editor script visually and added collapsible sections",
				"Added functionality and options for displaying a currently selected button",
				"Heavily modified the button interaction to allow for more customization",
				"Created a new C# script to hold the UltimateRadialButtonInfo class for easier modification",
				"Included in-engine documentation to the README file and removed links to outdated online documentation",
				"Added a new simple example scene to show the world space radial menu in action",
				"Included new options for inverting the axis of the controller input",
				// REMOVED //
				"Removed the Text Positioning Option: Relative to Icon",
				"Removed the options for disabling the icon and text when the button is disabled. This functionality still exists when using the Color Change option for the icon and text",
				"Removed the callback: OnRadialMenuButtonFound. A new callback with improved functionality is OnRadialButtonEnter",
				// ADDED METHODS //
				"Added a new function: RegisterToRadialMenu. This function now handles all the functionality for adding any sort of information to the radial menu",
				"Added a new function: ClearRadialButtonInformations. This function clears all the registered button information",
				"Added a new function: RemoveAllRadialButtons. This function deletes all of the radial buttons in the menu",
				"Added a new function: CreateEmptyRadialButton. This function creates a new button with no information attached to it",
				// DEPRECIATED //
				"Depreciated the UpdateSizeAndPlacement function. This name wasn't easy to find or understand. The new function name is: UpdatePositioning",
				"Depreciated the GetUltimateRadialButton function. The radial button can be referenced by it's index",
				"Depreciated the UpdateRadialButton functions. The new RegisterToRadialMenu function now handles the same functionality",
				"Depreciated the AddRadialButton functions. The new RegisterToRadialMenu function now handles the same functionality",
				"Depreciated the InsertRadialButton functions. The new RegisterToRadialMenu function now handles the same functionality",
				"Depreciated the ClearRadialButtons functions. Please use RemoveAllRadialButtons instead",
				"Depreciated the OnRadialMenuButtonFound callback action. The OnRadialButtonEnter callback action should be instead",
				"Depreciated the OnUpdateSizeAndPlacement callback action. The OnUpdatePositioning callback action should be instead",
				"Depreciated the EnableInteraction function. If you want to enable interaction on the radial menu simply use the Interactable variable",
				"Depreciated the DisableInteraction function. If you want to disable interaction on the radial menu simply use the Interactable variable",
				"Depreciated the ResetRadialButtonInformation function. If you want to clear the information on the radial button use the ClearButtonInformation function",
			},
		},
		// VERSION 1.1.0
		new VersionHistory ()
		{
			versionNumber = "1.1.0",
			changes = new string[]
			{
				"Simplified the editor script internally",
				"Removed AnimBool functionality from the inspector to avoid errors with Unity 2019+",
				"Improved internal control of input interaction",
				"Added new option for the initial state of the radial menu to allow it to be enabled at the start of the scene",
				"Simplified internal calculations when assigning a new radial button",
				"Renamed example scene files to better identify their purpose",
				"Added new example scene for placing the radial menu over a world object's position",
				"Added new public function: ResetPosition(). This function will reset the position of the Ultimate Radial Menu to the default position that was calculated",
				"Added new public function in the UltimateRadialButton class: AddCallback(). This function will subscribe the provided function to the radial button interaction",
				"Added new public function: DisableInteraction(). This function will disable interaction on the radial menu",
				"Added new public function: EnableInteraction(). This function will enable interaction on the radial menu",
				"Added new public static function: SetPosition(). This function calls the public SetPosition() function on the targeted Ultimate Radial Menu",
				"Added new public and public static functions: GetUltimateRadialButton(). This function will return the targeted Ultimate Radial Button",
				"Added a new option to the Ultimate Radial Menu Input Manager: Touch Input. This option enables touch input on the radial menu so that it can be used in mobile projects",
				"Added new script: UltimateRadialMenuReadme.cs",
				"Added new script: UltimateRadialMenuReadmeEditor.cs",
				"Added new file at the Ultimate Radial Menu root folder: README. This file has all the documentation and how to information",
				"Removed the UltimateRadialMenuWindow.cs file. This script can safely be removed from your project. All of that information and more is now located in the README file",
				"Removed the old README text file. All of that information is now located in the README file",
				"Added options for changing the scene gizmo colors for the Ultimate Radial Menu. These options are located in the new README file in Settings",
			},
		},
		// VERSION 1.0.5
		new VersionHistory ()
		{
			versionNumber = "1.0.5",
			changes = new string[]
			{
				"Renamed prefabs to help know what the object is in the hierarchy",
			},
		},
		// VERSION 1.0.4
		new VersionHistory ()
		{
			versionNumber = "1.0.4",
			changes = new string[]
			{
				"Updated the Ultimate Radial Menu editor to display a few more functions in the generated example code",
				"Modified the Input Manager script to have virtual functions, allowing custom input scripts to be implemented",
				"Added new public function: <b>SetPosition</b> to allow for easily changing the position of the radial menu on the screen",
			},
		},
		// VERSION 1.0.3
		new VersionHistory ()
		{
			versionNumber = "1.0.3",
			changes = new string[]
			{
				"Fixed the Ultimate Radial Menu Editor to display the correct script reference code",
				"Overall bug fixes to the scripts",
			},
		},
		// VERSION 1.0.2
		new VersionHistory ()
		{
			versionNumber = "1.0.2",
			changes = new string[]
			{
				"Modified Input Manager script to handle all of the input for the radial menu. It is also now placed on the EventSystem in the scene",
				"Added a button to the Ultimate Radial Menu inspector to select the input manager",
			},
		},
		// VERSION 1.0.1
		new VersionHistory ()
		{
			versionNumber = "1.0.1",
			changes = new string[]
			{
				"Modified Input Manager script to allow for no key to be used for enabling and disabling the menu",
			},
		},
		// VERSION 1.0
		new VersionHistory ()
		{
			versionNumber = "1.0",
			changes = new string[]
			{
				"Initial Release",
			},
		},
	};

	[HideInInspector]
	public List<int> pageHistory = new List<int>();
	[HideInInspector]
	public Vector2 scrollValue = new Vector2();
}