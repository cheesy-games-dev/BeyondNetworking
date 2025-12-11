using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Beyond.Networking.Editor
{
    [CustomEditor(typeof(ObservableBehaviour), true), CanEditMultipleObjects]
    public class ObservableBehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Network View", target.GetNetworkView(), typeof(NetworkView), true);
            EditorGUI.EndDisabledGroup();
            base.OnInspectorGUI();
        }
    }
}
