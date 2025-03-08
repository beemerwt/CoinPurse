using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CoinPurse
{
	public class ZPackageHelper<T>
	{

		private static bool TryConvert(ZPackage pkg, int size, ref T value)
		{
			try
			{
				byte[] bytes = pkg.ReadByteArray(size);
				value = (T)Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), typeof(T));
				return true;
			} catch (Exception)
			{
				return false;
			}
		}

		public static T GetKeyedValue(ZPackage pkg, int key, T defaultValue = default)
		{
			var size = Marshal.SizeOf<T>();
			int origin = pkg.GetPos();

			int pos = origin;
			T value = defaultValue;

			while (pos + size < pkg.Size())
			{
				// Read up until the key specifically
				var k = pkg.ReadInt();
				pos += 4;

				if (k != key)
					continue;

				if (TryConvert(pkg, size, ref value))
					break;

				pkg.SetPos(pos); // move to next int
			}

			pkg.SetPos(origin);
			return value;
		}
	}

	public static class ZPackageExtensions
	{
		public static int GetKeyedInt(this ZPackage pkg, int key, int defaultValue = 0)
			=> ZPackageHelper<int>.GetKeyedValue(pkg, key, defaultValue);

		public static bool WriteKeyedValue<T>(this ZPackage pkg, int key, T value)
		{
			var size = Marshal.SizeOf<T>();
			pkg.Write(key);

			// the key always needs to be aligned to 4 bytes
			var boundary = 

			// Convert value to byte array
			byte[] bytes = new byte[size];
			Marshal.Copy(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), bytes, 0, size);
			pkg.Write(bytes);
			return true;
		}
	}
}
