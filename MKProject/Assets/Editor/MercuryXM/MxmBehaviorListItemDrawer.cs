// Copyright (c) 2017, Columbia University 
// All rights reserved. 
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of Columbia University nor the names of its 
//    contributors may be used to endorse or promote products derived from 
//    this software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 
// 
// =============================================================
// Authors: 
// Carmine Elvezio, Mengu Sukan, Steven Feiner
// =============================================================
// 
// 

using System;
using MercuryXM.Framework;
using UnityEditor;
using UnityEngine;

namespace MercuryXM
{
	/// <summary>
	/// MxmBehaviorListItem drawer.
	/// Adapted from http://catlikecoding.com/unity/tutorials/editor/custom-data/
	/// </summary>
	[CustomPropertyDrawer(typeof(MxmBehaviorListItem))]
	public class MxmBehaviorListItemDrawer : PropertyDrawer {

		public static float FieldWidthClone = 15;
		public static float FieldWidthLevel = 40;
		public static float FieldWidthTags = 50;

        /// <summary>
        /// Draw method for MxmBehaviorListItemDrawer.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {

			int oldIndentLevel = EditorGUI.indentLevel;

			EditorGUI.indentLevel = 0;

			label = EditorGUI.BeginProperty(position, label, property);

			Rect contentPosition = EditorGUI.PrefixLabel(position, label);

			/*
			if (position.height > 16f) {
				position.height = 16f;
				EditorGUI.indentLevel += 1;
				contentPosition = EditorGUI.IndentedRect(position);
				contentPosition.y += 18f;
			}
			*/

			var originalWidth = contentPosition.width;
			var remainingWidth = originalWidth;

			/*---------------------------*/
			contentPosition.width = (originalWidth - (FieldWidthClone + FieldWidthLevel + FieldWidthTags))/2f;

            SerializedProperty behaviorProperty = property.FindPropertyRelative("Behavior");

            var level = property.FindPropertyRelative("Level");
		    var listItemName = property.FindPropertyRelative("Name");

            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(
				contentPosition,
                behaviorProperty,
				GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                MxmBehavior behaviorObj = behaviorProperty.objectReferenceValue as System.Object as MxmBehavior;

                if (behaviorObj != null)
                {
                    MonoBehaviour containerObj = property.serializedObject.targetObject as MonoBehaviour;

                    //Get Name & Tag from the assigned object and apply it as the name of the object.
                    listItemName.stringValue = behaviorObj.gameObject.name;

                    //Get the reference of the list owner's WFNode for comparison. 
                    //Then, assign the level based on whether this is the same object, or a different one.
                    MxmLevelFilter levelFilterValue = MxmLevelFilter.Self;

                    if (behaviorObj.gameObject == containerObj.gameObject)
                    {
                        levelFilterValue = MxmLevelFilter.Self;
                    }
                    else
                    {
                        levelFilterValue = MxmLevelFilter.Child;
                    }

                    level.enumValueIndex = Array.IndexOf(Enum.GetValues(typeof(MxmLevelFilter)), levelFilterValue);
                }
            }

            remainingWidth -= contentPosition.width;
			contentPosition.x += contentPosition.width;
			/*---------------------------*/
			contentPosition.width = (originalWidth - (FieldWidthClone + FieldWidthLevel + FieldWidthTags))/2f; // (Same as above)
			 
			EditorGUI.PropertyField(
				contentPosition,
                listItemName,
				GUIContent.none);

			remainingWidth -= contentPosition.width;
			contentPosition.x += contentPosition.width;
			/*---------------------------*/
            contentPosition.width = FieldWidthLevel;

			//Level

            EditorGUI.LabelField(
                contentPosition,
				new GUIContent(level.enumValueIndex != -1 ? level.enumNames[level.enumValueIndex] : "None"),
				EditorStyles.label);

			remainingWidth -= contentPosition.width;
			contentPosition.x += contentPosition.width;
			/*---------------------------*/
			contentPosition.width = FieldWidthClone;

			EditorGUI.PropertyField(
				contentPosition,
				property.FindPropertyRelative("Clone"),
				GUIContent.none);

			remainingWidth -= contentPosition.width;
			contentPosition.x += contentPosition.width;
			/*---------------------------*/

			contentPosition.width = FieldWidthTags;

			EditorGUI.PropertyField(
				contentPosition,
				property.FindPropertyRelative("Tags"),
				GUIContent.none);

			remainingWidth -= contentPosition.width;
			contentPosition.x += contentPosition.width;

			EditorGUI.EndProperty();

			EditorGUI.indentLevel = oldIndentLevel;
		}

		/*
		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			return Screen.width < 333 ? (16f + 18f) : 16f;
		}
		*/
	}

    /// <summary>
    /// Drawer fro handling MxmTags
    /// </summary>
	[CustomPropertyDrawer(typeof(MxmTag))]
	public class MxmTagDrawer : PropertyDrawer
	{

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			property.intValue = (int) (MxmTag) EditorGUI.EnumMaskField(position, label, (MxmTag)property.intValue);
			EditorGUI.EndProperty();
		}
	}

    /// <summary>
    /// Drawer for handling MxmLevelFilters
    /// </summary>
    [CustomPropertyDrawer(typeof(MxmLevelFilter))]
    public class MxmLevelFilterDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            property.intValue = (int) (MxmLevelFilter) EditorGUI.EnumMaskField(position, label, (MxmLevelFilter)property.intValue);
            EditorGUI.EndProperty();
        }
    }
}
