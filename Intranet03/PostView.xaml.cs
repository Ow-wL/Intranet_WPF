using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
    /// PostView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PostView : Window
    {
        private Post.PostItem _post;
        private string _currentUserNickname;
        private string? _originalContent;
        private bool _isEditMode = false;

        public PostView(Post.PostItem post, string currentUserNickname)
        {
            InitializeComponent();

            _post = post;
            _currentUserNickname = currentUserNickname;
            _originalContent = post.Content;

            this.DataContext = post;
            tbContents.Background = new SolidColorBrush(Colors.Transparent);

            // 작성자와 현재 로그인한 사용자가 동일한 경우, 수정 및 삭제 버튼 표시
            if (post.Author == currentUserNickname)
            {
                authorButtons.Visibility = Visibility.Visible;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // 수정 모드 활성화
            _isEditMode = true;

            // 텍스트박스를 수정 가능 상태로 변경
            tbContents.IsReadOnly = false;
            tbContents.Background = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)); // 약간의 배경색 변경
            tbContents.BorderBrush = new SolidColorBrush(Colors.Gray);
            tbContents.Height = 295; 
            tbContents.Focus();

            // 수정 관련 버튼 표시 및 기존 버튼 숨김
            editControls.Visibility = Visibility.Visible;
            authorButtons.Visibility = Visibility.Collapsed;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // 수정 모드 종료
            ExitEditMode();

            // 원래 내용으로 복원
            tbContents.Text = _originalContent;
            _post.Content = _originalContent;
        }

        private void ExitEditMode()
        {
            _isEditMode = false;

            // 텍스트박스를 읽기 전용으로 복원
            tbContents.IsReadOnly = true;
            tbContents.Background = new SolidColorBrush(Colors.Transparent);
            tbContents.BorderBrush = new SolidColorBrush(Colors.Transparent);

            // 버튼 상태 복원
            editControls.Visibility = Visibility.Collapsed;
            authorButtons.Visibility = Visibility.Visible;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 수정된 내용 저장
            try
            {
                string newContent = tbContents.Text;
                bool success = await UpdatePostContent(_post.Id, newContent);

                if (success)
                {
                    // 내용 업데이트 및 수정 모드 종료
                    _post.Content = newContent;
                    _originalContent = newContent;
                    ExitEditMode();
                    MessageBox.Show("게시글이 성공적으로 수정되었습니다.", "수정 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("게시글 수정 중 오류가 발생했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"게시글 수정 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // 삭제 확인 메시지
            MessageBoxResult result = MessageBox.Show("정말로 이 게시글을 삭제하시겠습니까?", "삭제 확인",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = await DeletePost(_post.Id);
                    if (success)
                    {
                        MessageBox.Show("게시글이 성공적으로 삭제되었습니다.", "삭제 완료", MessageBoxButton.OK, MessageBoxImage.Information);

                        // 창 닫기
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("게시글 삭제 중 오류가 발생했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"게시글 삭제 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task<bool> UpdatePostContent(int postId, string newContent)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    string serverUrl = $"http://localhost:3000/posts/{postId}";
                    var updateData = new { content = newContent };
                    var json = JsonSerializer.Serialize(updateData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await httpClient.PutAsync(serverUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"게시글 수정 실패 ({response.StatusCode}): {errorMessage}"); // 서버 콘솔에 오류 로그
                        return false;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP 요청 오류: {ex.Message}"); // 서버 콘솔에 오류 로그
                    return false;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON 직렬화 오류: {ex.Message}"); // 서버 콘솔에 오류 로그
                    return false;
                }
            }
        }

        private async Task<bool> DeletePost(int postId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"http://localhost:3000/posts/{postId}";

                    // DELETE 요청 보내기
                    HttpResponseMessage response = await client.DeleteAsync(url);

                    // 응답 처리
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);

                        return result != null && result.ContainsKey("status") && result["status"].GetString() == "success";
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}