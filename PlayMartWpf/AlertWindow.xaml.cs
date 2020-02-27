using System;
using System.Windows;
using System.Windows.Threading;

namespace PlayMartWpf
{
    /// <summary>
    /// AlertWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlertWindow : Window
    {
        public string Message { get; set; }

        //타이머 선언
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer(); 

        public AlertWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            label.Text = Message;

            //타이머에 이벤트 추가 및 인터벌 셋팅
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start(); 
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //타이머 이벤트 실행히 실행될 코드

            //아래는 델리게이트 사용법 : 쓰레드 타이머를 사용하였기 때문에 UI Thread와 다른 Thread라서 델리게이트를 사용해야 함
            Dispatcher.Invoke(DispatcherPriority.Background, (Action)delegate()
            {
                this.Close();
            });
        } 
        

    }
}
