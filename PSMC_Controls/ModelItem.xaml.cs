using System;
using System.Collections.Generic;
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
using System.Drawing;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace PSMC
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class ModelItem : UserControl, INotifyPropertyChanged
    {


        #region Variablen und Events
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                this.isSelected = value;
                if (this.IsSelected)
                {
                    this.Background = this.Background = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    var bc = new BrushConverter();
                    this.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#FF3399FF");
                }
                OnPropertyChanged(new PropertyChangedEventArgs("IsSelected"));
            }
        }

        private BitmapImage image;
        public BitmapImage Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Image"));
            }
        }

        private string addon;
        public string FromAddon
        {
            get { return addon; }
            set
            {
                addon = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FromAddon"));
            }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                OnPropertyChanged(new PropertyChangedEventArgs("Text"));
            }
        }

        private string modelpath;
        public string ModelPath
        {
            get { return modelpath; }
            set
            {
                modelpath = value;
                if ((this.ModelVirtualPath == null || this.ModelVirtualPath == "") && value.Contains(@"\models\"))
                {
                    this.ModelVirtualPath = value.Substring(value.IndexOf(@"\models\") + 1).Replace('\\', '/');
                }
                else
                {
                    this.ModelVirtualPath = "Couldn't find model path pattern. Please enter path manually.";
                }
                OnPropertyChanged(new PropertyChangedEventArgs("ModelPath"));
            }
        }

        private string modelvirtualpath;
        public string ModelVirtualPath
        {
            get { return modelvirtualpath; }
            set
            {
                modelvirtualpath = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ModelVirtualPath"));
            }
        }

        private int modelprice;
        public int ModelPrice
        {
            get { return modelprice; }
            set
            {
                modelprice = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ModelPrice"));
            }
        }

        private string imagepath;
        public string ImagePath
        {
            get { return imagepath; }
            set
            {
                imagepath = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ImagePath"));
            }
        }


        private string description;

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Description"));
            }
        }


        #endregion

        public ModelItem()
        {
            InitializeComponent();
            this.Text = "Kein Text";
            this.Image = Tools.BitmapSource2BitmapImage(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(PSMC.Properties.Resources.Image1.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(PSMC.Properties.Resources.Image1.Width,
                    PSMC.Properties.Resources.Image1.Height)));
            this.Description = "";
            this.ModelPrice = 100;

        }

        public ModelItem(string path)
        {
            InitializeComponent();
            this.ModelPath = path;
            this.Image = Tools.BitmapSource2BitmapImage(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(PSMC.Properties.Resources.Image1.GetHbitmap(),
    IntPtr.Zero,
    System.Windows.Int32Rect.Empty,
    BitmapSizeOptions.FromWidthAndHeight(PSMC.Properties.Resources.Image1.Width,
        PSMC.Properties.Resources.Image1.Height)));
            this.Description = "";
            this.Text = Modelhandling.getModelNameFromPath(path);
            this.ModelPrice = 100;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.IsSelected)
                this.Background = System.Windows.Media.Brushes.Orange;
        }


        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsSelected)
            {
                var bc = new BrushConverter();
                this.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#FF3399FF");
            }

        }


        public override string ToString()
        {
            return this.Text + "@" + this.ModelPath;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            getPreview();
        }

        public void getPreview()
        {
            try
            {
                ProcessStartInfo si = new ProcessStartInfo();
                si.Arguments = this.ModelPath;
                si.UseShellExecute = false;
                si.FileName = PSMC.Properties.Settings.Default.hlmv_path;

                System.Diagnostics.Process.Start(si);
                Thread.Sleep(2000);
                this.Image = Tools.Bitmap2BitmapImage(PSMC.MDLpreview.getPreview());
                Process[] proc = Process.GetProcessesByName("hlmv");
                proc[0].Kill();
            }
            catch (Exception)
            {
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Panel)this.Parent).Children.Remove(this);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.IsSelected = !this.IsSelected;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            editModel em = new editModel(this);
            em.WindowStartupLocation = WindowStartupLocation.Manual;
            em.Left = Mouse.GetPosition(Application.Current.MainWindow).X;
            em.Top = Mouse.GetPosition(Application.Current.MainWindow).Y;
            em.Show();
        }

        private void itemImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.IsSelected = !this.IsSelected;
        }
    }
}
