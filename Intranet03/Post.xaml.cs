using System;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Emoji.Wpf;

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

        // 날짜 선택 이벤트 핸들러 수정
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var datePicker = sender as DatePicker;
            if (datePicker == null) return;

            // DatePicker의 내부 TextBox 찾기
            var textBox = datePicker.Template.FindName("PART_TextBox", datePicker) as System.Windows.Controls.TextBox;
            if (textBox == null) return;

            // 날짜가 선택된 경우
            if (datePicker.SelectedDate.HasValue)
            {
                if (firstSelectedDate == null)
                {
                    // 첫 번째 날짜 선택
                    firstSelectedDate = datePicker.SelectedDate.Value;
                    secondSelectedDate = null; // 두 번째 날짜는 명시적으로 null로 설정
                    textBox.Text = firstSelectedDate.Value.ToString("yyyy-MM-dd");
                }
                else if (secondSelectedDate == null)
                {
                    // 두 번째 날짜 선택
                    DateTime selectedDate = datePicker.SelectedDate.Value;

                    if (selectedDate < firstSelectedDate.Value)
                    {
                        // 두 번째 날짜가 첫 번째 날짜보다 이전인 경우, 두 날짜를 바꿈
                        secondSelectedDate = firstSelectedDate;
                        firstSelectedDate = selectedDate;
                        textBox.Text = $"{firstSelectedDate.Value:yyyy-MM-dd} - {secondSelectedDate.Value:yyyy-MM-dd}";
                    }
                    else
                    {
                        // 정상적인 범위 선택
                        secondSelectedDate = selectedDate;
                        textBox.Text = $"{firstSelectedDate.Value:yyyy-MM-dd} - {secondSelectedDate.Value:yyyy-MM-dd}";
                    }
                }
                else
                {
                    // 범위 선택 후 다시 날짜를 선택하는 경우 (초기화 후 새로운 선택)
                    firstSelectedDate = datePicker.SelectedDate.Value;
                    secondSelectedDate = null;
                    textBox.Text = firstSelectedDate.Value.ToString("yyyy-MM-dd");
                }
            }
            else
            {
                // 날짜 선택이 해제된 경우 (예: 클리어 버튼 클릭)
                firstSelectedDate = null;
                secondSelectedDate = null;
                textBox.Text = string.Empty;
            }
        }

        // 글 작성하기 버튼 누르기
        private void btnPostup_Click(object sender, RoutedEventArgs e)
        {
            PostWrite postWrite = new PostWrite(this.Nickname);
            postWrite.Show();
        }

        private void Post_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPostList(); 
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

            var existingDateColumn = dgPostlist.Columns
                .FirstOrDefault(col => col.Header?.ToString() == "작성일");
            if (existingDateColumn != null)
            {
                dgPostlist.Columns.Remove(existingDateColumn);
            }

            var dateColumn = new DataGridTextColumn
            {
                Header = "작성일",
                Binding = new Binding("Date")
                {
                    StringFormat = "yyyy-MM-dd HH:mm:ss"
                }
            };
            dgPostlist.Columns.Add(dateColumn);
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

        // 글 삭제 구현 우클릭 메뉴
        private void dgPostlist_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid? dataGrid = sender as DataGrid;
            if (dataGrid == null) return;

            DependencyObject? source = e.OriginalSource as DependencyObject;

            while (source != null && !(source is Visual))
            {
                source = LogicalTreeHelper.GetParent(source);
            }

            if (source == null) return;

            DataGridRow? row = FindVisualParent<DataGridRow>(source);
            if (row == null) return;

            PostItem? postItem = row.Item as PostItem;
            if (postItem == null) return;

            ContextMenu contextMenu = new ContextMenu();
            MenuItem deleteMenuItem = new MenuItem { Header = "삭제" };
            deleteMenuItem.Click += (s, args) => ConfirmDelete(postItem);
            contextMenu.Items.Add(deleteMenuItem);
            contextMenu.IsOpen = true;
        }

        private static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject? parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            T? parent = parentObject as T;
            return parent ?? FindVisualParent<T>(parentObject);
        }

        private void ConfirmDelete(PostItem postItem)
        {
            MessageBoxResult result = MessageBox.Show("정말로 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                DeletePost(postItem);
            }
        }

        private async void DeletePost(PostItem postItem)
        {
            if (postItem.Author != this.Nickname)
            {
                MessageBox.Show("삭제 권한이 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var httpClient = new HttpClient())
            {
                try
                {
                    string serverUrl = $"http://localhost:3000/posts/{postItem.Id}?author={this.Nickname}";
                    HttpResponseMessage response = await httpClient.DeleteAsync(serverUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("게시글이 삭제되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadPostList();
                    }
                    else
                    {
                        MessageBox.Show($"게시글 삭제 실패: {response.StatusCode}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"HTTP 요청 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // 더블 클릭 시 게시글 읽기
        private void dgPostlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid? dataGrid = sender as DataGrid;
            if (dataGrid == null) return;
            PostItem? selectedPost = dataGrid.SelectedItem as PostItem;
            if (selectedPost == null) return;

            string currentUserNickname = Nickname;

            PostView postView = new PostView(selectedPost, currentUserNickname);
            postView.Show();
        }


        // 글 검색하기
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchPosts(); 
        }

        // 글 초기화
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            firstSelectedDate = null;
            secondSelectedDate = null;
            dpDate.SelectedDate = null;

            var textBox = dpDate.Template.FindName("PART_TextBox", dpDate) as System.Windows.Controls.TextBox;
            if (textBox != null)
            {
                textBox.Text = string.Empty;
            }

            cbCategory.SelectedIndex = -1;
            tbTitle.Text = string.Empty;
            tbAuthor.Text = string.Empty;

            LoadPostList();
        }

        private async void SearchPosts()
        {
            try
            {
                // 검색 조건을 화면에 표시
                ShowSearchingStatus();

                // 기본 URL
                string baseUrl = "http://localhost:3000/posts";
                Dictionary<string, string> queryParams = new Dictionary<string, string>();

                // 날짜 검색 파라미터 추가
                if (firstSelectedDate.HasValue)
                {
                    string startDate = firstSelectedDate.Value.ToString("yyyy-MM-dd");
                    queryParams.Add("startDate", startDate);

                    if (secondSelectedDate.HasValue)
                    {
                        string endDate = secondSelectedDate.Value.ToString("yyyy-MM-dd");
                        queryParams.Add("endDate", endDate);
                    }
                }

                // 카테고리 검색 파라미터 추가
                if (cbCategory.SelectedIndex > 0) // 0번째는 "전체"라고 가정
                {
                    string? category = (cbCategory.SelectedItem as ComboBoxItem)?.Content.ToString();
                    if (!string.IsNullOrEmpty(category))
                    {
                        queryParams.Add("category", category);
                    }
                }

                // 제목 검색 파라미터 추가
                if (!string.IsNullOrEmpty(tbTitle.Text))
                {
                    queryParams.Add("title", tbTitle.Text.Trim());
                }

                // 작성자 검색 파라미터 추가
                if (!string.IsNullOrEmpty(tbAuthor.Text))
                {
                    queryParams.Add("author", tbAuthor.Text.Trim());
                }

                // 쿼리 파라미터가 있으면 URL에 추가
                string url = baseUrl;
                if (queryParams.Count > 0)
                {
                    url += "?" + string.Join("&", queryParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
                }

                // 디버그를 위한 로그
                Console.WriteLine($"Searching with URL: {url}");

                // API 호출
                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(jsonString))
                        {
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            List<PostItem>? posts = JsonSerializer.Deserialize<List<PostItem>>(jsonString, options);

                            // 결과를 DataGrid에 바인딩
                            dgPostlist.ItemsSource = posts;

                            // 검색 결과가 없는 경우
                            if (posts == null || posts.Count == 0)
                            {
                                MessageBox.Show("검색 결과가 없습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            MessageBox.Show("검색 결과가 없습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                            dgPostlist.ItemsSource = new List<PostItem>(); // 빈 결과 표시
                        }
                    }
                    else
                    {
                        MessageBox.Show($"검색 실패: {response.StatusCode}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"검색 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                HideSearchingStatus();
            }
        }

        // 검색 상태를 표시하는 메서드
        private void ShowSearchingStatus()
        {
            Mouse.OverrideCursor = Cursors.Wait;
        }

        // 검색 상태 표시를 해제하는 메서드
        private void HideSearchingStatus()
        {
            Mouse.OverrideCursor = null;
        }

    }
}
