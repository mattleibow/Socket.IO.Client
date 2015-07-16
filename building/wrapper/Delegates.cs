namespace SocketIO
{
	public delegate void ConnectionListenerDelegate ();
	public delegate void CloseListenerDelegate (Client.CloseReason reason);
	public delegate void ReconnectListenerDelegate (uint reconn_made, uint delay);
	public delegate void SocketListenerDelegate (string nsp);
	public delegate void EventListenerAuxDelegate (string name, Message message, bool need_ack, Message ack_message);
	public delegate void EventListenerDelegate (SocketEvent _event);
	public delegate void ErrorListenerDelegate (Message message);
	public delegate void AckDelegate (Message.List msglist);

	partial class Client
	{
		private readonly ProxyDelegateManager delegateManager = new ProxyDelegateManager ();

		public void SetConnectionOpenListener (ConnectionListenerDelegate l)
		{
			SetConnectionOpenListener (Proxies.ConnectionListenerProxy, delegateManager.Register ("SetConnectionOpenListener", l));
		}

		public void SetConnectionFailListener (ConnectionListenerDelegate l)
		{
			SetConnectionFailListener (Proxies.ConnectionListenerProxy, delegateManager.Register ("SetConnectionFailListener", l));
		}

		public void SetReconnectingListener (ConnectionListenerDelegate l)
		{
			SetReconnectingListener (Proxies.ConnectionListenerProxy, delegateManager.Register ("SetReconnectingListener", l));
		}

		public void SetReconnectListener (ReconnectListenerDelegate l)
		{
			SetReconnectListener (Proxies.ReconnectListenerProxy, delegateManager.Register ("SetReconnectListener", l));
		}

		public void SetConnectionCloseListener (CloseListenerDelegate l)
		{
			SetConnectionCloseListener (Proxies.CloseListenerProxy, delegateManager.Register ("SetConnectionCloseListener", l));
		}

		public void SetSocketOpenListener (SocketListenerDelegate l)
		{
			SetSocketOpenListener (Proxies.SocketListenerProxy, delegateManager.Register ("SetSocketOpenListener", l));
		}

		public void SetSocketCloseListener (SocketListenerDelegate l)
		{
			SetSocketCloseListener (Proxies.SocketListenerProxy, delegateManager.Register ("SetSocketCloseListener", l));
		}
	}

	partial class Socket
	{
//		public void SetSocketCloseListener (SocketListenerDelegate l)
//		{
//			SetSocketCloseListener (Proxies.SocketListenerProxy, delegateManager.Register ("SetSocketCloseListener", l));
//		}
//
//		public void SetSocketCloseListener (SocketListenerDelegate l)
//		{
//			SetSocketCloseListener (Proxies.SocketListenerProxy, delegateManager.Register ("SetSocketCloseListener", l));
//		}
//
//		public void SetSocketCloseListener (SocketListenerDelegate l)
//		{
//			SetSocketCloseListener (Proxies.SocketListenerProxy, delegateManager.Register ("SetSocketCloseListener", l));
//		}
//
//		public void SetSocketCloseListener (SocketListenerDelegate l)
//		{
//			SetSocketCloseListener (Proxies.SocketListenerProxy, delegateManager.Register ("SetSocketCloseListener", l));
//		}

//		void On(std::string const& event_name, EventListenerDelegateInternal const& func, void* userdata) {
//			$self->on(event_name, [func, userdata](sio::event& event){
//				func(event, userdata);
//			});
//		}
//		void On(std::string const& event_name, EventListenerAuxDelegateInternal const& func, void* userdata) {
//			$self->on(event_name, [func, userdata](const std::string& name, sio::message::ptr const& message, bool need_ack, sio::message::ptr& ack_message){
//				func(name, message, need_ack, ack_message, userdata);
//			});
//		}
//		void OnError(ErrorListenerDelegateInternal const& func, void* userdata) {
//			$self->on_error([func, userdata](sio::message::ptr const& message){
//				func(message, userdata);
//			});
//		}
//		void Emit(std::string const& name, sio::message::list const& msglist, AckDelegateInternal const& ack, void* userdata) {
//			$self->emit(name, msglist, [ack, userdata](sio::message::list const& message_list){
//				ack(message_list, userdata);
//			});
//		}
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
