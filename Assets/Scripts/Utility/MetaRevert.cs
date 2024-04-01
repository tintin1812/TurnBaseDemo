using System;
using System.Collections.Generic;
using FairyGUI;

namespace Utility
{
    public class MetaRevert
    {
        private readonly List<Func<GTweener>> _wait;

        public MetaRevert(List<Func<GTweener>> wait)
        {
            _wait = wait;
        }

        public void Do(GTweenCallback onComplete)
        {
            GTweener lastTweener = null;
            foreach (var d in _wait)
            {
                var t = d();
                if (t != null)
                {
                    lastTweener = t;
                }
            }

            lastTweener?.OnComplete(onComplete);
        }
    }
}