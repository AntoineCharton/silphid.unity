﻿using UniRx;
using UnityEngine;

namespace Silphid.Showzup
{
    public class InstantTransition : Transition
    {
        public static readonly ITransition Instance = new InstantTransition();
        
        public override void Prepare(GameObject sourceContainer, GameObject targetContainer, Direction direction) {}

        public override IObservable<Unit> Perform(GameObject sourceContainer, GameObject targetContainer,
            Direction direction, float duration) => Observable.ReturnUnit();
    }
}