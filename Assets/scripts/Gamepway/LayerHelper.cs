#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace TDCards.Editor
{
    public static class LayerHelper
    {
        [MenuItem("!!!!STUFF!!!!/Sync layers")]
        public static void SyncLayers()
        {
            SynchronizeLayersWithEnum<Layer>();
        }

        [MenuItem("!!!!STUFF!!!!/Sync layers (overwrite on a conflict)")]
        public static void SyncLayersForce()
        {
            SynchronizeLayersWithEnum<Layer>(overwrite: true);
        }

        // TODO: Refactor into a common.editor sort of assembly, move to a package too maybe.
        public static void SynchronizeLayersWithEnum<T>(bool overwrite = false) where T : struct, Enum
        {
            var tagManagerAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (tagManagerAssets.Length != 1)
            {
                Debug.LogError("The TagManager asset not found.");
                return;
            }

            var tagManager = new SerializedObject(tagManagerAssets[0]);

            var layers = tagManager.FindProperty("layers");
            if (layers is not { isArray: true })
            {
                Debug.LogError("Unexpected TagManager asset structure.");
                return;
            }

            int IntFromEnum(T e)
            {
                return (int)(object)e;
            }

            const int maxLayers = 32;
            string?[] expectedNames = new string[maxLayers];
            foreach (var layer in (T[])Enum.GetValues(typeof(T)))
            {
                var i = IntFromEnum(layer);
                if (i is < 0 or >= maxLayers)
                {
                    Debug.LogError($"Layer {layer} with value {i} in the enum is too large to be a layer.");
                    continue;
                }
                expectedNames[i] = layer.ToString();
            }

            IEnumerable<(string ExpectedName, SerializedProperty LayerProperty)> Zip()
            {
                for (int i = 0; i < expectedNames.Length; i++)
                {
                    var expectedName = expectedNames[i];
                    if (expectedName is null)
                    {
                        continue;
                    }

                    var layerProperty = layers.GetArrayElementAtIndex(i);
                    yield return (expectedName, layerProperty);
                }
            }

            bool ExistingNamesMatchWithExpectedIfSpecified_LogErrors()
            {
                bool isError = false;
                foreach (var (expectedName, layerProperty) in Zip())
                {
                    if (layerProperty.stringValue is not { } actualName)
                    {
                        continue;
                    }

                    if (actualName != expectedName)
                    {
                        isError = true;
                        Debug.LogError($"Layer named {actualName} should be named {expectedName}");
                    }
                }
                return isError;
            }

            {
                // Maybe make this an IEnumerable.
                bool hasConflicts = ExistingNamesMatchWithExpectedIfSpecified_LogErrors();
                if (!overwrite && hasConflicts)
                {
                    return;
                }

                if (hasConflicts)
                {
                    Debug.LogWarning("All unmatching layers will be overwritten");
                }
            }

            // Still doing the loop, because some may just be null.
            foreach (var (expectedName, layerProperty) in Zip())
            {
                layerProperty.stringValue = expectedName;
            }

            tagManager.ApplyModifiedProperties();
        }
    }
}
#endif