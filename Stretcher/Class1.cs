using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class Stretcher
{
	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public unsafe static Bitmap StretchBitmap(Bitmap original, int stretchedWidth, int stretchedHeight, Point topLeft, Point topRight, Point bottomLeft, Point bottomRight)
	{
		Bitmap stretched = new(stretchedWidth, stretchedHeight, PixelFormat.Format8bppIndexed);

		int imgHeight = original.Height;
		int imgWidth = original.Width;

		float xOffsetLeftPre = (float)(bottomLeft.X - topLeft.X) / stretchedHeight;
		float xOffsetRightPre = (float)(bottomRight.X - topRight.X) / stretchedHeight;

		var mapOrig = original.LockBits(new(0, 0, imgWidth, imgHeight), ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);

		var mapStretched = stretched.LockBits(new(0, 0, stretchedWidth, stretchedHeight), ImageLockMode.ReadOnly, stretched.PixelFormat);
		byte* stretchedPtr = (byte*)mapStretched.Scan0.ToPointer();

		for (int y = 1; y < stretchedHeight; y++)
		{
			int xOffsetLeft = (int)((xOffsetLeftPre * y) + topLeft.X);
			int xOffsetRight = (int)((xOffsetRightPre * y) + topRight.X);
			float xLineLength = stretchedWidth - xOffsetLeft - xOffsetRight;
			float xFactor = imgWidth / xLineLength;

			float yOffsetTopPre = (topRight.Y - topLeft.Y) / xLineLength;
			float yOffsetBottomPre = (bottomRight.Y - bottomLeft.Y) / xLineLength;

			for (int x = 1; x < xLineLength; x++)
			{
				int yOffsetTop = (int)((yOffsetTopPre * x) + topLeft.Y);
				int yOffsetBottom = (int)((yOffsetBottomPre * x) + bottomLeft.Y);
				float yLineLength = stretchedHeight - yOffsetBottom - yOffsetTop;
				float yFactor = imgHeight / yLineLength;

				if (y * yFactor >= imgHeight)
					continue;

				if (!(y < yLineLength + yOffsetTop))
					continue;

				int h = y * stretchedWidth, p = y * imgWidth;
				stretchedPtr[((y + yOffsetTop) * mapStretched.Stride) + x + xOffsetLeft]
					= GetIndexedPixel((int)(x * xFactor), (int)(y * yFactor), mapOrig) != 0
					? byte.MaxValue
					: (byte)0x0;

			}
		}

		original.UnlockBits(mapOrig);
		stretched.UnlockBits(mapStretched);

		return stretched;
	}

	internal static unsafe int GetIndexedPixel(int x, int y, BitmapData bmd)
	{
		var index = (y * bmd.Stride) + (x >> 3);
		var p = Marshal.ReadByte(bmd.Scan0, index);
		var mask = (byte)(0x80 >> (x & 0x7));
		return p &= mask;
	}
}