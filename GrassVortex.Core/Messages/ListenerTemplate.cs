using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassVortex.Messages
{
	public class ListenerTemplate : IEnumerable<Method>
	{
		private Method[] methods;
		public ListenerTemplate(Type listenerType, IEnumerable<Method> methods)
		{
			this.Type = listenerType;
			this.methods = methods.ToArray();
		}

		/// <summary>
		/// The type of the listener. 
		/// </summary>
		public Type Type { get; }

		public IEnumerator<Method> GetEnumerator()
		{
			foreach (var m in methods)
			{
				yield return m;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
