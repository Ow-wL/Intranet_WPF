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
using System.Windows.Threading;

namespace Intranet03
{
    /// <summary>
    /// DashBoard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DashBoard : UserControl
    {
        private  DispatcherTimer _timer;
        public DashBoard()
        {
            InitializeComponent();
            InitializeClock();
        }
        private void InitializeClock()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _timer.Tick += UpdateClock;
                _timer.Start();
            }

            // 초기 시간 설정
            lblTimer.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }

        private void UpdateClock(object sender, EventArgs e)
        {
            // 현재 시간 및 날짜 업데이트
            lblTimer.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }
}
