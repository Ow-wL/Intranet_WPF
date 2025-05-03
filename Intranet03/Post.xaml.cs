using System;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace Intranet03
{
    public partial class Post : UserControl
    {
        private DateTime? firstSelectedDate = null;
        private DateTime? secondSelectedDate = null;
        public string Nickname { get; set; }

        public Post(string nickname)
        {
            InitializeComponent();
            this.Nickname = nickname;
            this.Loaded += Post_Loaded;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var datePicker = sender as DatePicker;
            if (datePicker == null) return;

            // 템플릿에서 PART_TextBox를 찾습니다.
            var textBox = datePicker.Template.FindName("PART_TextBox", datePicker) as TextBox;
            if (textBox == null) return;

            if (datePicker.SelectedDate.HasValue)
            {
                if (firstSelectedDate == null)
                {
                    firstSelectedDate = datePicker.SelectedDate.Value;
                    textBox.Text = firstSelectedDate.Value.ToString("yyyy-MM-dd");
                }
                else if (secondSelectedDate == null)
                {
                    secondSelectedDate = datePicker.SelectedDate.Value;
                    if (datePicker.SelectedDate.Value < firstSelectedDate.Value)
                    {
                        // 두 번째 날짜가 첫 번째 날짜보다 이전인 경우
                        firstSelectedDate = secondSelectedDate;
                        textBox.Text = firstSelectedDate.Value.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        textBox.Text = $"{firstSelectedDate.Value:yyyy-MM-dd} - {secondSelectedDate.Value:yyyy-MM-dd}";

                    }
                }
                else
                {
                    // 두 날짜가 이미 선택된 경우 초기화
                    firstSelectedDate = datePicker.SelectedDate.Value;
                    secondSelectedDate = null;
                    textBox.Text = firstSelectedDate.Value.ToString("yyyy-MM-dd");
                }
            }
        }

        // 글 작성하기 버튼 누르기
        private void btnPostup_Click(object sender, RoutedEventArgs e)
        {
            PostWrite postWrite = new PostWrite(this.Nickname);
            postWrite.Show();
        }

        // 글 목록 조회 버튼
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Post_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPostList(); // 데이터 로드
        }

        private async Task<List<PostItem>> GetPostListFromApi()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    string serverUrl = "http://localhost:3000/posts"; // Node.js 서버 주소
                    HttpResponseMessage response = await httpClient.GetAsync(serverUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(jsonString)) 
                        {
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            List<PostItem>? posts = JsonSerializer.Deserialize<List<PostItem>>(jsonString, options); 
                            return posts ?? new List<PostItem>(); 
                        }
                        else
                        {
                            MessageBox.Show("Server Data Error", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                            return new List<PostItem>();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"게시글 목록 로드 실패: {response.StatusCode}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return new List<PostItem>();
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"HTTP 요청 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new List<PostItem>();
                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"JSON 파싱 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new List<PostItem>();
                }
            }
        }

        private async void LoadPostList()
        {
            List<PostItem> postList = await GetPostListFromApi();
            dgPostlist.ItemsSource = postList;

            DataGridTextColumn? dateColumn = dgPostlist.Columns.FirstOrDefault(col => col.Header.ToString() == "작성일") as DataGridTextColumn;
            if (dateColumn != null)
            {
                dateColumn.Binding.StringFormat = "yyyy-MM-dd"; // 원하는 날짜 형식 지정
            }
        }

        // 글 목록 가져오기
        public class PostItem
        {
            public int Id { get; set; }
            public string? Author { get; set; }
            public string? Category { get; set; }
            public string? Title { get; set; }
            public string? Content { get; set; }
            public DateTime Date { get; set; }
            public int Views { get; set; }
        }
    }
}
