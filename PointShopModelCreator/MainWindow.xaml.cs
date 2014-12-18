using Morten.Gmod.Gmad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PSMC;
using System.Diagnostics;
using Microsoft.Win32;

namespace PSMC
{
    public enum CrModes
    {
        Character,
        Custom,
        Hat,
        Item
    };
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }


        public MainWindow()
        {
            InitializeComponent();
            AddHandler(Keyboard.KeyDownEvent, (System.Windows.Input.KeyEventHandler)HandleKeyDownEvent);

            CrMode = CrModes.Character;
        }

        private CrModes crMode;
        public CrModes CrMode
        {
            get
            {
                return crMode;
            }

            set
            {
                crMode = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CrMode"));
            }
        }

        private string generateLua(ModelItem item, CrModes crm)
        {
            string pattern = "";
            switch (crm)
            {
                case CrModes.Character:
                    pattern = PSMC.Properties.Resources.PlayerModelPattern;
                    break;
                case CrModes.Custom:
                    pattern = PSMC.Properties.Settings.Default.CustomModelPattern;
                    break;
                case CrModes.Hat:
                    pattern = PSMC.Properties.Resources.HatModelPattern;
                    break;
                case CrModes.Item:
                    pattern = PSMC.Properties.Resources.ItemModelPattern;
                    break;
                default:
                    break;
            }

            pattern = pattern.Replace("%MODEL_NAME%", item.Text);
            pattern = pattern.Replace("%MODEL_VIRTUAL_PATH%", item.ModelVirtualPath);
            pattern = pattern.Replace("%MODEL_FILENAME%", Modelhandling.getModelNameFromPath(item.ModelPath) + ".mdl");
            pattern = pattern.Replace("%MODEL_PRICE%", item.ModelPrice.ToString());
            pattern = pattern.Replace("%MODEL_CLEAN_NAME%", Tools.AdjustPath(item.Text));

            return pattern;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ModelItems.SelectedItems == null)
                return;

            for (int i = 0; i < 10; i++)
            {
                var blur = new BlurEffect();
                var curr = this.Background;
                blur.Radius = i;
                Main.Effect = blur;
                Thread.Sleep(20);
                Tools.DoEvents();
            }
            List<string> addonlist = new List<string>();
            progressb.Width = this.ActualWidth / 2;
            progressb.Visibility = System.Windows.Visibility.Visible;
            progressl.Visibility = System.Windows.Visibility.Visible;

            int count = ModelItems.SelectedItems.Length;
            progressb.Maximum = count;
            int current = 0;
            progressl.Content = current + "/" + count;
            foreach (ModelItem item in ModelItems.SelectedItems)
            {
                if (!Directory.Exists(modelSavePath.Text + "\\"))
                    Directory.CreateDirectory(modelSavePath.Text + "\\");

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(modelSavePath.Text + "\\" + Tools.AdjustPath(item.Text) + ".lua", true))
                {
                    file.Write(generateLua(item, this.CrMode));
                    file.Close();
                    file.Dispose();
                }
                if (item.FromAddon != null && item.FromAddon != "" && !addonlist.Contains("resource.AddWorkshop(\"" + item.FromAddon + "\")\r\n"))
                {
                    addonlist.Add("resource.AddWorkshop(\"" + item.FromAddon + "\")\r\n");
                }
                current++;
                progressb.Value = current;
                progressl.Content = current + "/" + count;
                Tools.DoEvents();
            }

            Main.Effect = null;
            progressb.Visibility = System.Windows.Visibility.Hidden;
            progressl.Visibility = System.Windows.Visibility.Hidden;

            if (addonlist.Count != 0)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(modelSavePath.Text + "\\AddonResources.lua", true))
                {
                    addonlist.ForEach(file.Write);
                    file.Close();
                    file.Dispose();
                }
            }
            if (oac.IsChecked.Value)
                Process.Start(modelSavePath.Text + "\\");
        }

        private string[] getFilesWithEnding(string ending)
        {
            string[] files = (from f in Directory.GetFiles(searchPath.Text, "*", SearchOption.AllDirectories)
                              where f.EndsWith(ending)
                              select f).ToArray();
            return files;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ModelItems.Children.Clear();

            for (int i = 0; i < 10; i++)
            {
                var blur = new BlurEffect();
                var curr = this.Background;
                blur.Radius = i;
                Main.Effect = blur;
                Thread.Sleep(20);
                Tools.DoEvents();
            }
            progressb.Width = this.ActualWidth / 2;
            progressb.Visibility = System.Windows.Visibility.Visible;
            progressl.Visibility = System.Windows.Visibility.Visible;
            try
            {
                string[] mdls = getFilesWithEnding(".mdl");
                int count = mdls.Length;
                progressb.Maximum = count;
                int current = 0;
                progressl.Content = current + "/" + count;

                if ((bool)extradd.IsChecked)
                {

                    string[] gmas = getFilesWithEnding(".gma");
                    count += gmas.Length;
                    progressb.Maximum = count;

                    foreach (var item in gmas)
                    {
                        current++;
                        GmadArchive gma = new GmadArchive(item);
                        foreach (var gmitem in gma.Files)
                        {
                            Directory.CreateDirectory(@".\Tmp\" + gmitem.Name.Substring(0, gmitem.Name.LastIndexOf('\\')));
                            if (!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"Tmp\" + gmitem.Name))
                                gmitem.Extract(System.AppDomain.CurrentDomain.BaseDirectory + @"Tmp\" + gmitem.Name);
                            if (gmitem.Name.EndsWith(".mdl"))
                            {
                                ModelItem model = new ModelItem(System.AppDomain.CurrentDomain.BaseDirectory + @"Tmp\" + gmitem.Name);
                                model.Text = gma.Name;
                                model.Description = gma.Description;
                                model.FromAddon = gma.FileName.Substring(gma.FileName.Length - 13, 9);
                                ModelItems.Children.Add(model);
                            }
                        }
                        progressb.Value = current;
                        progressl.Content = current + "/" + count;

                        Tools.DoEvents();
                    }
                }

                foreach (var item in mdls)
                {
                    current++;
                    ModelItem model = new ModelItem(item);
                    ModelItems.Children.Add(model);
                    progressb.Value = current;
                    Tools.DoEvents();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ey alta, gab Feeehlaaaa!");
            }
            Main.Effect = null;
            progressb.Visibility = System.Windows.Visibility.Hidden;
            progressl.Visibility = System.Windows.Visibility.Hidden;

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (ModelItem item in ModelItems.Children)
            {
                item.getPreview();
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (ModelItem item in ModelItems.SelectedItems)
            {
                item.getPreview();
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            foreach (ModelItem item in ModelItems.Children)
            {
                item.IsSelected = true;
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            foreach (ModelItem item in ModelItems.Children)
            {
                item.IsSelected = !item.IsSelected;
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            foreach (ModelItem item in ModelItems.SelectedItems)
            {
                ModelItems.Children.Remove(item);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            PatternEditor pe = new PatternEditor();
            pe.Show();
        }

        private void Button_Unloaded(object sender, RoutedEventArgs e)
        {
            PSMC.Properties.Settings.Default.Save();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(@".\Tmp\"))
                Directory.Delete(@".\Tmp\", true);
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            About ab = new About();
            ab.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "hlmv|hlmv.exe|*|*.*";
            ofd.Multiselect = false;
            ofd.ShowDialog();
            PSMC.Properties.Settings.Default.hlmv_path = ofd.FileName;
            Kapsel.setHlmvPath(ofd.FileName);
            PSMC.Properties.Settings.Default.Save();
        }

        private void HandleKeyDownEvent(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.H && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                MessageBox.Show(PSMC.Properties.Settings.Default.hlmv_path);
            }
        }

    }
}
