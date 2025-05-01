using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        public int UserId { get; private set; }
        public string Username { get; private set; }
        public string Nickname { get; private set; }
        private Label lblname { get; set; }

        public Main(int userId, string username, string nickname)
        {
            InitializeComponent();
            OpenDashBoard(); // 기본 메인 폼 진입 시 대시보드창
            this.UserId = userId;
            this.Username = username;
            this.Nickname = nickname;

            // XAML에서 정의한 lblNickname 컨트롤을 찾아서 속성에 할당합니다.
            lblname = (Label)FindName("lblNickname");

            // 창이 로드될 때 별명을 lblNickname에 설정합니다.
            Loaded += (sender, e) =>
            {
                if (lblname != null)
                {
                    SetNicknameWithEmoji(Nickname); // 이모지와 텍스트를 분리하여 설정
                }
            };
        }
        private void SetNicknameWithEmoji(string nickname)
        {
            // TextBlock 생성
            var textBlock = new TextBlock();

            // 이모지 정규식 (유니코드 범위)
            var emojiRegex = new Regex(@"[\u1F600-\u1F64F\u1F300-\u1F5FF\u1F680-\u1F6FF\u1F700-\u1F77F\u2600-\u26FF\u2700-\u27BF\u2B50]+");

            int lastIndex = 0;

            // 닉네임 문자열에서 이모지와 텍스트를 분리
            foreach (Match match in emojiRegex.Matches(nickname))
            {
                // 이모지 이전의 일반 텍스트 추가
                if (match.Index > lastIndex)
                {
                    textBlock.Inlines.Add(new Run(nickname.Substring(lastIndex, match.Index - lastIndex))
                    {
                        FontFamily = new FontFamily("Pretendard ExtraBold") // 일반 텍스트 폰트
                    });
                }

                // 이모지 추가
                textBlock.Inlines.Add(new Run(match.Value)
                {
                    FontFamily = new FontFamily("Segoe UI Emoji") // 이모지 폰트
                });

                lastIndex = match.Index + match.Length;
            }

            // 마지막 남은 일반 텍스트 추가
            if (lastIndex < nickname.Length)
            {
                textBlock.Inlines.Add(new Run(nickname.Substring(lastIndex))
                {
                    FontFamily = new FontFamily("Pretendard ExtraBold") // 일반 텍스트 폰트
                });
            }

            // lblname에 TextBlock 설정
            lblname.Content = textBlock;
        }


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