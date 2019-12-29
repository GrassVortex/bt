using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassVortex.Messages
{
	[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class DispatcherInstantiatorAttribute : Attribute
	{
		public DispatcherInstantiatorAttribute()
		{

		}
	}
}
