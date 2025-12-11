using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Beyond.Networking.Editor
{
    [CustomEditor(typeof(NetworkView), true)]
    public class NetworkViewEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var target = (NetworkView)this.target;
            EditorGUILayout.BeginVertical((GUIStyle)"HelpBox");
            EditorGUILayout.LabelField("Runtime Data", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Scene Id", target.SceneId);
            EditorGUILayout.IntField("Instantiation Id", target.InstantiationId);
            EditorGUILayout.IntField("View Id", target.ViewId);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            base.OnInspectorGUI();
        }
    }
}
