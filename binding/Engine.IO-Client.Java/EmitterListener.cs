using System;

namespace EngineIO
{
    public class EmitterListener : Java.Lang.Object, Emitter.IListener
    {
        private readonly Action<Java.Lang.Object[]> listener;

        public EmitterListener(Action<Java.Lang.Object[]> listener)
        {
            this.listener = listener;
        }

        void Emitter.IListener.Call(params Java.Lang.Object[] arguments)
        {
            var handler = listener;
            if (handler != null)
            {
                handler(arguments);
            }
        }
    }
}