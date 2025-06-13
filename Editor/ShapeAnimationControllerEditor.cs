using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System.Runtime.Serialization.Formatters;
using static PlasticGui.LaunchDiffParameters;
using Codice.Client.Common.GameUI;
using System.Reflection.Emit;
using UnityEngine.UIElements;
using System.Dynamic;
using UnityEngine.SceneManagement;
using U2DSpriteShapeAnimator.Runtime;

#if UNITY_EDITOR
namespace U2DSpriteShapeAnimation.Internals
{
    [CustomEditor(typeof(SpriteShapeAnimatorController))]
    public class ShapeAnimationControllerEditor : Editor
    {
        private int guiWidth = 300;
        private Color confirm = Color.cyan;
        private Color cancel = new Color(0.7f, 0.25f, 0.07f, 1f);
        //private SerializedProperty autoMatchProperty;
        private SpriteShapeAnimatorController targetAnimationController;

        private bool showConfirmation = false;

        GUIStyle wrappedLabel;

        public GameObject jointPrefabStart;
        public GameObject jointPrefabMiddle;
        public GameObject jointPrefabEnd;

        public void OnEnable()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            targetAnimationController = target as SpriteShapeAnimatorController;
            //autoMatchProperty = serializedObject.FindProperty(nameof(targetAnimationController.autoMatchToSpline));

            var shapeAnimation = target as SpriteShapeAnimatorController;
            if (shapeAnimation.Controller) { }
            if (shapeAnimation.AnimationShape) { }
            if (shapeAnimation.ShapeRenderer) { }

            shapeAnimation.Initialize();
            shapeAnimation.SyncEventSubscribers();
        }

        public void OnDisable()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            targetAnimationController.UnregisterAll();
        }

        public override void OnInspectorGUI()
        {          
            if (EditorApplication.isPlaying)
            {
                return;
            }

            base.OnInspectorGUI();

            #region Custom GUI Formatting
            wrappedLabel = new GUIStyle(GUI.skin.label);
            wrappedLabel.wordWrap = true;

            GUILayout.Space(10f);

            GUI.color = Color.cyan;
            #region Populate Button
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Action when pressing auto populate button
            if (GUILayout.Button("Populate from points", GUILayout.Width(guiWidth)))
            {

                var controller = target as SpriteShapeAnimatorController;
                var splinePointCount = controller.Controller.spline.GetPointCount();

                // if the number of registered animation joints does not match the number of spline points, do logic.
                if (controller.AnimationJoints.Count == splinePointCount)
                {
                    return;
                }

                // Loop through each spline point and instantiate an Animation Joint at its position
                for (int i = 0; i < splinePointCount; i++)
                {
                    var pointPosition = controller.Controller.spline.GetPosition(i);
                    controller.CreateNewJoint(i, pointPosition);
                }
            }

            GUILayout.EndHorizontal();
            #endregion


            #region Removal button
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Action when pressing Remove button
            GUI.color = cancel;
            if (GUILayout.Button("Remove all joints", GUILayout.Width(guiWidth)))
            {
                showConfirmation = !showConfirmation;
            }

            GUILayout.EndHorizontal(); // conf/cancel
            #endregion

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (showConfirmation)
            {
                GUILayout.BeginVertical();
                GUI.color = Color.white;
                GUILayout.Label("This will delete all AnimationJoint assets from the scene.  \n\nAre you sure you want to do this?", wrappedLabel, GUILayout.Width(guiWidth));

                GUILayout.Space(10f);

                GUILayout.BeginHorizontal(GUILayout.Width(guiWidth));
                #region Cancel
                GUI.color = cancel;
                if (GUILayout.Button("No"))
                {
                    showConfirmation = false;
                }
                #endregion

                #region Confirm
                GUI.color = confirm;
                if (GUILayout.Button("Yes"))
                {
                    showConfirmation = false;
                    RemoveAllChildJoints();
                    //RemoveAllChildJoints(myTargets);
                }
                #endregion

                GUILayout.EndHorizontal(); // conf/cancel
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            #endregion
            
            targetAnimationController.SyncEventSubscribers();
            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveAllChildJoints()
        {
            targetAnimationController.RemoveAllJoints();
        }
    } 
}
#endif
