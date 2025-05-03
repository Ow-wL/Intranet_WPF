using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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

namespace Intranet03
{
    /// <summary>
    /// PostWrite.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PostWrite : Window
    {
        private string Nickname;
        public string? SelectedCategory { get; set; }
        public PostWrite(string nickname)
        {
            InitializeComponent();
            this.Nickname = nickname;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCategory.SelectedItem is ComboBoxItem selectedItem)
            {
                SelectedCategory = selectedItem.Content.ToString(); // 선택된 텍스트를 속성에 저장
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string title = tbTitle.Text;
            string content = new TextRange(rtbContent.Document.ContentStart, rtbContent.Document.ContentEnd).Text.Trim();
            string category = SelectedCategory ?? ""; // category를 string으로 선언하고 SelectedCategory 값을 바로 할당
            if (string.IsNullOrWhiteSpace(category)) // null 또는 공백 문자열 검사
            {
                MessageBox.Show("카테고리 선택 Error", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string author = Nickname;

            // 입력 값 유효성 검사
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(category))
            {
                MessageBox.Show("제목, 내용, 작성자 명, 카테고리를 모두 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 보낼 데이터 객체 생성
            var postData = new
            {
                author = author,
                title = title,
                content = content,
                category = category
            };

            // JSON으로 직렬화
            string jsonString = JsonSerializer.Serialize(postData);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // HttpClient 인스턴스 생성 (using 문을 사용하여 리소스 관리)
            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Node.js 서버의 엔드포인트 주소
                    string serverUrl = "http://localhost:3000/posts"; // 서버 주소 및 포트 번호 확인

                    // POST 요청 보내기 (비동기)
                    HttpResponseMessage response = await httpClient.PostAsync(serverUrl, httpContent);

                    // 응답 상태 코드 확인
                    if (response.IsSuccessStatusCode)
                    {
                        string responseJson = await response.Content.ReadAsStringAsync();
                        // 응답 JSON 파싱 (예: 성공 메시지 처리)
                        var responseData = JsonSerializer.Deserialize<JsonElement>(responseJson);
                        string message = responseData.TryGetProperty("message", out var messageElement)
                            ? (messageElement.GetString() ?? "")
                            : "";
                        MessageBox.Show(message, "게시글 등록 성공", MessageBoxButton.OK, MessageBoxImage.Information);

                        // 성공 후 입력 필드 초기화 (선택 사항)
                        tbTitle.Clear();
                        rtbContent.Document.Blocks.Clear();
                        cbCategory.SelectedIndex = -1; // 콤보박스 선택 해제
                        this.Close();
                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"게시글 등록 실패: {response.StatusCode} - {errorResponse}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        // 오류 응답 내용 로깅 또는 추가 처리
                        Console.WriteLine($"Error Response: {errorResponse}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"HTTP 요청 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    // 네트워크 오류 처리
                    Console.WriteLine($"HTTP Request Exception: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"JSON 처리 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    // JSON 직렬화/역직렬화 오류 처리
                    Console.WriteLine($"JSON Exception: {ex.Message}");
                }
            }
        }
    }
}
