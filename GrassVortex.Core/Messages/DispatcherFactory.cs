using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GrassVortex.Messages
{
	public class DispatcherFactory
	{
		private readonly DispatcherData mainData;
		private readonly Dictionary<string, IDispatcherInstantiator> factories;

		/// <summary>
		/// Creates a factory that can create IDispatcher instances.
		/// The dispatcher and receiver assembly lists can have any amount of, or no, overlap.
		/// </summary>
		/// <remarks>The dispatchers created by a factory can only handle receivers that are defined in any of the specified assemblies.</remarks>
		/// <param name="dispatcherAssemblies">A list of assemblies that might contain dispatchers and their factories.</param>
		/// <param name="receiverAssemblies">A list of assemblies that might contain receivers.</param>
		public DispatcherFactory(IEnumerable<Assembly> dispatcherAssemblies, IEnumerable<Assembly> receiverAssemblies)
		{
			this.factories = new Dictionary<string, IDispatcherInstantiator>(StringComparer.InvariantCulture);

			List<IEnumerable<ListenerTemplate>> templates = new List<IEnumerable<ListenerTemplate>>();
			foreach (var asm in receiverAssemblies)
			{
				var listenerTypes = asm.GetTypes().Where(t => typeof(IListener).IsAssignableFrom(t));
				var list = LoadListenerTemplates(listenerTypes);
				templates.Add(list);
			}
			this.mainData = new DispatcherData(templates.SelectMany(t => t));


			foreach (var asm in dispatcherAssemblies)
			{
				var factoryTypes = asm.GetTypes().Where(t => typeof(IDispatcherInstantiator).IsAssignableFrom(t) && t.GetCustomAttribute<DispatcherInstantiatorAttribute>() != null);
				LoadFactoryTemplates(factoryTypes);
			}
		}

		private IEnumerable<ListenerTemplate> LoadListenerTemplates(IEnumerable<Type> listenerTypes)
		{
			foreach (Type listenerType in listenerTypes)
			{
				var methods = Method.GetMethods(listenerType);
				var template = new ListenerTemplate(listenerType, methods);
				yield return template;
			}
		}

		private void LoadFactoryTemplates(IEnumerable<Type> factoryTypes)
		{
			foreach (Type factoryType in factoryTypes)
			{
				try
				{
					IDispatcherInstantiator factory = (IDispatcherInstantiator)Activator.CreateInstance(factoryType);
					foreach (string name in factory.GetDispatcherNames())
					{
						factories.Add(name, factory);
					}
				}
				catch (Exception e)
				{
					throw new MessageDispatcherInitializationException($"Exception when initializing dispatcher factory of type {factoryType.AssemblyQualifiedName}.", e);
				}
			}
		}

		/// <summary>
		/// Instantiates a dispatcher of the specified type. 
		/// The dispatchers that are available are determined by the arguments to the constructor of the factory.
		/// </summary>
		/// <param name="dispatcherName"></param>
		/// <returns></returns>
		public IDispatcher InstantiateDispatcher(string dispatcherName)
		{
			if (factories.ContainsKey(dispatcherName))
			{
				throw new ArgumentException($"No factory found for dispatcher '{dispatcherName}'.", nameof(dispatcherName));
			}

			IDispatcherInstantiator factory = factories[dispatcherName];
			return factory.Instantiate(dispatcherName, mainData);
		}

	}
}
