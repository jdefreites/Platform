using System;
using System.Collections.Generic;

namespace Platform.Collections
{
	/// <summary>
	/// An queue implementation that uses an array as a store.
	/// </summary>
	public class ArrayQueue<T>
		: QueueBase<T>
	{
		protected const int DefaultInitialCapacity = 16;

		private int count;
		private T[] array;
		private int startIndex;

		public ArrayQueue()
			: this(DefaultInitialCapacity)
		{
		}

		public ArrayQueue(int initialCapacity)
		{
			count = 0;
			array = new T[initialCapacity];
		}

		private void Grow()
		{
			int newLength;
			var oldArray = array;

			if (count == Int32.MaxValue)
			{
				throw new OutOfMemoryException();
			}

			if (oldArray.Length >= Int32.MaxValue / 2)
			{
				newLength = Int32.MaxValue;
			}
			else
			{
				newLength = oldArray.Length << 1;
			}

			array = new T[newLength];

			var y = 0;

			for (var i = startIndex; i < startIndex + count; i++)
			{
				var x = i % oldArray.Length;

				array[y++] = oldArray[x];
			}

			startIndex = 0;
		}

		public override void Enqueue(T item)
		{
		    if (count >= array.Length)
			{
				Grow();
			}

			var index = (startIndex + count) % array.Length;

			array[index] = item;
			count++;
		}

		public override bool TryDequeue(out T value)
		{
			if (count == 0)
			{
				value = default(T);

				return false;
			}

			value = array[startIndex];

			startIndex++;
			startIndex %= array.Length;
			count--;

			return true;
		}

		public override bool TryPeek(out T value)
		{
			if (count == 0)
			{
				value = default(T);

				return false;
			}

			value = array[startIndex];

			return true;
		}

		public override int Count
		{
			get
			{
				return count;
			}
		}
		
		public override IEnumerator<T> GetEnumerator()
		{
		    for (var i = startIndex; i < startIndex + count; i++)
			{
				yield return array[i];
			}
		}

		public override void Clear()
		{
		    count = 0;

			var x = (startIndex + count) % array.Length;

			if (x < startIndex)
			{
				Array.Clear(array, startIndex, array.Length - startIndex);
				Array.Clear(array, 0, x + 1);
			}
			else
			{
				Array.Clear(array, startIndex, count);
			}
		}
	}
}
