using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using WeArt.Components;

namespace WeArt.UnityEditor
{
    [CustomEditor(typeof(WeArtStatusTracker), true), CanEditMultipleObjects]
    public class WeArtStatusTrackerEditor : WeArtComponentEditor
    {
        private WeArtStatusTracker StatusTracker => serializedObject.targetObject as WeArtStatusTracker;

        public override VisualElement CreateInspectorGUI()
        {
            var editor = base.CreateInspectorGUI();
            editor.Bind(serializedObject);
            
            {
                var header = new Label("Event");
                header.AddToClassList("header");
                editor.Add(header);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtStatusTracker._OnMiddlewareStatus));
                editor.Add(new PropertyField(property)
                {
                    tooltip = "Event invoked when the middleware status or the status of the connected devices changes"
                });
            }

            if (EditorApplication.isPlaying)
            {
                // Label
                {
                    var header = new Label("Runtime");
                    header.AddToClassList("header");
                    editor.Add(header);
                }

                // Middleware Status
                editor.Add(CreateReadOnlyStringProperty(
                    name: "Status",
                    getter: () => StatusTracker.Status.ToString(),
                    tooltip: "The current Middleware status"
                    ));

                // Middleware version
                editor.Add(CreateReadOnlyStringProperty(
                    name: "Version",
                    getter: () => StatusTracker.Version,
                    tooltip: "The current Middleware version"
                    ));

                // Middleware Status Code
                editor.Add(CreateReadOnlyStringProperty(
                    name: "Status Code",
                    getter: () => StatusTracker.StatusCode.ToString(),
                    tooltip: "Last status code sent by the middleware"
                    ));

                // Number of connected devices
                editor.Add(CreateReadOnlyStringProperty(
                    name: "Connected Devices",
                    getter: () => StatusTracker.Devices is null ? "0" : StatusTracker.Devices.Count.ToString(),
                    tooltip: "Number of devices connected to the middleware"
                    ));
            }

            return editor;
        }

        private VisualElement CreateReadOnlyStringProperty(string name, Func<string> getter, string tooltip)
        {
            var container = new VisualElement();
            container.AddToClassList("propertyRow");

            // Label
            var label = new Label(name + ":");
            label.AddToClassList("unity-base-field__label");
            label.AddToClassList("unity-property-field__label");
            container.Add(label);

            // Status
            var propertyValue = new Label();
            propertyValue.AddToClassList("unity-base-field__label");
            propertyValue.AddToClassList("unity-property-field__label");
            propertyValue.RegisterCallback<AttachToPanelEvent>(evt => EditorApplication.update += updateCallback);
            propertyValue.RegisterCallback<DetachFromPanelEvent>(evt => EditorApplication.update -= updateCallback);
            void updateCallback() => propertyValue.text = getter();
            container.Add(propertyValue);

            container.tooltip = tooltip;
            return container;
        }
    }
}