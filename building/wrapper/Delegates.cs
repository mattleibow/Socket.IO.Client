using System.Runtime.InteropServices;

namespace SocketIO.Internal
{
	public delegate void con_listener();

	public delegate void close_listener(client.close_reason reason);

	public delegate void reconnect_listener(uint reconn_made, uint delay);

	public delegate void socket_listener(string nsp);

	public delegate void event_listener_aux(string name, message message, bool need_ack, message ack_message);

	public delegate void event_listener(_event _event);

	public delegate void error_listener(message message);

	public delegate void ack_delegate(message.list msglist);
}
