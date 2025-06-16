using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace U2DSpriteShapeAnimator.Internals
{

    public abstract class EventManager : MonoBehaviour
    {
        private List<ISubscriber> _subscribers;        
        private List<Subject> _subjects;

        protected EventManager()
        {
            _subscribers = new List<ISubscriber>();
            _subjects = new List<Subject>();
        }

        public virtual void Register(ISubscriber sub)
        {
            _subscribers.Add(sub);
        }

        public virtual void Register(Subject subject)
        {
            _subjects.Add(subject);
            subject.SetSubjectIndex(_subjects.Count - 1);
        }

        public virtual void Unregister(ISubscriber sub)
        {
            _subscribers.Remove(sub);
        }

        public virtual void Unregister(Subject subject)
        {
            _subjects.Remove(subject);
        }

        public virtual void UnregisterAll()
        {
            _subjects.Clear();
            _subscribers.Clear();
        }

        public virtual void Notify(Subject updatedSubject, int subjectIndex)
        {
            foreach (ISubscriber sub in _subscribers)
            {
                sub.SubjectUpdated(updatedSubject, subjectIndex);
            }
        }

        public virtual void SyncEventSubscribers()
        {
            // check if they are synced or not

            foreach (Subject subject in _subjects)
            {
                subject.Notify();
            }
        }
    }
}