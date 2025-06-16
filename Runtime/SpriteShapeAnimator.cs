using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using U2DSpriteShapeAnimator.Internals;
using UnityEngine.SceneManagement;
using Codice.CM.Client.Differences.Graphic;


namespace U2DSpriteShapeAnimator.Runtime
{
    [Serializable]
    public class SpriteShapeAnimator : EventManager
    {
        [SerializeField]
        private SpriteShapeController controller;

        [SerializeField]
        private AnimationShape animationShape;

        [SerializeField]
        [HideInInspector]
        private List<AnimationJoint> animationJoints = new List<AnimationJoint>();

        [SerializeField][HideInInspector]        
        private SpriteShapeRenderer shapeRenderer;

        [SerializeField][HideInInspector]
        private int shapeLayerOrder;

        public bool autoMatchToSpline = false;
        public bool autoRename = true;

        public void AssignSpriteShapeController(SpriteShapeController controller, AnimationShape shape)
        {
            this.controller = controller;
            this.animationShape = shape;
        }

        public SpriteShapeRenderer ShapeRenderer
        {
            get
            {
                if (!shapeRenderer)
                    shapeRenderer = GetComponentInChildren<SpriteShapeRenderer>();
                return shapeRenderer;
            }

        }

        public List<AnimationJoint> AnimationJoints
        {
            get
            {
                animationJoints = GetAnimationJoints();
                return animationJoints;
            }
        }

        public AnimationShape AnimationShape
        {
            get {
                if (!animationShape)
                    animationShape = GetComponentInChildren<AnimationShape>();
                return animationShape;

            }
        }

        public SpriteShapeController Controller
        {
            get
            {
                if (!controller)
                {
                    controller = GetComponentInChildren<SpriteShapeController>();
                }
                return controller;
            }
        }

        public int ShapeLayerOrder
        {
            get
            {
                shapeLayerOrder = shapeRenderer.sortingOrder;
                return shapeLayerOrder;
            }
        }

        private List<AnimationJoint> GetAnimationJoints()
        {
            var _joints = new List<AnimationJoint>();
            var i = 0;

            //This should be reworked. Joints will never be updated if the count differs.
            if (animationJoints.Count == 0 || animationJoints.Count != transform.childCount - 1)
            {
                animationJoints.Clear();
                foreach (Transform child in transform)
                {
                    var script = child.GetComponent<AnimationJoint>();
                    if (!script)
                    {
                        continue;
                    }

                    script.JointIndex = i;
                    _joints.Add(script);
                    i++;
                }
                return _joints;
            }

            return animationJoints;

        }

        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            UnregisterAll();

            animationJoints = GetAnimationJoints();

            foreach (Transform child in transform)
            {
                var subscriber = child.GetComponent<ISubscriber>();
                var subject = child.GetComponent<AnimationJoint>();

                if (subscriber != null)
                {
                    Register(subscriber);
                }

                if (subject)
                {                    
                    Register(subject);
                }
            }
        }

        #region Sync methods


        public override void SyncEventSubscribers()
        {
    #if UNITY_EDITOR
            // check if they are synced or not
            if (autoMatchToSpline)
            {
                CheckJointSynchronization();
            }
    #endif

            base.SyncEventSubscribers();
        }

        #endregion

        // do this before moving splines to points
        public void CheckJointSynchronization()
        {
            var splineCount = controller.spline.GetPointCount();
            var jointCount = animationJoints.Count;

            var unmatchedPoints = new Dictionary<int, Vector3>();

            // populate dictionary
            for (int i = 0; i < splineCount; i++)
            {
                unmatchedPoints.Add(i, controller.spline.GetPosition(i));
            }

            var iterations = unmatchedPoints.Count();

            if (splineCount > jointCount)
            {
                // Add all joints if there are none in the AnimationJoints list
                if (animationJoints.Count == 0)
                {
                    for (int i = 0; i < iterations; i++)
                    {
                        CreateNewJoint(i, unmatchedPoints[i]);
                    }

                    return;
                }

                for (int i = 0; i < iterations; i++)
                {
                    if (animationJoints.Count - 1 < i)
                    {
                        continue;
                    }

                    // Loop over points to find a match. Remove matches from the list to search.
                    if (IsJointOnAPoint(animationJoints[i], unmatchedPoints, out int index))
                    {
                        //RenameJoint(animationJoints[i], index);
                        unmatchedPoints.Remove(index);
                    }
                }

                List<int> indexes = unmatchedPoints.Keys.ToList<int>();

                for (int i = 0; i < unmatchedPoints.Count; i++)
                {
                    CreateNewJoint(indexes[i], unmatchedPoints[indexes[i]]);
                }
                
                Initialize();
            }

            // 
            else if (splineCount < jointCount)
            {
                for (int i = 0; i < animationJoints.Count; i++)
                {
                    // Loop over points to find a match. Remove matches from the list to search.
                    if (IsJointOnAPoint(animationJoints[i], unmatchedPoints, out int index))
                    {
                        unmatchedPoints.Remove(index);
                    }

                    else
                    {
                        RemoveJoint(i);
                        i--;
                    }
                }
                
                Initialize();
            }


            if (!autoRename)
            {
                return;
            }

            // Rename all joints to match their new order.
            for (int i = 0; i < animationJoints.Count; i++)
            {
                RenameJoint(animationJoints[i], i);
            }

            

        }

        private void RenameJoint(AnimationJoint joint, int index)
        {
            joint.gameObject.name = Regex.Replace(joint.gameObject.name, @"\d", string.Empty)
                                         .Replace("(Clone)", string.Empty)
                                         .Trim();

            joint.gameObject.name += $" {index}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="pointPosition"></param>
        /// <param name="pointIndex">Outputs index of matched vector3. -1 if there are no matches.</param>
        /// <returns></returns>
        public bool IsJointOnAPoint(AnimationJoint joint, Dictionary<int, Vector3> pointPosition, out int pointIndex)
        {
            var jointPosOffset = joint.transform.position - transform.position;

            foreach (var keyValuePair in pointPosition)
            {
                if (keyValuePair.Value == jointPosOffset)
                {
                    pointIndex = keyValuePair.Key;
                    return true;
                }
            }

            pointIndex = -1;
            return false;
        }

        public void CreateNewJoint(int index, Vector3 pointPosition)
        {

            var instance = new GameObject("Joint", typeof(AnimationJoint));

            instance.transform.position = pointPosition + transform.position;
            instance.transform.parent = transform;
            instance.transform.SetSiblingIndex(index + 1);

            var animJoint = instance.GetComponent<AnimationJoint>();

            RenameJoint(animJoint, index);
            animationJoints.Insert(index, animJoint);
            Register(animJoint);
        }

        /// <summary>
        /// Removes an AnimationJoint at the given index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveJoint(int index)
        {
            if (index >= animationJoints.Count)
            {
                Debug.LogWarning($"Joint index ({index}) is greater than joint count on AnimationController. Could not be removed.");
                return;
            }


            //Not always reliable, as Skin is also part of this. Index might be misleading to actual component.
            var result = transform.GetChild(index + 1).TryGetComponent<AnimationJoint>(out var child);

            if (result)
            {
                Debug.Log($"Removed joint {index} from Controller. Name was: {animationJoints[index].gameObject.name}");
                Unregister(child);
                animationJoints.RemoveAt(index);
                DestroyImmediate(child.gameObject);
            }
        }

        /// <summary>
        /// Removes all child <see cref="AnimationJoint"/>s in this object's hierarchy from the <see cref="Scene"/>.
        /// </summary>
        public void RemoveAllJoints()
        {
            if (transform.childCount <= 0)
            {
                return;
            }

            var animJoints = new List<Transform>();

            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<AnimationJoint>(out var component))
                {
                    animJoints.Add(child);
                }
            }

            // return if there are no AnimationJoints in transform
            if (animJoints.Count <= 0)
            {
                return;
            }

            // Destroy all AnimationJoints
            for (int i = animJoints.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(animJoints[i].gameObject);
                animJoints.RemoveAt(i);
            }

            animationJoints.Clear();
            UnregisterAll();
        }
    }
}
