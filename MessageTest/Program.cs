using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GrassVortex.Messages;

namespace MessageTest
{
	class Program
	{
		static void Main(string[] args)
		{

			//dispatcher.SendMessage<MessageA>(data);

			IEnumerable<Assembly> dispatcherAssemblies = new Assembly[] { Assembly.GetAssembly(typeof(DispatcherFactory)) };
			IEnumerable<Assembly> receiverAssemblies = new Assembly[] { Assembly.GetExecutingAssembly() };

			var factory = new DispatcherFactory(dispatcherAssemblies, receiverAssemblies);
		}
	}
}
