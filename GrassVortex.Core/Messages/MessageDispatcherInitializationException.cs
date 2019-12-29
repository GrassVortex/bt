using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassVortex.Messages
{

	[Serializable]
	public class MessageDispatcherInitializationException : Exception
	{
		public MessageDispatcherInitializationException() { }
		public MessageDispatcherInitializationException(string message) : base(message) { }
		public MessageDispatcherInitializationException(string message, Exception inner) : base(message, inner) { }
		protected MessageDispatcherInitializationException(
		 System.Runtime.Serialization.SerializationInfo info,
		 System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
