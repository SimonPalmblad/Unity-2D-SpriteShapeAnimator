using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using U2DSpriteShapeAnimator.Runtime;

namespace U2DSpriteShapeAnimation.Internals
{
    [CustomEditor(typeof(AnimationJoint))]
    public class AnimationJointEditor : Editor
    {
        private void OnSceneGUI()
        {
            AnimationJoint myTarget = (AnimationJoint)target;
            myTarget.CheckJointUpdate();
        }
    } 
}
