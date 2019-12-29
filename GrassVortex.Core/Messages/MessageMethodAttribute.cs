using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassVortex.Messages
{
	[System.AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class MessageMethodAttribute : Attribute
	{
		public MessageMethodAttribute(Type messageType)
		{
			this.MessageType = messageType;
		}

		public Type MessageType { get; private set; }

	}
}
