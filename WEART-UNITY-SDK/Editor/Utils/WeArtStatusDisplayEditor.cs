using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using WeArt.Utils;

namespace WeArt.UnityEditor
{
    [CustomEditor(typeof(WeArtStatusDisplay), true), CanEditMultipleObjects]
    public class WeArtStatusDisplayEditor : WeArtComponentEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var editor = base.CreateInspectorGUI();
            editor.Bind(serializedObject);

            // Label
            {
                var header = new Label("Status Tracking");
                header.AddToClassList("header");
                editor.Add(header);
            }

            // Properties
            {
                var property = serializedObject.FindProperty(nameof(WeArtStatusDisplay.tracker));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the game object containing the middleware status tracker"
                };
                editor.Add(propertyField);
            }

            // Label
            {
                var header = new Label("Status Visualization");
                header.AddToClassList("header");
                editor.Add(header);
            }

            // Properties
            {
                var property = serializedObject.FindProperty(nameof(WeArtStatusDisplay.MiddlewareStatusText));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Text component visualizing the current middleware status"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtStatusDisplay.MiddlewareVersionText));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Text component visualizing the middleware version"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtStatusDisplay.MiddlewareStatusCodeText));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Text component visualizing the current status code"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtStatusDisplay.ActuationsText));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Text component visualizing whether the actuations are enabled or not"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtStatusDisplay.ErrorDescriptionText));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Text component visualizing whether some errors occurs"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtStatusDisplay.ErrorDescriptionPanel));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Component containing the current stastus code description text"
                };
                editor.Add(propertyField);
            }
            
            return editor;
        }
    }
}