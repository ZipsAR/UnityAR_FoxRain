using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer( typeof( UltimateRadialButtonInfo ) )]
public class UltimateRadialButtonInfoPropertyDrawer : PropertyDrawer
{
	bool showMore = false;

	public override float GetPropertyHeight ( SerializedProperty property, GUIContent label )
	{
		int lineCount = 1;
		int endSpacingModifier = 0;
		if( showMore )
		{
			endSpacingModifier = 4;
			lineCount = 8;
		}

		return EditorGUIUtility.singleLineHeight * lineCount + ( ( lineCount * 2 ) + endSpacingModifier );
	}

	public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label )
	{
		EditorGUI.BeginProperty( position, label, property );

		int i = 0;

		GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbarButton ) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 11, stretchWidth = false };
		showMore = GUI.Toggle( GetNewPositionRect( EditorGUI.IndentedRect( position ), i++ ), showMore, ( showMore ? "▼" : "►" ) + label.text, toolbarStyle );

		position.y += 2;
		
		if( showMore )
		{
			EditorGUI.indentLevel++;
			EditorGUI.PropertyField( GetNewPositionRect( position, i++ ), property.FindPropertyRelative( "key" ), new GUIContent( "Key", "The string key associated with this element." ) );
			EditorGUI.PropertyField( GetNewPositionRect( position, i++ ), property.FindPropertyRelative( "id" ), new GUIContent( "ID", "The integer ID associated with this element." ) );
			EditorGUI.PropertyField( GetNewPositionRect( position, i++ ), property.FindPropertyRelative( "name" ), new GUIContent( "Name", "The name of this element." ) );

			Event mEvent = Event.current;

			if( mEvent.type == EventType.KeyDown && mEvent.keyCode == KeyCode.Return )
			{
				string control = GUI.GetNameOfFocusedControl();
				GUI.FocusControl( control );
			}

			if( property.FindPropertyRelative( "description" ).stringValue == string.Empty && Event.current.type == EventType.Repaint )
			{
				GUIStyle style = new GUIStyle( GUI.skin.textField ) { wordWrap = true };
				style.normal.textColor = new Color( 0.5f, 0.5f, 0.5f, 0.75f );
				EditorGUI.DelayedTextField( GetNewPositionRect( position, i++, 52 ), "Description", style );
			}
			else
			{
				GUIStyle style = new GUIStyle( GUI.skin.textField ) { wordWrap = true };
				property.FindPropertyRelative( "description" ).stringValue = EditorGUI.DelayedTextField( GetNewPositionRect( position, i++, 52 ), property.FindPropertyRelative( "description" ).stringValue, style );
			}

			i += 2;

			EditorGUI.PropertyField( GetNewPositionRect( position, i++ ), property.FindPropertyRelative( "icon" ) );
			EditorGUI.indentLevel--;
		}
		EditorGUI.EndProperty();
	}

	Rect GetNewPositionRect ( Rect position, int i, int height = 16 )
	{
		return new Rect( position.x, position.y + ( 18 * i ), position.width, height );
	}
}