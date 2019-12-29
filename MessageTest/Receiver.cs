using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrassVortex;
using GrassVortex.Messages;

namespace MessageTest
{
	class Receiver : IListener
	{
		[MessageMethod(typeof(MessageA))]
		private void Message(Type type, SimpleData data)
		{

		}

		[MessageMethod(typeof(MessageA))]
		private void Message(Type type, PositionalData data)
		{

		}

		[MessageMethod(typeof(MessageB))]
		private void MessageB(Type type, SimpleData data)
		{

		}

		[MessageMethod(typeof(MessageB))]
		private void MessageB(Type type, PositionalData data)
		{

		}

		[MessageMethod(typeof(MessageA))]
		[MessageMethod(typeof(MessageB))]
		private void MixedMessages(Type type, SimpleData data)
		{

		}
	}
}
