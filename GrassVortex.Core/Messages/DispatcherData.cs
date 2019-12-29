using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassVortex.Messages
{
	/// <summary>
	/// Enables access to data about all the available listeners and the messages they can receive.
	/// Intended to be used during initialization and not during general operation of a dispatcher. As a result some 
	/// of the implemented methods are not focused on performance, but ease of use and flexibility.
	/// </summary>
	public class DispatcherData
	{
		private readonly IEnumerable<ListenerTemplate> listenerTemplates;

		internal DispatcherData(IEnumerable<ListenerTemplate> listenerTemplates)
		{
			this.listenerTemplates = listenerTemplates;
		}


		public IEnumerable<Method> GetAllMethods()
		{
			return listenerTemplates.SelectMany(l => l.Select(m => m));
		}

		public IEnumerable<Type> GetListenerTypes()
		{
			return listenerTemplates.Select(l => l.Type);
		}

		/// <summary>
		/// Returns a list of all the message types that are defined in the listeners
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Type> GetAllMessageTypes()
		{
			return listenerTemplates.SelectMany(l => l.Select(m => m.Signature.MessageType)).Distinct();
		}

		public IEnumerable<Type> GetAllPayloadTypes()
		{
			return listenerTemplates.SelectMany(l => l.Select(m => m.Signature.PayloadType)).Distinct();
		}

		public IEnumerable<MethodSignature> GetAllSignatures()
		{
			return listenerTemplates.SelectMany(l => l.Select(m => m.Signature)).Distinct();
		}

	}
}
