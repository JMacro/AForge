using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AForge.Imaging
{
	public static class MemoryManager
	{
		private class CacheBlock
		{
			public IntPtr MemoryBlock;

			public int Size;

			public bool Free;

			public CacheBlock(IntPtr memoryBlock, int size)
			{
				MemoryBlock = memoryBlock;
				Size = size;
				Free = false;
			}
		}

		private static int maximumCacheSize = 3;

		private static int currentCacheSize = 0;

		private static int busyBlocks = 0;

		private static int cachedMemory = 0;

		private static int maxSizeToCache = 20971520;

		private static int minSizeToCache = 10240;

		private static List<CacheBlock> memoryBlocks = new List<CacheBlock>();

		public static int MaximumCacheSize
		{
			get
			{
				lock (memoryBlocks)
				{
					return maximumCacheSize;
				}
			}
			set
			{
				lock (memoryBlocks)
				{
					maximumCacheSize = System.Math.Max(0, System.Math.Min(10, value));
				}
			}
		}

		public static int CurrentCacheSize
		{
			get
			{
				lock (memoryBlocks)
				{
					return currentCacheSize;
				}
			}
		}

		public static int BusyMemoryBlocks
		{
			get
			{
				lock (memoryBlocks)
				{
					return busyBlocks;
				}
			}
		}

		public static int FreeMemoryBlocks
		{
			get
			{
				lock (memoryBlocks)
				{
					return currentCacheSize - busyBlocks;
				}
			}
		}

		public static int CachedMemory
		{
			get
			{
				lock (memoryBlocks)
				{
					return cachedMemory;
				}
			}
		}

		public static int MaxSizeToCache
		{
			get
			{
				return maxSizeToCache;
			}
			set
			{
				maxSizeToCache = value;
			}
		}

		public static int MinSizeToCache
		{
			get
			{
				return minSizeToCache;
			}
			set
			{
				minSizeToCache = value;
			}
		}

		public static IntPtr Alloc(int size)
		{
			lock (memoryBlocks)
			{
				if (busyBlocks >= maximumCacheSize || size > maxSizeToCache || size < minSizeToCache)
				{
					return Marshal.AllocHGlobal(size);
				}
				if (currentCacheSize == busyBlocks)
				{
					IntPtr intPtr = Marshal.AllocHGlobal(size);
					memoryBlocks.Add(new CacheBlock(intPtr, size));
					busyBlocks++;
					currentCacheSize++;
					cachedMemory += size;
					return intPtr;
				}
				for (int i = 0; i < currentCacheSize; i++)
				{
					CacheBlock cacheBlock = memoryBlocks[i];
					if (cacheBlock.Free && cacheBlock.Size >= size)
					{
						cacheBlock.Free = false;
						busyBlocks++;
						return cacheBlock.MemoryBlock;
					}
				}
				for (int j = 0; j < currentCacheSize; j++)
				{
					CacheBlock cacheBlock2 = memoryBlocks[j];
					if (cacheBlock2.Free)
					{
						Marshal.FreeHGlobal(cacheBlock2.MemoryBlock);
						memoryBlocks.RemoveAt(j);
						currentCacheSize--;
						cachedMemory -= cacheBlock2.Size;
						IntPtr intPtr2 = Marshal.AllocHGlobal(size);
						memoryBlocks.Add(new CacheBlock(intPtr2, size));
						busyBlocks++;
						currentCacheSize++;
						cachedMemory += size;
						return intPtr2;
					}
				}
				return IntPtr.Zero;
			}
		}

		public static void Free(IntPtr pointer)
		{
			lock (memoryBlocks)
			{
				for (int i = 0; i < currentCacheSize; i++)
				{
					if (memoryBlocks[i].MemoryBlock == pointer)
					{
						memoryBlocks[i].Free = true;
						busyBlocks--;
						return;
					}
				}
				Marshal.FreeHGlobal(pointer);
			}
		}

		public static int FreeUnusedMemory()
		{
			lock (memoryBlocks)
			{
				int num = 0;
				for (int num2 = currentCacheSize - 1; num2 >= 0; num2--)
				{
					if (memoryBlocks[num2].Free)
					{
						Marshal.FreeHGlobal(memoryBlocks[num2].MemoryBlock);
						cachedMemory -= memoryBlocks[num2].Size;
						memoryBlocks.RemoveAt(num2);
						num++;
					}
				}
				currentCacheSize -= num;
				return num;
			}
		}
	}
}
