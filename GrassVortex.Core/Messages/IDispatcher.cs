using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassVortex.Messages
{
	public interface IDispatcher
	{
		void AddListener(IListener listener);

		//void RemoveListener(IListener listener);

		void Send(IMessageType type, IMessagePayload payload);

		void SendExplicit<TMessageType, TMessagePayload>(TMessageType type, TMessagePayload payload)
			where TMessageType : class, IMessageType
			where TMessagePayload : class, IMessagePayload;

		/// <summary>
		/// Sends all the messages that have arrived since the last dispatch
		/// </summary>
		void DispatchMessages();

		//void ImmediateSend(IMessageType type, IMessagePayload payload);

		//void ImmediateSendExplicit<TMessageType, TMessagePayload>(TMessageType type, TMessagePayload payload)
		//	where TMessageType : class, IMessageType
		//	where TMessagePayload : class, IMessagePayload;

	}
}
