using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using WeArt.Components;

namespace WeArt.UnityEditor
{
    /// <summary>
    /// A custom inspector for components of type <see cref="WeArtHandController"/>.
    /// </summary>
    [CustomEditor(typeof(WeArtHandController), true), CanEditMultipleObjects]
    public class WeArtHandControllerEditor : WeArtComponentEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var editor = base.CreateInspectorGUI();
            editor.Bind(serializedObject);

            // Label
            {
                var header = new Label("Hand Side");
                header.AddToClassList("header");
                editor.Add(header);
            }

            // Hand Side
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._handSide));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Decleare hand side parameter"
                };
                editor.Add(propertyField);
            }

            // Label
            {
                var header = new Label("Hand states");
                header.AddToClassList("header");
                editor.Add(header);
            }

            // Open hand state
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._openedHandState));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the animation clip containing the open (relaxed) hand pose"
                };
                editor.Add(propertyField);
            }

            // Closed hand state
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._closedHandState));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the animation clip containing the closed hand pose"
                };
                editor.Add(propertyField);
            }

            // Abduction hand state
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._abductionHandState));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the animation clip containing the abduction hand pose"
                };
                editor.Add(propertyField);
            }

            // Label
            {
                var header = new Label("Fingers avatar masks");
                header.AddToClassList("header");
                editor.Add(header);
            }

            // Fingers avatar masks
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._thumbMask));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the avatar mask for the thumb"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._indexMask));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the avatar mask for the index"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._middleMask));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the avatar mask for the middle"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._ringMask));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the avatar mask for the ring"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._pinkyMask));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the avatar mask for the pinky"
                };
                editor.Add(propertyField);
            }

            // Label
            {
                var header = new Label("Renderer");
                header.AddToClassList("header");
                editor.Add(header);
            }

            // Skinned Mesh Renderer
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._handSkinnedMeshRenderer));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the SkinnedMeshRenderer for the hand"
                };
                editor.Add(propertyField);
            }
            // Logo Panel
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._logoHandPanel));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the Logo panel for the hand"
                };
                editor.Add(propertyField);
            }

            // Label
            {
                var header = new Label("Ghost Hand Materials");
                header.AddToClassList("header");
                editor.Add(header);
            }

            // Ghost Hand Invisible Material
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._ghostHandInvisibleMaterial));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the Logo panel for the hand"
                };
                editor.Add(propertyField);
            }

            // Ghost Hand Transparent Material
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._ghostHandTransparentMaterial));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the Logo panel for the hand"
                };
                editor.Add(propertyField);
            }

            // Label
            {
                var header = new Label("Thimbles Tracking");
                header.AddToClassList("header");
                editor.Add(header);
            }

            // Thimbles Tracking
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._thumbThimbleTracking));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the thimble for the thumb"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._indexThimbleTracking));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the thimble for the index"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._middleThimbleTracking));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the thimble for the middle"
                };
                editor.Add(propertyField);
            }

            // Label
            {
                var header = new Label("Thimbles Haptic");
                header.AddToClassList("header");
                editor.Add(header);
            }

            // Thimbles Haptic
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._thumbThimbleHaptic));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the thimble for the thumb"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._indexThimbleHaptic));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the thimble for the index"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._middleThimbleHaptic));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the thimble for the middle"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._palmThimbleHaptic));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the thimble for the palm"
                };
                editor.Add(propertyField);
            }

            // Grasper
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._grasper));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Trasform position Grasper"
                };
                editor.Add(propertyField);
            }

            // Label
            {
                var header = new Label("Grasp Check Haptics");
                header.AddToClassList("header");
                editor.Add(header);
            }

            // Thimbles Haptic
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._thumbGraspCheckHaptic));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the thimble for the thumb"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._indexGraspCheckHaptic));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the thimble for the index"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._middleGraspCheckHaptic));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the thimble for the middle"
                };
                editor.Add(propertyField);
            }

            // Label
            {
                var header = new Label("Real Thimbles Position");
                header.AddToClassList("header");
                editor.Add(header);
            }

            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._realThumbPosition));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the real position for the thumb"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._realIndexPosition));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the real position for the index"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._realMiddlePosition));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the real position for the middle"
                };
                editor.Add(propertyField);
            }

            // Label
            {
                var header = new Label("Surface Exploration Origins");
                header.AddToClassList("header");
                editor.Add(header);
            }

            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._thumbExplorationOrigin));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the origin of the surface exploration the thumb"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._indexExplorationOrigin));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the origin of the surface exploration the index"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._middleExplorationOrigin));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the origin of the surface exploration for the middle"
                };
                editor.Add(propertyField);
            }
            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._palmExplorationOrigin));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the origin of the surface exploration for the palm"
                };
                editor.Add(propertyField);
            }

            // Label
            {
                var header = new Label("Surface Exploration Target Locations");
                header.AddToClassList("header");
                editor.Add(header);
            }

            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._explorationThumbHapticTransform));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the origin of the surface exploration for the palm"
                };
                editor.Add(propertyField);
            }

            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._explorationIndexHapticTransform));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the origin of the surface exploration for the palm"
                };
                editor.Add(propertyField);
            }

            {
                var property = serializedObject.FindProperty(nameof(WeArtHandController._explorationMiddleHapticTransform));
                var propertyField = new PropertyField(property)
                {
                    tooltip = "Select here the origin of the surface exploration for the palm"
                };
                editor.Add(propertyField);
            }

            return editor;
        }
    }
}