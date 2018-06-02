using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

class Wallpaper
{
    Wallpaper() { }

    static void Main(string[] args)
    {
        Thread t = new Thread(new ThreadStart(newThread));
        t.SetApartmentState(ApartmentState.STA);
        t.Start();

        Console.Read();
    }

    private static void newThread()
    {
        String path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\animate.mp4";
        if (!File.Exists(path))
        {
            Console.WriteLine("File does not exist at path " + path);
            Console.Read();
            return;
        }

        Form f = new Form
        {
            Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height),
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None,
        };

        MediaElement me = new MediaElement()
        {
            Name = "mediaElement1",
            Height = f.Height,
            Width = f.Width,
            Source = new Uri(path),
            Stretch = System.Windows.Media.Stretch.UniformToFill,
            LoadedBehavior = MediaState.Manual,
        };

        me.MediaEnded += new System.Windows.RoutedEventHandler(mediaEnded);

        ElementHost host = new ElementHost();
        host.Dock = DockStyle.Fill;
        host.Child = me;
        me.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

        f.Controls.Add(host);
        me.Play();

        Win32.setOwnership(f);

        Application.EnableVisualStyles();
        Application.Run(f);
    }

    private static void mediaEnded(object sender, System.Windows.RoutedEventArgs e)
    {
        ((MediaElement)sender).Position = TimeSpan.Zero;
        ((MediaElement)sender).Play();
    }
}
