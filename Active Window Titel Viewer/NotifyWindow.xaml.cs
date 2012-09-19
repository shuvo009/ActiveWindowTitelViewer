using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls.Primitives;
namespace Active_Window_Titel_Viewer
{
	/// <summary>
	/// Interaction logic for NotifyWindow.xaml
	/// </summary>
	public partial class NotifyWindow : UserControl
	{
        private bool isClosing = false;
		public NotifyWindow()
		{
			this.InitializeComponent();
            TaskbarIcon.AddBalloonClosingHandler(this, OnBallonClosing);
		}

        private void OnBallonClosing(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            isClosing = true;
        }

		private void notifiWindowCloseBUtton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            if (isClosing)
            {
                return;
            }
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.ResetBalloonCloseTimer();
            taskbarIcon.CloseBalloon();
		}

        private void OnFadeOutCompleted(object sender, EventArgs e)
        {
            Popup pp = (Popup)Parent;
            pp.IsOpen = false;
            
        }
	}
}