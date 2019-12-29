using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassVortex.Messages
{
	/// <summary>
	/// Classes implementing this interface must be able to be constructed via a default constructor
	/// </summary>
	public interface IDispatcherInstantiator
	{
		/// <summary>
		/// Returns a list of dispatcher names that this factory can instantiate
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetDispatcherNames();

		/// <summary>
		/// Instantiates the specified dispatcher.
		/// A list of all available listeners is provided.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		IDispatcher Instantiate(string name, DispatcherData data);
	}
}
