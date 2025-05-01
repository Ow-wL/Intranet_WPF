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

namespace Intranet02
{
    public partial class SignUpWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        public SignUpWindow()
        {
            InitializeComponent();

            AddPlaceholder(IDtext, "아이디");
            AddPlaceholder(NameText, "별명");
            AddPlaceholder(PWtext, "비밀번호");
            AddPlaceholder(PWChecktext, "비밀번호 확인");

            IDtext.TextChanged += (s, e) =>
            {
                UpdateIDPlaceholder();
            };

            NameText.TextChanged += (s, e) =>
            {
                UpdateNamePlaceholder();
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
        private void UpdateNamePlaceholder()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(NameText);
            if (adornerLayer != null)
            {
                var adorner = adornerLayer.GetAdorners(NameText)?.FirstOrDefault();
                if (adorner != null)
                {
                    adornerLayer.Remove(adorner);
                }

                if (string.IsNullOrEmpty(NameText.Text))
                {
                    var placeholderAdorner = new PlaceholderAdorner(NameText, "별명");
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
        private async void Registerbtn_Click(object sender, RoutedEventArgs e)
        {
            string id = IDtext.Text; // ID 필드는 그대로 사용한다고 가정
            string password = PWtext.Password;
            string passwordCheck = PWChecktext.Password;
            string nickname = NameText.Text; // 별명 입력 필드 값 가져오기

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck) || string.IsNullOrEmpty(nickname))
            {
                MessageBox.Show("모든 필드를 입력해주세요.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != passwordCheck)
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
                PWtext.Clear();
                PWChecktext.Clear();
                PWtext.Focus();
                return;
            }

            var response = await RegisterUser(id, password, nickname);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                try
                {
                    var registerResult = System.Text.Json.JsonSerializer.Deserialize<RegisterResponse>(jsonResponse);
                    if (registerResult?.status == "success")
                    {
                        MessageBox.Show($"회원가입 성공! 사용자 {registerResult.nickname}!", "정보", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoginWindow LoginWindow = new LoginWindow();
                        LoginWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"회원가입 실패: {registerResult?.message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    MessageBox.Show($"회원가입 응답 처리 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                string errorMessage = response.ReasonPhrase;
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
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
                else
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
                MessageBox.Show($"회원가입 실패: {errorMessage}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<HttpResponseMessage> RegisterUser(string id, string password, string name)
        {
            var user = new { username = id, password = password, nickname = name };
            return await client.PostAsJsonAsync("http://localhost:3000/register", user);
        }

        public class RegisterResponse
        {
            public string status { get; set; }
            public string message { get; set; }
            public int id { get; set; }
            public string nickname { get; set; } 
        }

        private void returnbtn_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow LoginWindow = new LoginWindow();
            LoginWindow.Show();
            this.Close();
        }
    }
}
