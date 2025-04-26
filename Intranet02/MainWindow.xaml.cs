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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Intranet02
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AddPlaceholder(IDtext, "아이디");
            AddPlaceholder(PWtext, "비밀번호");

            IDtext.TextChanged += (s, e) =>
            {
                UpdateIDPlaceholder();
            };

            PWtext.PasswordChanged += (s, e) =>
            {
                UpdatePasswordPlaceholder();
            };
        }
        private void AddPlaceholder(Control control, string placeholder)
        {
            control.Loaded += (s, e) =>
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(control);
                if (adornerLayer != null)
                {
                    var placeholderAdorner = new PlaceholderAdorner(control, placeholder);
                    adornerLayer.Add(placeholderAdorner);
                }
            };
        }
        private void UpdateIDPlaceholder()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(IDtext);
            if (adornerLayer != null)
            {
                var adorner = adornerLayer.GetAdorners(IDtext)?.FirstOrDefault();
                if (adorner != null)
                {
                    adornerLayer.Remove(adorner);
                }

                if (string.IsNullOrEmpty(IDtext.Text))
                {
                    var placeholderAdorner = new PlaceholderAdorner(IDtext, "아이디를 입력하세요");
                    adornerLayer.Add(placeholderAdorner);
                }
            }
        }
        private void UpdatePasswordPlaceholder()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(PWtext);
            if (adornerLayer != null)
            {
                var adorner = adornerLayer.GetAdorners(PWtext)?.FirstOrDefault();
                if (adorner != null)
                {
                    adornerLayer.Remove(adorner);
                }

                if (string.IsNullOrEmpty(PWtext.Password))
                {
                    var placeholderAdorner = new PlaceholderAdorner(PWtext, "비밀번호를 입력하세요");
                    adornerLayer.Add(placeholderAdorner);
                }
            }
        }

        private void Loginbtn_Click(object sender, RoutedEventArgs e)
        {
            string username = IDtext.Text;
            string password = PWtext.Password;

            // 간단한 로그인 체크 (예시)
            if (username == "admin" && password == "1234")
            {
                MessageBox.Show("Login Successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // TODO: 메인 윈도우 열기 또는 다른 작업
            }
            else
            {
                MessageBox.Show("Invalid Username or Password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Signupbtn_Click(object sender, RoutedEventArgs e)
        {
            // 회원가입 버튼 클릭 시 동작
            MessageBox.Show("회원가입 기능은 아직 구현되지 않았습니다.", "정보", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
