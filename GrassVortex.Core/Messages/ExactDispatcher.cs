﻿using System;
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

		private readonly List<Message> messages;

		private ExactDispatcher(DispatcherData data)
		{
			messages = new List<Message>();

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
				Dictionary<Type, List<Method>> matchingSignatures = new Dictionary<Type, List<Method>>();

				foreach (Method method in data.GetAllMethods())
				{
					if (method.Signature == signature)
					{
						var list = matchingSignatures.GetOrCreate(method.ListenerType);
						list.Add(method);
					}
				}

				IEnumerable<DistributionTarget> targets = matchingSignatures.Select(ms => new DistributionTarget(ms.Key, ms.Value));
				DistributionList distributionList = new DistributionList(signature, targets);
				distributionLists.Add(distributionList.Signature, distributionList);
			}

		}

		public void AddListener(IListener listener)
		{

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
			messages.Add(new Message(receiverSignature, payload));
		}

		public void DispatchMessages()
		{
			throw new NotImplementedException();
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