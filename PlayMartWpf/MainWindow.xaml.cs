using RawInput_dll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace PlayMartWpf
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        protected string SelectedDeviceName { get; set; }


        private RawInput _rawinput;
        const bool CaptureOnlyInForeground = false; // 활성화 상태에서만 true

        private string selectedBarcode = string.Empty;
        private Dictionary<string, Item> list = new Dictionary<string, Item>();
        private Dictionary<string, Item> cartItems = new Dictionary<string, Item>();

        ScaleTransform scale = new ScaleTransform();
        double orginalWidth, originalHeight;

        private List<string> kbList;

        public MainWindow()
        {
            InitializeComponent();


        }

        private void OnKeyPressed(object sender, RawInputEventArg e)
        {
            if (e.KeyPressEvent.Message == Win32.WM_KEYUP)
            {
                if (e.KeyPressEvent.DeviceName == SelectedDeviceName)
                {
                    if (e.KeyPressEvent.VKey == 13)
                    {
                        ModifyCart(selectedBarcode);
                        selectedBarcode = string.Empty;
                    }
                    else
                    {
                        selectedBarcode += (new System.Windows.Forms.KeysConverter()).ConvertToString(e.KeyPressEvent.VKey);
                    }
                }
            }
        }

        private static void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (null == ex) return;

            // Log this error. Logging the exception doesn't correct the problem but at least now
            // you may have more insight as to why the exception is being thrown.
            //Debug.WriteLine(string.Format("Unhandled Exception: " + ex.Message));
            //Debug.WriteLine(string.Format("Unhandled Exception: " + ex));
            MessageBox.Show(ex.Message);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RawKeyboard rk = new RawKeyboard();
            kbList = rk.DeviceNameList();
            SelectedDeviceName = kbList[0];

            //Init Keyboard
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _rawinput = new RawInput(Process.GetCurrentProcess().MainWindowHandle, CaptureOnlyInForeground);
            _rawinput.AddMessageFilter();   // Adding a message filter will cause keypresses to be handled
            Win32.DeviceAudit();            // Writes a file DeviceAudit.txt to the current directory
            _rawinput.KeyPressed += OnKeyPressed;


            // 이미지 백그라운드 설정
            this.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), System.AppDomain.CurrentDomain.BaseDirectory + @"\skin.jpg")));

            // 창 사이즈 조절용
            orginalWidth = this.Width;
            originalHeight = this.Height;
            if (this.WindowState == WindowState.Maximized)
            {
                ChangeSize(this.ActualWidth, this.ActualHeight);
            }
            this.SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);

            //바코드파일 읽어서 리스트로 가지고 있기
            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\barcode.txt";

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line.Trim()))
                        {
                            //객체 생성해서 넣기
                            string[] data = line.Split(';');
                            if (!list.ContainsKey(data[0]))
                                list.Add(data[0], new Item { Barcode = data[0], Name = data[1], Amount = Convert.ToInt32(data[2].Replace(",", string.Empty)), Count = 1 });
                        }
                    }
                }
            }


            //샘플 데이터 삽입
            //int cnt = 0;
            //foreach (KeyValuePair<string, Item> i in list)
            //{
            //    if (cnt > 50)
            //    {
            //        break;
            //    }
            //    cartItems.Add(i.Value.Barcode, new Item { Barcode = i.Value.Barcode, Amount = i.Value.Amount, Name = i.Value.Name, Count = 1 });
            //    cnt++;
            //}



            lvCart.ItemsSource = cartItems;
            UpdateTotalAmount();


            //버튼 텍스트 변경
            Label lbAdd = (Label)btnADD.Template.FindName("Content", btnADD);
            lbAdd.Content = "추가+";
            Label lbRemove = (Label)btnREMOVE.Template.FindName("Content", btnREMOVE);
            lbRemove.Content = "제거-";
            Label lbInit = (Label)btnInit.Template.FindName("Content", btnInit);
            lbInit.Content = "초기화";
            Label lbCard = (Label)btnCard.Template.FindName("Content", btnCard);
            lbCard.Content = "카드결제";
            Label lbCash = (Label)btnCash.Template.FindName("Content", btnCash);
            lbCash.Content = "현금결제";

            btnADD.IsEnabled = false;
        }

        private void ModifyCart(string barcode)
        {
            if (!list.ContainsKey(barcode))
            {
                Common.PopupMessage("물품을 등록해주세요!");
                return;
            }

            if (btnREMOVE.IsEnabled)
            {
                //상품 추가중 - 상품이 추가중일때는 barcode로 키가 있는지 검색 후 있으면 카운트++ 없으면 아이템 추가
                if (cartItems.ContainsKey(barcode))
                {
                    Item tmp = cartItems[barcode];
                    tmp.Count++;
                    cartItems[barcode] = tmp;
                }
                else
                {
                    cartItems.Add(barcode, list[barcode]);
                }
            }
            else
            {
                //상품 제거중 - 상품이 제거중 일때는 barcode로 검색 후 있으면 카운트가 1보다 클때는 카운트--, 카운트가 1일때는 딕셔너리에서 삭제
                if (cartItems.ContainsKey(barcode))
                {
                    if (cartItems[barcode].Count < 2)
                    {
                        //삭제
                        cartItems.Remove(barcode);
                    }
                    else
                    {
                        Item tmp = cartItems[barcode];
                        tmp.Count--;
                        cartItems[barcode] = tmp;
                    }
                }
                else
                {
                    Common.PopupMessage("제거할 상품이 없습니다!");
                }
            }
           
            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            lvCart.Items.Refresh();
            int total = 0;
            foreach (KeyValuePair<string, Item> i in cartItems)
            {
                total += i.Value.TotalAmount;
            }
            txtTotalAmount.Text = total.ToString("#,##0");
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _rawinput.KeyPressed -= OnKeyPressed;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeSize(e.NewSize.Width, e.NewSize.Height);
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            cartItems.Clear();
            lvCart.Items.Refresh();
            UpdateTotalAmount();
            txtTotalAmount.Focus();
        }

        private void ChangeSize(double width, double height)
        {
            scale.ScaleX = width / orginalWidth;
            scale.ScaleY = height / originalHeight;

            FrameworkElement rootElement = this.Content as FrameworkElement;

            rootElement.LayoutTransform = scale;
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            KeyboardWindow kw = new KeyboardWindow();
            kw.kbList = this.kbList;
            kw.SelectedKeyboard = SelectedDeviceName;
            kw.SelecetBarcodeEvent += new KeyboardWindow.SelecetBarcodeEventHandler(BarcodeSetting);
            //새창을 가운대로
            kw.Owner = Application.Current.MainWindow; // We must also set the owner for this to work.
            kw.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            kw.ShowDialog();

            txtTotalAmount.Focus();
        }

        private void BarcodeSetting(string kbname)
        {
            SelectedDeviceName = kbname;
        }

        private void btnStatus_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).Name;

            if (name == "btnADD")
            {
                btnADD.IsEnabled = false;
                btnREMOVE.IsEnabled = true;
            }
            else
            {
                btnADD.IsEnabled = true;
                btnREMOVE.IsEnabled = false;
            }
        }

        private void btnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCard_Click(object sender, RoutedEventArgs e)
        {
            if (cartItems.Count > 0)
            {
                Common.PopupMessage("모두 결제되었습니다!");
                cartItems.Clear();
                UpdateTotalAmount();
            }
            else
            {
                Common.PopupMessage("결제할 물건이 없습니다!");
            }
            txtTotalAmount.Focus();
        }

        private void btnCash_Click(object sender, RoutedEventArgs e)
        {

            if (cartItems.Count > 0)
            {
                CashCalcWindow ccw = new CashCalcWindow();
                ccw.CalcCompletedEvent += new CashCalcWindow.CalcCompletedEventHandler(CashCalcCompleted);
                ccw.cartList = cartItems;


                //새창을 가운대로
                ccw.Owner = Application.Current.MainWindow; // We must also set the owner for this to work.
                ccw.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                ccw.ShowDialog();
            }
            else
            {
                Common.PopupMessage("결제할 물건이 없습니다!");
            }
            txtTotalAmount.Focus();
        }

      
        private void CashCalcCompleted()
        {
            Common.PopupMessage("모두 결제되었습니다!");
            cartItems.Clear();
            UpdateTotalAmount();
        }






    }
    public struct Item
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public int Count { get; set; }
        public int TotalAmount { get { return Amount * Count; } }
    }
}
