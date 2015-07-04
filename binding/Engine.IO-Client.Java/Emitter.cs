using System;

namespace EngineIO
{
    partial class Emitter
    {
        public Emitter On(string eventName, Action<Java.Lang.Object[]> listener)
        {
            return On(eventName, new EmitterListener(listener));
        }

        public Emitter Once(string eventName, Action<Java.Lang.Object[]> listener)
        {
            return Once(eventName, new EmitterListener(listener));
        }

        public Emitter Off(string eventName, Action<Java.Lang.Object[]> listener)
        {
            return Off(eventName, new EmitterListener(listener));
        }
    }
}