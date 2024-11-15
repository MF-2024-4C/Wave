namespace Quantum.FF
{
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;

	public static class ListPool<T>
	{
		// CONSTANTS

		private const int THREAD_COUNT  = 4;
		private const int POOL_CAPACITY = 4;
		private const int LIST_CAPACITY = 16;

		// PRIVATE MEMBERS

		private static List<List<T>>   _pool          = new List<List<T>>(POOL_CAPACITY);
		private static List<List<T>>[] _threadedPools = new List<List<T>>[THREAD_COUNT];

		static ListPool()
		{
			for (int idx = 0; idx < THREAD_COUNT; idx++)
			{
				_threadedPools[idx] = new List<List<T>>(POOL_CAPACITY);
			}
		}

		// PUBLIC METHODS

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> Get(int capacity)
		{
			lock (_pool)
			{
				int poolCount = _pool.Count;

				if (poolCount == 0)
				{
					return new List<T>(capacity > 0 ? capacity : LIST_CAPACITY);
				}

				for (int i = 0; i < poolCount; ++i)
				{
					List<T> list = _pool[i];

					if (list.Capacity < capacity)
						continue;

					_pool.RemoveWithSwap(i);
					return list;
				}

				int lastListIndex = poolCount - 1;

				List<T> lastList = _pool[lastListIndex];
				lastList.Capacity = capacity;

				_pool.RemoveAt(lastListIndex);

				return lastList;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Return(List<T> list)
		{
			if (list == null)
				return;

			list.Clear();

			lock (_pool)
			{
				_pool.Add(list);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> Get(int capacity, int threadIndex)
		{
			var pool      = _threadedPools[threadIndex];
			int poolCount = pool.Count;

			if (poolCount == 0)
			{
				return new List<T>(capacity > 0 ? capacity : LIST_CAPACITY);
			}

			for (int i = 0; i < poolCount; ++i)
			{
				List<T> list = pool[i];

				if (list.Capacity < capacity)
					continue;

				pool.RemoveWithSwap(i);
				return list;
			}

			int lastListIndex = poolCount - 1;

			List<T> lastList = pool[lastListIndex];
			lastList.Capacity = capacity;

			pool.RemoveAt(lastListIndex);

			return lastList;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Return(List<T> list, int threadIndex)
		{
			if (list == null)
				return;

			list.Clear();

			_threadedPools[threadIndex].Add(list);
		}
	}

	//=============================================================================================================

	public static class ListPool
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> Get<T>(int capacity)
		{
			return ListPool<T>.Get(capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Return<T>(List<T> list)
		{
			ListPool<T>.Return(list);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> Get<T>(int capacity, int threadIndex)
		{
			return ListPool<T>.Get(capacity, threadIndex);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Return<T>(List<T> list, int threadIndex)
		{
			ListPool<T>.Return(list, threadIndex);
		}
	}
}
