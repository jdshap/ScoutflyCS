using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace SFcustom
{
    /// <summary>
    /// gi_param.gip Relevant Offsets (Original World Ver.)
    /// 
    /// 00C0 - Flies Normal
    /// 00C4 - Flies Noticed
    /// 00C8 - Flies Scared
    /// 00CB - Flies Elder
    /// 00D0 - Flies Tempered
    /// 00D4 - Tracks Normal
    /// 00D8 - Tracks Elder
    /// 00DB - Tracks Tempered
    /// 
    /// gi_param.gip Relevant Offsets (Iceborne Ver.)
    /// 
    /// 00C0 - Flies Normal
    /// 00C4 - Flies Noticed
    /// 00C8 - Flies Scared
    /// 00CB - Flies Elder
    /// 00D0 - Flies Tempered
    /// 00D4 - Tracks Normal
    /// 00D8 - Tracks Elder
    /// 00DB - Tracks Tempered
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        static String path = "";
        static byte[] raw;
        public static ObservableCollection<Xceed.Wpf.Toolkit.ColorItem> ColorList;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            PrepareColorPicker();

            OpacitySlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(OpacitySlider_ValueChanged);
            OpacityBox.TextChanged += new TextChangedEventHandler(OpacityBox_TextChanged);
            LoadFile.Click += new RoutedEventHandler(LoadFile_Click);
            SaveFile.Click += new RoutedEventHandler(SaveFile_Click);
        }

        private void PrepareColorPicker()
        {
            ColorList = new ObservableCollection<Xceed.Wpf.Toolkit.ColorItem>();
            ColorList.Add(new ColorItem(Color.FromRgb(127, 182, 75), "Normal Green"));
            ColorList.Add(new ColorItem(Color.FromRgb(98, 134, 255), "Elder Blue"));
            ColorList.Add(new ColorItem(Color.FromRgb(169, 118, 220), "Tempered Purple"));
            ColorList.Add(new ColorItem(Color.FromRgb(237, 125, 51), "AT Orange"));
            ColorList.Add(new ColorItem(Color.FromRgb(155, 81, 56), "Startled Red"));

            Style style = new Style(typeof(ColorPicker));
            style.Setters.Add(new Setter(ColorPicker.MarginProperty, new Thickness(20)));
            style.Setters.Add(new Setter(ColorPicker.ShowStandardColorsProperty, true));
            style.Setters.Add(new Setter(ColorPicker.ShowRecentColorsProperty, false));
            style.Setters.Add(new Setter(ColorPicker.ShowAvailableColorsProperty, false));

            style.Setters.Add(new Setter(ColorPicker.StandardTabHeaderProperty, "Defaults"));
            style.Setters.Add(new Setter(ColorPicker.AdvancedTabHeaderProperty, "Custom"));

            style.Setters.Add(new Setter(ColorPicker.ShowDropDownButtonProperty, false));

            style.Setters.Add(new Setter(ColorPicker.StandardColorsProperty, ColorList));
            Resources.Add(typeof(ColorPicker), style);
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Scoutfly File|gi_param.gip";
            if (openFileDialog.ShowDialog() == true)
                path = openFileDialog.FileName;

            raw = File.ReadAllBytes(path);
            if (raw[4] == 71)
            {
                ColorNormal.SelectedColor = Color.FromRgb(raw[220], raw[221], raw[222]);
                ColorNormalTrack.SelectedColor = Color.FromRgb(raw[240], raw[241], raw[242]);

                ColorElder.SelectedColor = Color.FromRgb(raw[232], raw[233], raw[234]);
                ColorElderTrack.SelectedColor = Color.FromRgb(raw[244], raw[245], raw[246]);

                ColorTemper.SelectedColor = Color.FromRgb(raw[236], raw[237], raw[238]);
                ColorTemperTrack.SelectedColor = Color.FromRgb(raw[248], raw[249], raw[250]);

                ColorAlert.SelectedColor = Color.FromRgb(raw[228], raw[229], raw[230]);

                OpacitySlider.Value = raw[223];

                ColorHFRNormal.SelectedColor = Color.FromRgb(raw[252], raw[253], raw[254]);
                ColorHFRNormalTrack.SelectedColor = Color.FromRgb(raw[260], raw[261], raw[262]);

                ColorHFRElder.SelectedColor = Color.FromRgb(raw[256], raw[257], raw[258]);
                ColorHFRElderTrack.SelectedColor = Color.FromRgb(raw[264], raw[265], raw[266]);

                SysOut.Text = "File loaded successfully!";
            }
            else if (raw[0] == 71)
            {
                SysOut.Text = "Unable to open file.&#x0a;Make sure you have the new gi_param.gip file.";
            }
            else SysOut.Text = "Unable to open file.";
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            byte[] fnorm = new byte[] { ((Color)ColorNormal.SelectedColor).R, ((Color)ColorNormal.SelectedColor).G, ((Color)ColorNormal.SelectedColor).B };
            byte[] tnorm = new byte[] { ((Color)ColorNormalTrack.SelectedColor).R, ((Color)ColorNormalTrack.SelectedColor).G, ((Color)ColorNormalTrack.SelectedColor).B };

            byte[] feld = new byte[] { ((Color)ColorElder.SelectedColor).R, ((Color)ColorElder.SelectedColor).G, ((Color)ColorElder.SelectedColor).B };
            byte[] teld = new byte[] { ((Color)ColorElderTrack.SelectedColor).R, ((Color)ColorElderTrack.SelectedColor).G, ((Color)ColorElderTrack.SelectedColor).B };

            byte[] ftemp = new byte[] { ((Color)ColorTemper.SelectedColor).R, ((Color)ColorTemper.SelectedColor).G, ((Color)ColorTemper.SelectedColor).B };
            byte[] ttemp = new byte[] { ((Color)ColorTemperTrack.SelectedColor).R, ((Color)ColorTemperTrack.SelectedColor).G, ((Color)ColorTemperTrack.SelectedColor).B };

            byte[] startle = new byte[] { ((Color)ColorAlert.SelectedColor).R, ((Color)ColorAlert.SelectedColor).G, ((Color)ColorAlert.SelectedColor).B };

            Color lighter = GenerateLighter((Color)ColorAlert.SelectedColor);
            byte[] startlite = new byte[] { lighter.R, lighter.G, lighter.B };

            byte alpha = (byte)OpacitySlider.Value;

            byte[] fnormHFR = new byte[] { ((Color)ColorHFRNormal.SelectedColor).R, ((Color)ColorHFRNormal.SelectedColor).G, ((Color)ColorHFRNormal.SelectedColor).B };
            byte[] tnormHFR = new byte[] { ((Color)ColorHFRNormalTrack.SelectedColor).R, ((Color)ColorHFRNormalTrack.SelectedColor).G, ((Color)ColorHFRNormalTrack.SelectedColor).B };

            byte[] feldHFR = new byte[] { ((Color)ColorHFRElder.SelectedColor).R, ((Color)ColorHFRElder.SelectedColor).G, ((Color)ColorHFRElder.SelectedColor).B };
            byte[] teldHFR = new byte[] { ((Color)ColorHFRElderTrack.SelectedColor).R, ((Color)ColorHFRElderTrack.SelectedColor).G, ((Color)ColorHFRElderTrack.SelectedColor).B };

            raw[220] = fnorm[0]; raw[221] = fnorm[1]; raw[222] = fnorm[2];
            raw[240] = tnorm[0]; raw[241] = tnorm[1]; raw[242] = tnorm[2];

            raw[232] = feld[0]; raw[233] = feld[1]; raw[234] = feld[2];
            raw[244] = teld[0]; raw[245] = teld[1]; raw[246] = teld[2];

            raw[236] = ftemp[0]; raw[237] = ftemp[1]; raw[238] = ftemp[2];
            raw[248] = ttemp[0]; raw[249] = ttemp[1]; raw[250] = ttemp[2];

            raw[224] = startlite[0]; raw[225] = startlite[1]; raw[226] = startlite[2];
            raw[228] = startle[0]; raw[229] = startle[1]; raw[230] = startle[2];

            raw[223] = raw[227] = raw[231] = raw[235] = raw[239] = raw[243] = raw[247] = raw[251] = raw[255] = raw[259] = raw[263] = raw[267] = raw[1523] = alpha;

            raw[252] = fnormHFR[0]; raw[253] = fnormHFR[1]; raw[254] = fnormHFR[2];
            raw[260] = tnormHFR[0]; raw[261] = tnormHFR[1]; raw[262] = tnormHFR[2];

            raw[256] = feldHFR[0]; raw[257] = feldHFR[1]; raw[258] = feldHFR[2];
            raw[264] = teldHFR[0]; raw[265] = teldHFR[1]; raw[266] = teldHFR[2];

            File.Delete(path);
            File.WriteAllBytes(path, raw);

            SysOut.Text = "File saved successfully!";
        }

        private Color GenerateLighter(Color input)
        {
            int red = input.R;
            int green = input.G;
            int blue = input.B;

            red += 99;
            green += 127;
            blue += 101;

            return Color.FromRgb((byte) red, (byte) green, (byte) blue);
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OpacityBox.Text = OpacitySlider.Value.ToString();
        }

        private void OpacityBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(OpacityBox.Text, out int value);
            OpacitySlider.Value = value;
            OpacityBox.Text = OpacitySlider.Value.ToString();
        }
    }
}
