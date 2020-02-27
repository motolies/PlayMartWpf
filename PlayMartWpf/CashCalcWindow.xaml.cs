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
    /// CashCalcWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CashCalcWindow : Window
    {
        public delegate void CalcCompletedEventHandler();
        public event CalcCompletedEventHandler CalcCompletedEvent;

        public Dictionary<string, Item> cartList { get; set; }
        private int TotalAmount = 0;

        public CashCalcWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TotalAmount = 0;
            foreach (KeyValuePair<string, Item> i in cartList)
            {
                TotalAmount += i.Value.TotalAmount;
            }
            txtSubject.Text = TotalAmount.ToString("#,##0");

        }

        private void btnNumber_Click(object sender, RoutedEventArgs e)
        {
            string bName = ((Button)sender).Name.Replace("btn", string.Empty);
            string tmpRecieve = txtRecieve.Text.Replace(",", string.Empty);


            switch (bName)
            {
                case "BS":
                    int iRecieveBS = Convert.ToInt32(tmpRecieve.Substring(0, tmpRecieve.Length - 1));
                    txtRecieve.Text = iRecieveBS.ToString("#,##0");
                    break;
                case "CLR":
                    txtRecieve.Text = "0";
                    break;
                default:
                    string tmpRecieveAdd = tmpRecieve + bName;
                    int rInteger = 0;
                    if (int.TryParse(tmpRecieveAdd, out rInteger))
                    {
                        txtRecieve.Text = rInteger.ToString("#,##0");
                    }
                    else
                    {
                        Common.PopupMessage("21억보다 큰 수는 입력할 수 없습니다.");
                        return;
                    }
                    break;

            }

            //일련의 작업이 끝나면 다시 계산
            int result = (TotalAmount - Convert.ToInt32(txtRecieve.Text.Replace(",", string.Empty))) * -1;
            txtReturnCharge.Text = result.ToString("#,##0");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCalc_Click(object sender, RoutedEventArgs e)
        {
            //거스름돈이 더 클때만... 
            int rst = Convert.ToInt32(txtReturnCharge.Text.Replace(",", string.Empty));
            if(rst < 0)
            {
                Common.PopupMessage("받은 돈이 더 적습니다.");
            }
            else
            {
                CalcCompletedEvent();
                this.Close();
            }


        }
    }
}
