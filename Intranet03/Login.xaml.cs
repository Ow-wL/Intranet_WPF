using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
using static Intranet03.Post;

namespace Intranet03
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : Window
    {
        private readonly HttpClient client = new HttpClient();
        public Login()
        {
            InitializeComponent();
            /*AddPlaceholder(IDtext, "아이디");
            AddPlaceholder(PWtext, "비밀번호");

            IDtext.TextChanged += (s, e) =>
            {
                UpdateIDPlaceholder();
            };

            PWtext.PasswordChanged += (s, e) =>
            {
                UpdatePasswordPlaceholder();
            };*/

            this.PreviewKeyDown += LoginWindow_PreviewKeyDown; // 엔터키 입력 시 로그인 버튼 클릭 이벤트 호출
        }

        private void LoginWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Loginbtn_Click(Loginbtn, new RoutedEventArgs());
            }
        }
        /*private void AddPlaceholder(Control control, string placeholder)
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
        }*/

        private async void Loginbtn_Click(object sender, RoutedEventArgs e)
        {
            string username = IDtext.Text;
            string password = PWtext.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("아이디와 비밀번호를 모두 입력해주세요.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 서버에 로그인 요청
            var response = await LoginUser(username, password);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                try
                {
                    var loginResult = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(jsonResponse);
                    if (loginResult?.status == "success" && loginResult != null)
                    {
                        MessageBox.Show($"로그인 성공! 사용자 {loginResult.nickname}", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
                        int userId = loginResult.userId ?? 0;
                        string username_ = loginResult.username ?? "";
                        string nickname = loginResult.nickname ?? "";
                        string password_ = loginResult.password ?? "";
                        Main mainWindow = new Main(userId, username_, nickname, password_);
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"로그인 실패: {loginResult?.message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    MessageBox.Show($"로그인 응답 처리 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                string errorMessage = response?.ReasonPhrase ?? "Null Error";
                if (response != null && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorResult = System.Text.Json.JsonSerializer.Deserialize<ErrorResponse>(errorContent);
                        if (errorResult?.message != null)
                        {
                            errorMessage = errorResult.message;
                        }
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        // JSON 파싱 실패 시 기본 오류 메시지 사용
                    }
                }
                MessageBox.Show($"로그인 실패: {errorMessage}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<HttpResponseMessage> LoginUser(string username, string password)
        {
            var user = new { username = username, password = password };
            return await client.PostAsJsonAsync("http://localhost:3000/login", user);
        }

        private void Signupbtn_Click(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("회원가입으로 이동합니다.", "정보", MessageBoxButton.OK, MessageBoxImage.Information);
            Register registerwindow = new Register();
            registerwindow.Show();
            this.Close();
        }
    }
    public class LoginResponse
    {
        public string? status { get; set; }
        public string? message { get; set; }
        public int? userId { get; set; } // 사용자 ID
        public string? username { get; set; } // 실제 ID
        public string? nickname { get; set; } // 유저 별명
        public string? password { get; set; } // 유저 별명
    }

    public class ErrorResponse
    {
        public string? status { get; set; }
        public string? message { get; set; }
    }
}
