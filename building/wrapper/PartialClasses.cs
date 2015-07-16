using System;
using System.Collections.Generic;

namespace SocketIO
{
	partial class MessageList : IList<Message>
	{
		public int IndexOf (Message item)
		{
			for (int i = 0; i < Count; i++) {
				if (this [i] == item) {
					return i;
				}
			}
			return -1;
		}

		public bool Contains (Message item)
		{
			return IndexOf (item) != -1;
		}

		public bool Remove (Message item)
		{
			int index = IndexOf (item);
			if (index >= 0) {
				RemoveAt (index);
				return true;
			}
			return false;
		}

		public void AddRange (IEnumerable<Message> messages)
		{
			foreach (var message in messages) {
				Add (message);
			}
		}
	}

	partial class Message
	{
		public static Message CreateEmptyArray ()
		{
			return ArrayMessage.Create ();
		}

		public static Message CreateEmptyObject ()
		{
			return ObjectMessage.Create ();
		}

		public static Message Create (IEnumerable<Message> messages)
		{
			var message = ArrayMessage.Create ();
			message.GetArray ().AddRange (messages);
			return message;
		}

		public static Message Create (double v)
		{
			return DoubleMessage.Create (v);
		}

		public static Message Create (long v)
		{
			return IntMessage.Create (v);
		}

		public static Message Create (IDictionary<string, Message> objectValue)
		{
			var message = ObjectMessage.Create ();
			foreach (var item in objectValue) {
				message.GetObject ().Add (item.Key, item.Value);
			}
			return message;
		}

		public static Message Create (string v)
		{
			return StringMessage.Create (v);
		}

		partial class List
		{
			public Message this [uint index] {
				get { return GetItem (index); }
			}

			public IEnumerable<Message> Messages {
				get { 
					for (uint i = 0; i < Count; i++) {
						yield return GetItem (i);
					}
				}
			}
		}
	}

	partial class Socket
	{
		// map messages to a list

		public void Emit (string name, Message message)
		{
			Emit (name, new Message.List (message));
		}

		// map message values to message

		public void Emit (string name, int intValue)
		{
			Emit (name, Message.Create (intValue));
		}

		public void Emit (string name, double doubleValue)
		{
			Emit (name, Message.Create (doubleValue));
		}

		public void Emit (string name, string stringValue)
		{
			Emit (name, Message.Create (stringValue));
		}

		public void Emit (string name, IEnumerable<Message> arrayValue)
		{
			Emit (name, Message.Create (arrayValue));
		}

		public void Emit (string name, IDictionary<string, Message> objectValue)
		{
			Emit (name, Message.Create (objectValue));
		}
	}
}

