using System.Windows;

namespace PlayMartWpf
{
    class Common
    {

        public static void PopupMessage(string msg)
        {
            AlertWindow aw = new AlertWindow();

            //새창을 가운대로
            aw.Owner = Application.Current.MainWindow; // We must also set the owner for this to work.
            aw.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            aw.Message = msg;
            aw.ShowDialog();
        }




    }
}
