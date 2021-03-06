using System;
using System.Collections.Generic;

namespace Platform.Xml.Serialization
{
	public class TypeSerializerCache		
	{		
		private readonly TypeSerializerFactory typeSerializerFactory;
		private readonly Dictionary<Pair<Type, object>, TypeSerializer> cache;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeSerializerFactory"></param>
		public TypeSerializerCache(TypeSerializerFactory typeSerializerFactory)
		{
			this.typeSerializerFactory = typeSerializerFactory;
			cache = new Dictionary<Pair<Type, object>, TypeSerializer>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serializer"></param>
		public void Add(TypeSerializer serializer)
		{
			if (!serializer.MemberBound)
			{
				cache[new Pair<Type, object>(serializer.SupportedType, null)] = serializer;
			}
			else
			{
				throw new ArgumentException("Use TypeSerializerCache.Add(TypeSerializer, MemberInfo) with member bound serializers.");
			}
		}

		public virtual void Add(TypeSerializer serializer, object key)
		{
			cache[new Pair<Type, object>(serializer.SupportedType, key)] = serializer;			
		}

		public virtual void Add(TypeSerializer serializer, SerializationMemberInfo memberInfo)
		{
			if (serializer.MemberBound)
			{
				Add(serializer, (object)memberInfo);
			}
			else
			{
				Add(serializer);
			}
		}

		public TypeSerializer GetTypeSerializerBySerializerType(Type type)
		{
			var serializer = this.typeSerializerFactory.NewTypeSerializerBySerializerType(type, this);

			Add(serializer);

			return serializer;
		}

		public TypeSerializer GetTypeSerializerBySerializerType(Type type, SerializationMemberInfo memberInfo)
		{
			var serializer = typeSerializerFactory.NewTypeSerializerBySerializerType(type, memberInfo, this);

			Add(serializer, memberInfo);

			return serializer;
		}

		public TypeSerializer GetTypeSerializerBySupportedType(Type type)
		{
			TypeSerializer serializer;

			if (cache.TryGetValue(new Pair<Type, object>(type, null), out serializer))
			{
				return serializer;
			}

			serializer = typeSerializerFactory.NewTypeSerializerBySupportedType(type, this);

			Add(serializer);
			
			return serializer;
		}

		public TypeSerializer GetTypeSerializerBySupportedType(Type type, SerializationMemberInfo memberInfo)
		{
			TypeSerializer serializer;

			if (cache.TryGetValue(new Pair<Type, object>(type, null), out serializer))
			{
				return serializer;
			}

			if (cache.TryGetValue(new Pair<Type, object>(type, memberInfo), out serializer))
			{
				return serializer;
			}

			serializer = typeSerializerFactory.NewTypeSerializerBySupportedType(type, memberInfo, this);

			Add(serializer, memberInfo);
			
			return serializer;
		}
	}
}
