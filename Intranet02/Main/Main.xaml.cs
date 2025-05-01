using System;
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


namespace Intranet02
{
    /// <summary>
    /// Main.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Main : Window
    {
        private DispatcherTimer _timer; 
        public Main()
        {
            InitializeComponent();
            OpenDashBoard(); // 기본 메인 폼 진입 시 대시보드창
        }

        private void OpenDashBoard()
        {
            ContentArea.Children.Clear();
            var dashBoardControl = new DashBoard(); 
            Grid.SetColumn(dashBoardControl, 1); 
            ContentArea.Children.Add(dashBoardControl);
        }
        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            OpenDashBoard();
        }

        private void btnPost_Click(object sender, RoutedEventArgs e)
        {
            // GridMain의 기존 콘텐츠를 제거
            ContentArea.Children.Clear();

            // Post UserControl을 동적으로 추가
            var postControl = new Post(); // Post를 UserControl로 변경
            Grid.SetColumn(postControl, 1); // Grid의 두 번째 열에 배치
            ContentArea.Children.Add(postControl);
        }

    }
}