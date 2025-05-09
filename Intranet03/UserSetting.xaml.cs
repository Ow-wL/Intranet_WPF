using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;

namespace Intranet03
{
    /// <summary>
    /// UserSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserSetting : UserControl, INotifyPropertyChanged
    {
        // 사용자 설정 객체
        private UserSettingsModel _userSettings;

        public event PropertyChangedEventHandler PropertyChanged;
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string PW { get; private set; }

        public UserSetting(string username, string nickname, string password)
        {
            InitializeComponent();
            this.ID = username;
            this.Name = nickname;
            this.PW = password; ;

            // 초기 사용자 설정 로드
            LoadUserSettings();

            // 데이터 컨텍스트 설정
            this.DataContext = this;
        }

        private void LoadUserSettings()
        {
            try
            {
                // 실제 구현에서는 설정을 로드하는 코드를 추가
                // 예: 데이터베이스, 설정 파일 등에서 로드
                _userSettings = new UserSettingsModel
                {
                    Nickname = "사용자",
                    IsDarkTheme = true,
                    ColorTheme = "Blue",
                    RememberWindowPosition = true,
                    NotificationsEnabled = true,
                    NotificationSoundsEnabled = true
                };

                // UI에 설정 적용
                ApplySettingsToUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정을 로드하는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySettingsToUI()
        {
            // 별명 설정
            txtNickname.Text = _userSettings.Nickname;

            // 테마 설정은 라디오버튼 이벤트로 처리

            // 창 위치 기억하기
            //toggleWindowPos.IsOn = _userSettings.RememberWindowPosition;

            // 알림 설정
            //toggleNotification.IsOn = _userSettings.NotificationsEnabled;
            //toggleNotificationSound.IsOn = _userSettings.NotificationSoundsEnabled;
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // UI에서 설정 값 가져오기
                //_userSettings.Nickname = txtNickname.Text;
                //_userSettings.RememberWindowPosition = toggleWindowPos.IsOn;
                //_userSettings.NotificationsEnabled = toggleNotification.IsOn;
                //_userSettings.NotificationSoundsEnabled = toggleNotificationSound.IsOn;

                // 설정 저장 처리
                SaveUserSettings();

                MessageBox.Show("설정이 저장되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정을 저장하는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveUserSettings()
        {
            // 실제 구현에서는 설정을 저장하는 코드를 추가
            // 예: 데이터베이스, 설정 파일 등에 저장
            Console.WriteLine("사용자 설정 저장됨");
        }

        private void NicknameChange_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNickname.Text))
            {
                MessageBox.Show("별명을 입력해주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _userSettings.Nickname = txtNickname.Text;
            MessageBox.Show("별명이 변경되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PasswordChange_Click(object sender, RoutedEventArgs e)
        {
            // 비밀번호 변경 다이얼로그 표시
            // 실제 구현에서는 별도의 다이얼로그를 표시
            MessageBox.Show("비밀번호 변경 기능은 아직 구현되지 않았습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            // 확인 다이얼로그 표시
            MessageBoxResult result = MessageBox.Show("모든 설정을 초기화하시겠습니까?", "설정 초기화", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 설정 초기화 코드
                _userSettings = new UserSettingsModel
                {
                    Nickname = "사용자",
                    IsDarkTheme = true,
                    ColorTheme = "Blue",
                    RememberWindowPosition = false,
                    NotificationsEnabled = true,
                    NotificationSoundsEnabled = true
                };

                // UI에 설정 적용
                ApplySettingsToUI();

                MessageBox.Show("모든 설정이 초기화되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // 확인 다이얼로그 표시
            MessageBoxResult result = MessageBox.Show("로그아웃하시겠습니까?", "로그아웃", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 로그아웃 처리
                // 실제 구현에서는 로그아웃 처리 코드 추가
                MessageBox.Show("로그아웃되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ColorTheme_Click(object sender, RoutedEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                // 선택된 테마 색상 적용
                // 실제 구현에서는 선택된 색상을 저장하고 적용하는 코드 추가
                _userSettings.ColorTheme = border.Background.ToString();

                // 선택된 색상 테마에 시각적 효과 적용
                ApplyColorThemeSelection(border);
            }
        }

        private void ApplyColorThemeSelection(Border selectedBorder)
        {
            // 모든 색상 테마 Border에서 효과 제거 후 선택된 것만 효과 적용
            // 실제 구현 시에는 모든 테마 Border에 접근하는 방식으로 수정 필요
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OpenColorPalette_Click(object sender, RoutedEventArgs e)
        {
            // 색상 팔레트 열기
            // 실제 구현에서는 색상 팔레트를 여는 코드 추가
            MessageBox.Show("색상 팔레트 기능은 아직 구현되지 않았습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    // 사용자 설정 모델 클래스
    public class UserSettingsModel
    {
        public string Nickname { get; set; }
        public bool IsDarkTheme { get; set; }
        public string ColorTheme { get; set; }
        public bool RememberWindowPosition { get; set; }
        public bool NotificationsEnabled { get; set; }
        public bool NotificationSoundsEnabled { get; set; }
    }
}