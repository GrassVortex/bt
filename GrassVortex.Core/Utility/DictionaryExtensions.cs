using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassVortex.Utility
{
	public static class DictionaryExtensions
	{
		public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
			where TValue : new()
		{
			TValue value;
			if (dictionary.TryGetValue(key, out value))
			{
				return value;
			}
			else
			{
				value = new TValue();
				dictionary.Add(key, value);
				return value;
			}
		}
	}
}
