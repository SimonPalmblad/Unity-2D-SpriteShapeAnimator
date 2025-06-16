using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using U2DSpriteShapeAnimator.Internals;

namespace U2DSpriteShapeAnimator.Runtime
{
    [Serializable]
    public class AnimationShape : MonoBehaviour, ISubscriber
    {
        [SerializeField]
        private SpriteShapeController controller;

        public SpriteShapeController Controller
        {
            get
            {
                if (!controller) controller = GetComponent<SpriteShapeController>();
                return controller;
            }
        }

        public void MoveSplinePoint(AnimationJoint joint, int pointIndex)
        {

            if (pointIndex > Controller.spline.GetPointCount() - 1)
            {
                //Debug.LogWarning($"Point Index {pointIndex} is out of range of spline points bound ({controller.spline.GetPointCount()}).");
                return;
            }

            Controller.spline.SetPosition(pointIndex, joint.transform.localPosition);
        }

        public void SubjectUpdated(Subject theChangedSubject, int subjectIndex)
        {
            var joint = theChangedSubject.GetComponent<AnimationJoint>();

            if (joint)
            {
                MoveSplinePoint(joint, subjectIndex);
            }
        }
    }

}