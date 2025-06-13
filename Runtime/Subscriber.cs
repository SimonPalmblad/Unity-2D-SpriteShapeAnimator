using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U2DSpriteShapeAnimator.Internals
{
	public interface ISubscriber
	{
		public abstract void SubjectUpdated(Subject theChangedSubject, int subjectIndex);
	} 
}


