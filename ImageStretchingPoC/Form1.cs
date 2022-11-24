using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ImageStretchingPoC
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			filler = new();
			//filler.PaintEvent += pictureBox1_Paint;
			bitmap = (Bitmap)Image.FromFile("""C:\Users\felix\Downloads\-Wallpapers-Full-HD-random-35881387-1920-1080.png""");
			InitializeComponent();
			filler.Show();
		}

		readonly Bitmap bitmap;
		readonly ScreenFiller filler;

		[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
		private void pictureBox1_Paint(int height, int width)
		{
			//Stopwatch stopwatch = Stopwatch.StartNew();

			var bmp = Stretcher.StretchBitmap(
				bitmap,
				width,
				height,
				new((int)numericUpDown2.Value, (int)numericUpDown7.Value),
				new((int)numericUpDown4.Value, (int)numericUpDown5.Value),
				new((int)numericUpDown1.Value, (int)numericUpDown8.Value),
				new((int)numericUpDown3.Value, (int)numericUpDown6.Value));

			//stopwatch.Stop();
			filler.BackgroundImage = bmp;
			//MessageBox.Show(stopwatch.ElapsedMilliseconds.ToString());
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			//filler.Invalidate();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			pictureBox1_Paint(filler.Height, filler.Width);
		}
	}
}