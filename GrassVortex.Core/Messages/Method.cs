using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GrassVortex.Messages
{
	public sealed class Method
	{
		private const BindingFlags ReceiverMethodBindings = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

#pragma warning disable S3246 // Generic type parameters should be co/contravariant when possible
		// We are only using this delegate internally, no need for co/contravariant
		private delegate void ListenerMethod<TListener, TMessagePayload>(TListener listener, Type type, TMessagePayload payload)
			where TListener : class, IListener
			where TMessagePayload : class, IMessagePayload;
#pragma warning restore S3246 // Generic type parameters should be co/contravariant when possible

		private readonly IMethodCaller caller;


		private Method(Type listenerType, MethodSignature signature, IMethodCaller caller)
		{
			this.ListenerType = listenerType;
			this.Signature = signature;
			this.caller = caller;
		}

		public Type ListenerType { get; }
		public MethodSignature Signature { get; private set; }

		internal static IEnumerable<Method> GetMethods(Type listenerType)
		{
			MethodInfo[] methods = listenerType.GetMethods(ReceiverMethodBindings);
			foreach (MethodInfo method in methods)
			{
				var messageAttributes = method.GetCustomAttributes<MessageMethodAttribute>();
				if (!messageAttributes.Any())
				{
					continue;
				}

				ParameterInfo[] parameters = method.GetParameters();
				if (method.ReturnType != typeof(void)
					|| parameters.Length != 2
					|| parameters[0].ParameterType != typeof(Type))
				{
					throw new MessageDispatcherInitializationException($"Method {method.Name} in type {listenerType.FullName} have an invalid signature.");
				}

				// Create type and delegate objects for the MethodCaller instance
				Type[] genericTypeArguments = new Type[] { listenerType, parameters[1].ParameterType };
				Type wrapperType = typeof(MethodWrapper<,>).MakeGenericType(genericTypeArguments);
				Type listenerMethodDelegateType = typeof(ListenerMethod<,>).MakeGenericType(genericTypeArguments);
				Delegate listenerMethodDelegate = Delegate.CreateDelegate(listenerMethodDelegateType, null, method);

				foreach (var attribute in messageAttributes)
				{
					MethodSignature signature = new MethodSignature(attribute.MessageType, parameters[1].ParameterType);
					var constructorArguments = new object[] { signature.MessageType, listenerMethodDelegate };
					IMethodCaller methodCaller = (IMethodCaller)Activator.CreateInstance(wrapperType, constructorArguments);

					yield return new Method(listenerType, signature, methodCaller);
				}
			}
		}

		/// <summary>
		/// Calls the listener with the specified payload.
		/// </summary>
		/// <param name="listener"></param>
		/// <param name="payload"></param>
		public void Call(IListener listener, IMessagePayload payload)
		{
			caller.Call(listener, payload);
		}

		private interface IMethodCaller
		{
			void Call(IListener listener, IMessagePayload payload);
		}

		private sealed class MethodWrapper<TListener, TMessagePayload> : IMethodCaller
			where TListener : class, IListener
			where TMessagePayload : class, IMessagePayload
		{
			private readonly Type messageType;
			private readonly ListenerMethod<TListener, TMessagePayload> method;

#pragma warning disable S1144 // Unused private types or members should be removed
			// This constructor is actually in use, but only via reflection
			public MethodWrapper(Type messageType, ListenerMethod<TListener, TMessagePayload> method)
			{
				this.messageType = messageType;
				this.method = method;
			}
#pragma warning restore S1144 // Unused private types or members should be removed

			public void Call(IListener listener, IMessagePayload payload)
			{
				method((TListener)listener, messageType, (TMessagePayload)payload);
			}
		}
	}
}
