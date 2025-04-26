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

namespace Intranet02
{
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();

            AddPlaceholder(IDtext, "아이디");
            AddPlaceholder(PWtext, "비밀번호");
            AddPlaceholder(PWChecktext, "비밀번호 확인");

            IDtext.TextChanged += (s, e) =>
            {
                UpdateIDPlaceholder();
            };

            PWtext.PasswordChanged += (s, e) =>
            {
                UpdatePasswordPlaceholder();
            };

            PWChecktext.PasswordChanged += (s, e) =>
            {
                UpdatePasswordCheckPlaceholder();
            };
            this.PreviewKeyDown += SignUpWindow_PreviewKeyDown;
        }
        private void SignUpWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Registerbtn_Click(Registerbtn, new RoutedEventArgs());
            }
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
                    var placeholderAdorner = new PlaceholderAdorner(IDtext, "아이디");
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
                    var placeholderAdorner = new PlaceholderAdorner(PWtext, "비밀번호");
                    adornerLayer.Add(placeholderAdorner);
                }
            }
        }
        private void UpdatePasswordCheckPlaceholder()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(PWChecktext);
            if (adornerLayer != null)
            {
                var adorner = adornerLayer.GetAdorners(PWChecktext)?.FirstOrDefault();
                if (adorner != null)
                {
                    adornerLayer.Remove(adorner);
                }

                if (string.IsNullOrEmpty(PWChecktext.Password))
                {
                    var placeholderAdorner = new PlaceholderAdorner(PWChecktext, "비밀번호 확인");
                    adornerLayer.Add(placeholderAdorner);
                }
            }
        }

        private void Registerbtn_Click(object sender, RoutedEventArgs e)
        {
            // 회원가입 버튼 클릭 시 처리 로직
            string id = IDtext.Text;
            string password = PWtext.Password;
            string passwordCheck = PWChecktext.Password;
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))
            {
                MessageBox.Show("모든 필드를 입력해주세요.");
                return;
            }
            if (password != passwordCheck)
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.");
                PWtext.Clear();
                PWChecktext.Clear();
                PWtext.Focus();
                return;
            }
            MessageBox.Show("회원가입 시스템 구현 예정 / DB 형식", "정보", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void returnbtn_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow LoginWindow = new LoginWindow();
            LoginWindow.Show();
            this.Close();
        }
    }
}
