using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PlayMartWpf
{
    /// <summary>
    /// KeyboardWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyboardWindow : Window
    {

        public delegate void SelecetBarcodeEventHandler(string kbname);
        public event SelecetBarcodeEventHandler SelecetBarcodeEvent;
 



        public List<string> kbList { get; set; }
        public string SelectedKeyboard { get; set; }

        public KeyboardWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string i in kbList)
            {
                cbList.Items.Add(i);
            }
            cbList.SelectedItem = SelectedKeyboard;


        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            string SelBarcode = cbList.SelectedValue.ToString();

            SelecetBarcodeEvent(SelBarcode);

            this.Close();
            
        }



    }
}
