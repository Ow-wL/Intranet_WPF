using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emoji.Wpf;

namespace Intranet03
{
    public partial class Main : Window
    {
        public int UserId { get; private set; }
        public string Username { get; private set; }
        public string Nickname { get; private set; }
        private Emoji.Wpf.TextBlock lblname { get; set; }

        public Main(int userId, string username, string nickname)
        {
            InitializeComponent();
            OpenDashBoard(); // 기본 메인 폼 진입 시 대시보드창
            this.UserId = userId;
            this.Username = username;
            this.Nickname = nickname;
            string name = "사용자: " + Nickname;
            // XAML에서 정의한 lblNickname 컨트롤을 찾아서 속성에 할당합니다.
            lblname = (Emoji.Wpf.TextBlock)FindName("lblNickname");

            // 창이 로드될 때 별명을 lblNickname에 설정합니다.
            Loaded += (sender, e) =>
            {
                if (lblname != null)
                {
                    SetNicknameWithEmoji(name); // 이모지와 텍스트를 분리하여 설정
                }
            };
        }
        private void SetNicknameWithEmoji(string nickname)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            var emojiRegex = new Regex(@"[\uD83C-\uDBFF\uDC00-\uDFFF\u2600-\u27BF\u2B50]+");

            int lastIndex = 0;

            foreach (Match match in emojiRegex.Matches(nickname))
            {
                if (match.Index > lastIndex)
                {
                    var text = nickname.Substring(lastIndex, match.Index - lastIndex);
                    panel.Children.Add(new System.Windows.Controls.TextBlock
                    {
                        Text = text,
                        FontFamily = new FontFamily("Pretendard ExtraBold"),
                        FontSize = 22,
                        Foreground = new SolidColorBrush(Color.FromRgb(224, 224, 224))
                    });
                }

                // EmojiTextBlock 사용하여 이모지 추가
                panel.Children.Add(new System.Windows.Controls.TextBlock
                {
                    Text = match.Value,
                    FontSize = 22,
                    FontFamily = new FontFamily("Segoe UI Emoji") // 컬러 이모지 폰트 설정
                });

                lastIndex = match.Index + match.Length;
            }

            // 마지막에 남은 텍스트 추가
            if (lastIndex < nickname.Length)
            {
                panel.Children.Add(new System.Windows.Controls.TextBlock
                {
                    Text = nickname.Substring(lastIndex),
                    FontFamily = new FontFamily("Pretendard ExtraBold"),
                    FontSize = 22,
                    Foreground = new SolidColorBrush(Color.FromRgb(224, 224, 224))
                });
            }

            lblname.Inlines.Clear();
            foreach (var child in panel.Children)
            {
                if (child is System.Windows.Controls.TextBlock textBlock)
                {
                    lblname.Inlines.Add(new Run(textBlock.Text)
                    {
                        FontFamily = textBlock.FontFamily,
                        FontSize = textBlock.FontSize,
                        Foreground = textBlock.Foreground
                    });
                }
            }
        }


        /*public Main()
        {
            InitializeComponent();
            OpenDashBoard(); // 기본 메인 폼 진입 시 대시보드창
        }*/

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