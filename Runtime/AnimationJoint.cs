using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U2DSpriteShapeAnimator.Internals;
using System;

namespace U2DSpriteShapeAnimator.Runtime
{
    public class AnimationJoint : Subject
    {
        public AnimationJoint(EventManager _manager) : base(_manager)
        {
        }

        private Vector3 previousPosition;
        private int jointIndex;
        private SpriteRenderer spriteRenderer;

        [SerializeField] // Editor needs to access this
        [HideInInspector]
        private Vector3 position;

        public int JointIndex { get => jointIndex; set => jointIndex = value; }

        public SpriteRenderer SpriteRenderer
        {
            get
            {
                if (!spriteRenderer)
                {
                    if (TryGetComponent<SpriteRenderer>(out spriteRenderer))
                        return spriteRenderer;
                    else
                        return null;

                }
                return spriteRenderer;
            }
        }

        private void OnEnable()
        {
            SetPreviousPosition();
        }

        private void Update()
        {
            CheckJointUpdate();
        }

        /// <summary>
        /// Notifies the EventManager if there have been any changes to this subscriber.
        /// </summary>
        public void CheckJointUpdate()
        {
            //Debug.Log($"My position {transform.position} |offset {transform.position - transform.parent.position} | local {transform.localPosition}| index {jointIndex}");
            if (PositionChanged())
            {
                Notify();
                SetPreviousPosition();
            }
        }

        public override void Notify()
        {
            if (!eventManager)
                eventManager = gameObject.GetComponentInParent<EventManager>();
            base.Notify();
        }

        private bool PositionChanged()
        {
            var hasMoved = transform.localPosition != previousPosition;
            return hasMoved;

        }

        private void SetPreviousPosition()
        {
            previousPosition = transform.localPosition;
        }

    }

}