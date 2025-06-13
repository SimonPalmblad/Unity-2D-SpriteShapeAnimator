using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace U2DSpriteShapeAnimator.Internals
{
    public abstract class Subject : MonoBehaviour
    {
        protected EventManager eventManager;
        protected int subjectIndex;

        public Subject(EventManager _manager = null)
        {
            if (!_manager)
                eventManager = gameObject.GetComponentInParent<EventManager>();
        }

        public virtual void SetSubjectIndex(int index)
        {
            subjectIndex = index;
        }

        public virtual void Notify()
        {
            if (eventManager)
            {
                eventManager.Notify(this, subjectIndex);
            }
        }
    } 
}
