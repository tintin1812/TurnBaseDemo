using FairyGUI;
using UnityEngine;

namespace Common
{
    public class DragGesture : EventDispatcher
    {
        private GObject host { get; set; }
        private bool _started;
        private int _touchId;
        private Vector2 _posStart;

        public DragGesture(GObject host)
        {
            this.host = host;
            Enable(true);
        }

        public void Dispose()
        {
            Enable(false);
            host = null;
        }

        public void Enable(bool value)
        {
            if (value)
            {
                if (host == GRoot.inst)
                {
                    Stage.inst.onTouchBegin.Add(__touchBegin);
                    Stage.inst.onTouchMove.Add(__touchMove);
                    Stage.inst.onTouchEnd.Add(__touchEnd);
                }
                else
                {
                    host.onTouchBegin.Add(__touchBegin);
                    host.onTouchMove.Add(__touchMove);
                    host.onTouchEnd.Add(__touchEnd);
                }
            }
            else
            {
                _started = false;
                if (host == GRoot.inst)
                {
                    Stage.inst.onTouchBegin.Remove(__touchBegin);
                    Stage.inst.onTouchMove.Remove(__touchMove);
                    Stage.inst.onTouchEnd.Remove(__touchEnd);
                }
                else
                {
                    host.onTouchBegin.Remove(__touchBegin);
                    host.onTouchMove.Remove(__touchMove);
                    host.onTouchEnd.Remove(__touchEnd);
                }
            }
        }

        void __touchBegin(EventContext context)
        {
            if (_started)
            {
                return;
            }

            _started = true;
            _touchId = context.inputEvent.touchId;
            context.CaptureTouch();
            _posStart = context.inputEvent.position;
        }

        void __touchMove(EventContext context)
        {
            if (!_started) return;
            if (_touchId != context.inputEvent.touchId) return;
            var pt = context.inputEvent.position;
            var pDrag = pt - _posStart;
            _posStart = pt;
            // host.xy += host.GlobalToLocal(pDrag);
            host.xy += pDrag;
        }

        void __touchEnd(EventContext context)
        {
            if (!_started) return;
            if (_touchId != context.inputEvent.touchId) return;
            if (_started)
            {
                _started = false;
            }
        }
    }
}