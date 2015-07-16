using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace SocketIO
{
	[MonoNativeFunctionWrapper] 
	internal delegate void ConnectionListenerDelegateInternal (IntPtr userData);

	[MonoNativeFunctionWrapper] 
	internal delegate void CloseListenerDelegateInternal (Client.CloseReason reason, IntPtr userData);

	[MonoNativeFunctionWrapper] 
	internal delegate void ReconnectListenerDelegateInternal (uint reconn_made, uint delay, IntPtr userData);

	[MonoNativeFunctionWrapper] 
	internal delegate void SocketListenerDelegateInternal (string nsp, IntPtr userData);

	[MonoNativeFunctionWrapper]
	internal delegate void EventListenerAuxDelegateInternal (string name, Message message, bool need_ack, Message ack_message, IntPtr userData);

	[MonoNativeFunctionWrapper]
	internal delegate void EventListenerDelegateInternal (SocketEvent _event, IntPtr userData);

	[MonoNativeFunctionWrapper]
	internal delegate void ErrorListenerDelegateInternal (Message message, IntPtr userData);

	[MonoNativeFunctionWrapper]
	internal delegate void AckDelegateInternal (Message.List msglist, IntPtr userData);

	internal static class Proxies
	{
		[MonoPInvokeCallback (typeof(ConnectionListenerDelegateInternal))]
		internal static void ConnectionListenerProxy (IntPtr userData)
		{
			ProxyDelegateManager.Invoke (null, userData);
		}

		[MonoPInvokeCallback (typeof(CloseListenerDelegateInternal))]
		internal static void CloseListenerProxy (Client.CloseReason reason, IntPtr userData)
		{
			ProxyDelegateManager.Invoke (new object[] { reason }, userData);
		}

		[MonoPInvokeCallback (typeof(ReconnectListenerDelegateInternal))]
		internal static void ReconnectListenerProxy (uint reconn_made, uint delay, IntPtr userData)
		{
			ProxyDelegateManager.Invoke (new object[] { reconn_made, delay }, userData);
		}

		[MonoPInvokeCallback (typeof(SocketListenerDelegateInternal))]
		internal static void SocketListenerProxy (string nsp, IntPtr userData)
		{
			ProxyDelegateManager.Invoke (new object[] { nsp }, userData);
		}

		[MonoPInvokeCallback (typeof(EventListenerAuxDelegateInternal))]
		internal static void EventListenerAuxProxy (string name, Message message, bool need_ack, Message ack_message, IntPtr userData)
		{
			ProxyDelegateManager.Invoke (new object[] { name, message, need_ack, ack_message }, userData);
		}

		[MonoPInvokeCallback (typeof(EventListenerDelegateInternal))]
		internal static void EventListenerProxy (SocketEvent _event, IntPtr userData)
		{
			ProxyDelegateManager.Invoke (new object[] { _event }, userData);
		}

		[MonoPInvokeCallback (typeof(ErrorListenerDelegateInternal))]
		internal static void ErrorListenerProxy (Message message, IntPtr userData)
		{
			ProxyDelegateManager.Invoke (new object[] { message }, userData);
		}

		[MonoPInvokeCallback (typeof(AckDelegateInternal))]
		internal static void AckProxy (Message.List msglist, IntPtr userData)
		{
			ProxyDelegateManager.Invoke (new object[] { msglist }, userData);
		}
	}

	internal class ProxyDelegateManager
	{
		// prevent the GC from collecting the managed part of the callbacks
		private readonly IDictionary<string, GCHandle> functions = new Dictionary<string, GCHandle> ();

		// make sure we release any memory
		internal void ReleaseFunctions ()
		{
			foreach (var guid in functions.Values) {
				guid.Free ();
			}
		}

		// execute the real delegates
		internal static void Invoke (object[] parameters, IntPtr userData)
		{
			// get the function
			var guidHandle = GCHandle.FromIntPtr (userData);
			var proxy = (Delegate)guidHandle.Target;

			// Invoke it
			proxy.Method.Invoke (proxy.Target, parameters);
		}

		// wrap the delegate for the proxy
		internal IntPtr Register (string funcName, Delegate func)
		{
			// create a handle
			var handle = GCHandle.Alloc (func);

			// store the handle
			GCHandle oldGuid;
			if (functions.TryGetValue (funcName, out oldGuid)) {
				// free an old handle (only one of each)
				oldGuid.Free ();
				functions [funcName] = handle;
			} else {
				functions.Add (funcName, handle);
			}

			// return the handle for native functions
			var userData = GCHandle.ToIntPtr (handle);
			return userData;
		}
	}
	
}

//		private struct Sqlite3FunctionMarshallingProxy
//		{
//			// functions
//			private readonly SocketListenerDelegate invoke;
//
//			private readonly Guid guid;
//			private readonly GCHandle guidHandle;
//
//			internal Sqlite3FunctionMarshallingProxy (string functionName, SocketListenerDelegate invoke)
//			{
//				this.invoke = invoke;
//
//				this.guid = Guid.NewGuid ();
//				this.guidHandle = GCHandle.Alloc (this.guid, GCHandleType.Pinned);
//
//				this.RegisterInstance (functionName);
//			}
//
//			internal IntPtr GuidHandlePtr {
//				get {
//					var ptr = GCHandle.ToIntPtr (this.guidHandle);
//					return ptr;
//				}
//			}
//
//			[MonoPInvokeCallback (typeof(SocketListenerDelegateInternal))]
//			public static void SocketListenerProxy (string nsp, IntPtr userData)
//			{
//				var guidHandle = GCHandle.FromIntPtr (userData);
//				var proxy = Sqlite3FunctionMarshallingProxy.instanceByGuidDic [(Guid)guidHandle.Target];
//
//				// Invoke it
//				proxy.invoke (nsp);
//			}
//
//			private static IDictionary<Guid, Sqlite3FunctionMarshallingProxy> instanceByGuidDic = new Dictionary<Guid, Sqlite3FunctionMarshallingProxy> ();
//			private static IDictionary<string, Guid> guidByFunc = new Dictionary<string, Guid> ();
//
//			internal void ReleaseProxies ()
//			{
//				foreach (var guid in guidByFunc.Values) {
//					instanceByGuidDic [guid].guidHandle.Free ();
//					instanceByGuidDic.Remove (guid);
//				}
//			}
//
//			private void RegisterInstance (string funcName)
//			{
//				Guid oldGuid;
//				if (guidByFunc.TryGetValue (funcName, out oldGuid)) {
//					instanceByGuidDic [oldGuid].guidHandle.Free ();
//					instanceByGuidDic.Remove (oldGuid);
//					guidByFunc [funcName] = this.guid;
//				} else {
//					guidByFunc.Add (funcName, this.guid);
//				}
//
//				instanceByGuidDic.Add (this.guid, this);
//			}
//		}
