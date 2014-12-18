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
using System.Windows.Forms;
using SyntaxHighlighter;
using System.ComponentModel;

namespace PSMC
{
    /// <summary>
    /// Interaktionslogik für PatternEditor.xaml
    /// </summary>
    public partial class PatternEditor : Window, INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public bool saveEnabled;
        public bool SaveEnabled
        {
            get
            {
                return saveEnabled;
            }

            set
            {
                saveEnabled = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SaveEnabled"));
            }
        }

        SyntaxRichTextBox m_syntaxRichTextBox = new SyntaxRichTextBox();
        public PatternEditor()
        {
            InitializeComponent();
            AddHandler(Keyboard.KeyDownEvent, (System.Windows.Input.KeyEventHandler)HandleKeyDownEvent);
            // Add the keywords to the list.
            m_syntaxRichTextBox.Settings.Keywords.Add("function");
            m_syntaxRichTextBox.Settings.Keywords.Add("if");
            m_syntaxRichTextBox.Settings.Keywords.Add("then");
            m_syntaxRichTextBox.Settings.Keywords.Add("else");
            m_syntaxRichTextBox.Settings.Keywords.Add("elseif");
            m_syntaxRichTextBox.Settings.Keywords.Add("end");

            // Set the comment identifier. 
            // For Lua this is two minus-signs after each other (--).
            // For C++ code we would set this property to "//".
            m_syntaxRichTextBox.Settings.Comment = "--";
            // Set the colors that will be used.
            m_syntaxRichTextBox.Settings.KeywordColor = System.Drawing.Color.Blue;
            m_syntaxRichTextBox.Settings.CommentColor = System.Drawing.Color.Green;
            m_syntaxRichTextBox.Settings.StringColor = System.Drawing.Color.Gray;
            m_syntaxRichTextBox.Settings.IntegerColor = System.Drawing.Color.Red;
            // Let's not process strings and integers.
            m_syntaxRichTextBox.Settings.EnableStrings = true;
            m_syntaxRichTextBox.Settings.EnableIntegers = true;

            // Let's make the settings we just set valid by compiling
            // the keywords to a regular expression.
            m_syntaxRichTextBox.CompileKeywords();
            m_syntaxRichTextBox.Font = new System.Drawing.Font("Courier New", 10);
            m_syntaxRichTextBox.Text = PSMC.Properties.Settings.Default.CustomModelPattern;
            m_syntaxRichTextBox.Invalidated  += m_syntaxRichTextBox_TextChanged;
            m_syntaxRichTextBox.PreviewKeyDown += m_syntaxRichTextBox_TextChanged;
            this.wfh.Child = m_syntaxRichTextBox;
            m_syntaxRichTextBox.ProcessAllLines();

        }

        void m_syntaxRichTextBox_TextChanged(object sender, EventArgs e)
        {
            SaveEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            m_syntaxRichTextBox.Text = PSMC.Properties.Resources.CustomModelPatternBackup;
            m_syntaxRichTextBox.Invalidate();
            m_syntaxRichTextBox.ProcessAllLines();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            m_syntaxRichTextBox.Text = PSMC.Properties.Resources.HatModelPattern;
            m_syntaxRichTextBox.Invalidate();
            m_syntaxRichTextBox.ProcessAllLines();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            m_syntaxRichTextBox.Text = PSMC.Properties.Resources.PlayerModelPattern;
            m_syntaxRichTextBox.Invalidate();
            m_syntaxRichTextBox.ProcessAllLines();

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            m_syntaxRichTextBox.Text = PSMC.Properties.Resources.ItemModelPattern;
            m_syntaxRichTextBox.Invalidate();
            m_syntaxRichTextBox.ProcessAllLines();

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            SaveEnabled = false;
            PSMC.Properties.Settings.Default.CustomModelPattern = m_syntaxRichTextBox.Text;
            PSMC.Properties.Settings.Default.Save();
        }

        private void HandleKeyDownEvent(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                SaveEnabled = false;
                PSMC.Properties.Settings.Default.CustomModelPattern = m_syntaxRichTextBox.Text;
                PSMC.Properties.Settings.Default.Save();
            }
        }
    }
}
