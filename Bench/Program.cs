using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Drawing;

BenchmarkRunner.Run<Bench>();



[MemoryDiagnoser(false)]
public class Bench
{
	Bitmap bmp = (Bitmap)Image.FromFile("""C:\Users\felix\Downloads\-Wallpapers-Full-HD-random-35881387-1920-1080.png""");


	[Benchmark]
	public void Stretch()
	{
		var stretched = Stretcher.StretchBitmap(bmp, 1920, 1080, new(0, 0), new(0, 100), new(0, 0), new(0, 0));
	}
}