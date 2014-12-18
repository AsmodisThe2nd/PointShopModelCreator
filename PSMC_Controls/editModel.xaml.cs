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
using System.Windows.Shapes;

namespace PSMC
{
    /// <summary>
    /// Interaktionslogik für editModel.xaml
    /// </summary>
    public partial class editModel : Window
    {
        public editModel(ModelItem item)
        {
            InitializeComponent();
            Binding myBinding = new Binding("Text");
            myBinding.Source = item;
            name.SetBinding(TextBox.TextProperty, myBinding);

            Binding myBinding2 = new Binding("Description");
            myBinding2.Source = item;
            desc.SetBinding(TextBox.TextProperty, myBinding2);

            Binding myBinding3 = new Binding("ModelVirtualPath");
            myBinding3.Source = item;
            v_path.SetBinding(TextBox.TextProperty, myBinding3);

            Binding myBinding4 = new Binding("ModelPrice");
            myBinding4.Source = item;
            price.SetBinding(TextBox.TextProperty, myBinding4);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Close();
            }

        }
    }
}
