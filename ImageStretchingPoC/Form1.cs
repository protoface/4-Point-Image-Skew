using System.IO.Ports;

namespace ImageStretchingPoC;

public partial class Form1 : Form
{
    public Form1(string[] args)
    {
        filler = new();
        filler.Show();
        blank = new(filler.Width, filler.Height);
        if (args.Any())
        {
            if (Directory.Exists(args.Last()))
                textBox1.Text = args.Last();
            if (args.Last().Contains('.') && File.Exists(args.Last()))
                blank = (Bitmap)Image.FromFile(args.Last());
        }
        graphics = filler.CreateGraphics();
        InitializeComponent();

    }

    string[]? frames;

    int currentFrameIndex;

    Point topLeft, topRight, bottomLeft, bottomRight;

    bool Running;

    SerialPort? connection;

    readonly Bitmap blank;
    ScreenFiller filler;

    Graphics? graphics;

    private void button1_Click(object sender, EventArgs e)
    {
        topLeft = new((int)numericUpDown2.Value, (int)numericUpDown7.Value);
        topRight = new((int)numericUpDown4.Value, (int)numericUpDown5.Value);
        bottomLeft = new((int)numericUpDown1.Value, (int)numericUpDown8.Value);
        bottomRight = new((int)numericUpDown3.Value, (int)numericUpDown6.Value);

        var bmp = Stretcher.StretchBitmap(
            blank,
            filler.Width,
            filler.Height,
            topLeft,
            topRight,
            bottomLeft,
            bottomRight);

        graphics?.DrawImageUnscaled(bmp, new Point(0, 0));
    }

    private void button2_Click(object sender, EventArgs e)
    {
        folderBrowserDialog1.ShowDialog();

        textBox1.Text = folderBrowserDialog1.SelectedPath;
    }

    private void button4_Click(object sender, EventArgs e)
    {
        comboBox1.Items.Clear();
        comboBox1.Items.AddRange(SerialPort.GetPortNames());
    }

    private void button6_Click(object sender, EventArgs e)
    {
        filler.Close();
        filler = new();
        filler.Show();
        graphics?.Dispose();
        graphics = filler.CreateGraphics();

    }

    private void button7_Click(object sender, EventArgs e)
    {
        if (Running) return;

        if (filler.WindowState == FormWindowState.Maximized)
        {
            filler.WindowState = FormWindowState.Normal;
            filler.FormBorderStyle = FormBorderStyle.FixedDialog;
        }
        else
        {
            filler.WindowState = FormWindowState.Maximized;
            filler.FormBorderStyle = FormBorderStyle.None;
        }
        graphics?.Dispose();
        graphics = filler.CreateGraphics();
    }

    private void button5_Click(object sender, EventArgs e)
    {
        connection = new((string)comboBox1.SelectedItem, 9600);
    }

    private void button3_Click(object sender, EventArgs e)
    {
        if (Running)
        {
            timer1.Stop();
            graphics?.Clear(Color.Black);
            Running = false;
            return;
        }
        if (!checkBox1.Checked)
        {
            if (connection == null || !connection.IsOpen) throw new Exception("No Serial connection established");
            connection.WriteLine("A0.1");
        }
        //if (frames == null) throw new Exception("No program loaded. Please select print first");

        frames = Directory.GetFiles(textBox1.Text);
        currentFrameIndex = 0;

        topLeft = new((int)numericUpDown2.Value, (int)numericUpDown7.Value);
        topRight = new((int)numericUpDown4.Value, (int)numericUpDown5.Value);
        bottomLeft = new((int)numericUpDown1.Value, (int)numericUpDown8.Value);
        bottomRight = new((int)numericUpDown3.Value, (int)numericUpDown6.Value);

        label4.Text = $"Schicht {currentFrameIndex + 1}/{frames.Length}";

        TrySetImage(frames[currentFrameIndex]);

        timer1.Interval = (int)numericUpDown9.Value;
        timer1.Start();
        Running = true;
    }

    void AdvanceLayer()
    {
        SetBlank();
        if (frames == null) throw new Exception("No program loaded. Please select print first");

        if (!checkBox1.Checked)
        {
            if (connection == null || !connection.IsOpen) throw new Exception("No Serial connection established");
            connection.WriteLine("+0.1");
            while (connection.ReadChar() != 'e') ; // Last letter of "Done"
        }

        currentFrameIndex++;

        TrySetImage(frames[currentFrameIndex]);
    }

    private void SetBlank() => graphics?.Clear(Color.Black);

    void TrySetImage(string path)
    {

        Bitmap original;
        try
        {
            original = new(path, true);
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to load image {path}", e);
        }

        var bmp = Stretcher.StretchBitmap(
            original,
            filler.Width,
            filler.Height,
            topLeft,
            topRight,
            bottomLeft,
            bottomRight);

        graphics?.DrawImageUnscaled(bmp, new Point(0, 0));
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        timer1.Stop();
        timer1.Interval = (int)numericUpDown10.Value;

        AdvanceLayer();

        label4.Text = $"Schicht {currentFrameIndex + 1}/{frames!.Length}";

        timer1.Start();

    }

    private void button8_Click(object sender, EventArgs e)
    {
        connection?.WriteLine("H");
    }
}