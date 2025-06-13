using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.TerrainTools;
using Unity.VisualScripting;
using U2DSpriteShapeAnimator.Runtime;
using System;

#if UNITY_EDITOR
namespace U2DSpriteShapeAnimation.Internals
{
    [EditorTool("Shape Point Editor ", typeof(SpriteShapeAnimatorController))]
    public class ShapeAnimationTool : EditorTool
    {
        #region Icons
        private Texture2D iconTex;
        private Texture2D handleIconStart;
        private Texture2D handleIconMiddle;
        private Texture2D handleIconEnd;

        
        private string toolPath = "Assets/Scripts/SpriteShapeAnimator/Editor/Tool Icons/";
        private string iconFileName = "SpriteShapeAnimation_Tool_Icon.png";
        private string handleStartFileName = "SpriteShapeAnimation_Handle_Start.png";
        private string handleMiddleFileName = "SpriteShapeAnimation_Handle_Middle.png";
        private string handleEndFileName = "SpriteShapeAnimation_Handle_End.png"; 
        #endregion

        GUIContent m_Icon;
        SerializedObject serializedController;

        SerializedProperty m_Spline;
        SerializedProperty m_ControlPoints;

        private GUIStyle centeredStyle;
        private SpriteShapeAnimatorController myTarget;

        public void OnEnable()
        {
            iconTex = (Texture2D) AssetDatabase.LoadAssetAtPath($"{toolPath}{iconFileName}", typeof(Texture2D));
            handleIconStart = (Texture2D)AssetDatabase.LoadAssetAtPath($"{toolPath}{handleStartFileName}", typeof(Texture2D));
            handleIconMiddle = (Texture2D)AssetDatabase.LoadAssetAtPath($"{toolPath}{handleMiddleFileName}", typeof(Texture2D));
            handleIconEnd = (Texture2D)AssetDatabase.LoadAssetAtPath($"{toolPath}{handleEndFileName}", typeof(Texture2D));

            m_Icon = new GUIContent()
            {
                image = iconTex,
                tooltip = "Shape Point Editor"
            };
        }

        public override GUIContent toolbarIcon
        {
            get { return m_Icon; }
        }

        public override void OnActivated()
        {
            if (!target)
                return;
            myTarget = (SpriteShapeAnimatorController)target;

            serializedController = new SerializedObject(myTarget.Controller);
            m_Spline = serializedController.FindProperty("m_Spline");
            m_ControlPoints = m_Spline.FindPropertyRelative("m_ControlPoints");
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (EditorApplication.isPlaying)
                return;

            if (serializedController == null)
            {
                //Debug.LogError("Error. Serialized object or controller not found.");
                return;
            }


            // This is extremely hacky, but worked to center the icon
            // Set icon to be text, apply style and revert back.

            centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.MiddleCenter;

            serializedController.Update();

            base.OnToolGUI(window);
            if (!(window is SceneView))
                return;

            UpdateSpriteShapePoints(serializedController, myTarget.AnimationJoints);
        }

        public override void OnWillBeDeactivated()
        {
            if (!target)
                return;
        }

        private void UpdateSpriteShapePoints(SerializedObject so, List<AnimationJoint> animationJoints)
        {
            for (int i = 0; i < animationJoints.Count; i++)
            {
                var element = m_ControlPoints.GetArrayElementAtIndex(i);
                var joint = animationJoints[i];
                if (joint == null)
                    return;

                var handleIcon = i == 0 ? handleIconStart
                                        : i == animationJoints.Count - 1
                                        ? handleIconEnd
                                        : handleIconMiddle;

                var serializedTransf = new SerializedObject(joint.transform);
                var transformProp = serializedTransf.FindProperty("m_LocalPosition");

                Vector3[] point = new Vector3[1] { joint.transform.position };

                EditorGUI.BeginChangeCheck();
                point[0] = Handles.PositionHandle(point[0], Quaternion.identity);

                if (handleIcon != null)
                    Handles.Label(point[0], handleIcon);


                if (EditorSnapSettings.gridSnapEnabled)
                {
                    Handles.SnapToGrid(point);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    var localPoint = point[0] - myTarget.transform.position;

                    transformProp.vector3Value = localPoint;
                    //apply changes to the point
                    element.FindPropertyRelative("position").vector3Value = localPoint;
                    serializedTransf.ApplyModifiedProperties();
                }
            }

            so.ApplyModifiedProperties();
        }

    } 
}
#endif