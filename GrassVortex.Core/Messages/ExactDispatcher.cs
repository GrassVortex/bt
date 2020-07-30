using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrassVortex.Utility;

namespace GrassVortex.Messages
{
	public class ExactDispatcher : IDispatcher
	{
		private readonly Dictionary<MethodSignature, DistributionList> distributionLists;
		private readonly Dictionary<Type, List<IListener>> activeListeners;

		private Queue<Message> messageQueue;
		private int messageQueueIndex;
		private readonly Queue<Message>[] messageQueuePool;

		private ExactDispatcher(DispatcherData data)
		{
			messageQueuePool = new Queue<Message>[2];
			messageQueuePool[0] = new Queue<Message>();
			messageQueuePool[1] = new Queue<Message>();
			messageQueueIndex = 0;
			SwapMessageQueues();

			distributionLists = new Dictionary<MethodSignature, DistributionList>();
			activeListeners = new Dictionary<Type, List<IListener>>();

			SetupDistributionLists(data);
			SetupListeners(data);
		}

		private void SetupListeners(DispatcherData data)
		{

			foreach (var listenerType in data.GetListenerTypes())
			{
				activeListeners.Add(listenerType, new List<IListener>());
			}
		}

		private void SetupDistributionLists(DispatcherData data)
		{
			foreach (MethodSignature signature in data.GetAllSignatures())
			{
				// Find all the methods that match the current signature 
				Dictionary<Type, List<Method>> matchingSignatures = new Dictionary<Type, List<Method>>();
				foreach (Method method in data.GetAllMethods())
				{
					if (method.Signature == signature)
					{
						var list = matchingSignatures.GetOrCreate(method.ListenerType);
						list.Add(method);
					}
				}

				// For the current signature, create a list of all the listener types that can be receiving this kind of message
				IEnumerable<DistributionTarget> targets = matchingSignatures.Select(ms => new DistributionTarget(ms.Key, ms.Value));
				DistributionList distributionList = new DistributionList(signature, targets);
				distributionLists.Add(distributionList.Signature, distributionList);
			}

		}

		public void AddListener(IListener listener)
		{
			Type listenerType = listener.GetType();

		}

		public void Send(IMessageType type, IMessagePayload payload)
		{
			Send(new MethodSignature(type.GetType(), payload.GetType()), payload);
		}

		public void SendExplicit<TMessageType, TMessagePayload>(TMessageType type, TMessagePayload payload)
			where TMessageType : class, IMessageType
			where TMessagePayload : class, IMessagePayload
		{
			Send(new MethodSignature(typeof(TMessageType), typeof(TMessagePayload)), payload);
		}

		private void Send(MethodSignature receiverSignature, IMessagePayload payload)
		{
			messageQueue.Enqueue(new Message(receiverSignature, payload));
		}

		public void DispatchMessages()
		{
			// Save the current queue and swap to the next one so that new messages can be enqueued as a result of a dispatch
			var messagesToDispatch = messageQueue;
			SwapMessageQueues();

			try
			{

			}
			finally
			{
				messagesToDispatch.Clear();
			}
		}

		private void SwapMessageQueues()
		{
			++messageQueueIndex;
			if (messageQueueIndex >= messageQueuePool.Length)
			{
				messageQueueIndex = 0;
			}
			messageQueue = messageQueuePool[messageQueueIndex];
		}

		private struct Message
		{
			public Message(MethodSignature signature, IMessagePayload payload)
			{
				this.Signature = signature;
				this.Payload = payload;
			}

			public MethodSignature Signature { get; }
			public IMessagePayload Payload { get; }
		}

		/// <summary>
		/// Keeps track of all the possible listeners of a method signature
		/// </summary>
		private class DistributionList : IEnumerable<DistributionTarget>
		{
			public DistributionList(MethodSignature signature, IEnumerable<DistributionTarget> targets)
			{
				this.Signature = signature;
				this.Targets = targets.ToArray();
			}

			public MethodSignature Signature { get; }
			public IEnumerable<DistributionTarget> Targets { get; }

			public IEnumerator<DistributionTarget> GetEnumerator()
			{
				return Targets.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		private class DistributionTarget
		{
			public DistributionTarget(Type listenerType, IEnumerable<Method> methods)
			{
				this.ListenerType = listenerType;
				this.Methods = methods.ToArray();
			}

			public Type ListenerType { get; }

			public IEnumerable<Method> Methods { get; }
		}

		[DispatcherInstantiator]
		private class Factory : IDispatcherInstantiator
		{
			public Factory()
			{

			}

			public IEnumerable<string> GetDispatcherNames()
			{
				yield return typeof(ExactDispatcher).FullName;
			}

			public IDispatcher Instantiate(string name, DispatcherData data)
			{
				if (name != typeof(ExactDispatcher).FullName)
				{
					throw new ArgumentException("Unsupported dispatcher name.");
				}

				return new ExactDispatcher(data);
			}
		}
	}

}
