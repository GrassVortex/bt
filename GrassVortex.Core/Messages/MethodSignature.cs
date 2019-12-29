using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassVortex.Messages
{
	public struct MethodSignature : IEquatable<MethodSignature>
	{
		public MethodSignature(Type messageType, Type payloadType)
		{
			VerifyType(messageType, nameof(messageType), typeof(IMessageType));
			VerifyType(payloadType, nameof(payloadType), typeof(IMessagePayload));

			this.MessageType = messageType;
			this.PayloadType = payloadType;
		}

		public Type MessageType { get; private set; }

		public Type PayloadType { get; private set; }

		private static void VerifyType(Type type, string argumentName, Type baseInterfaceType)
		{
			if (type == null)
			{
				throw new ArgumentNullException(argumentName);
			}
			if (!(type.IsInterface || (type.IsClass && baseInterfaceType.IsAssignableFrom(type))))
			{
				throw new ArgumentException($"Types must be interfaces or classes.", argumentName);
			}
		}

		public bool Equals(MethodSignature other)
		{
			return MessageType == other.MessageType && PayloadType == other.PayloadType;
		}

		public override bool Equals(object obj)
		{
			return obj is MethodSignature && Equals((MethodSignature)obj);
		}

		public override int GetHashCode()
		{
			return unchecked(MessageType.GetHashCode() + PayloadType.GetHashCode());
		}

		public override string ToString()
		{
			return $"[Type: {MessageType.FullName} Payload: {PayloadType.FullName}]";
		}

		public static bool operator ==(MethodSignature x, MethodSignature y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(MethodSignature x, MethodSignature y)
		{
			return !x.Equals(y);
		}
	}
}
