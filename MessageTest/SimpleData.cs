using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrassVortex.Messages;

namespace MessageTest
{
	class SimpleData : IMessagePayload
	{
		public string Text { get; set; }
	}
}
