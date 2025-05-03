----

# 🖥️ C# WPF를 통한 프로그램 개발 🖥️
+ 개발기간 : 25-04-27 ~
+ 개발자명 : 이주형(Ow-wL)
+ 개발목표 : C# WPF를 통해 실제 사용자가 사용할 수 있는 상용 프로그램 개발
+ 세부목표 : 
    1. 클라우드 DB를 통해 모든 사용자가 실시간 정보 공유를 할 수 있게 개발
    2. 원하는 문구를 사용자가 저장 후 클립보드에 저장할 수 있게 개발
    3. 사용자가 원하는 글을 게시판에 게시 및 다른 사용자가 확인할 수 있는 기능
    4. 메인창을 통해 최근에 올라온 글, 접속자 수, 현재 시각 등 확인할 수 있는 기능

----

## Environment
    Visual Studio 2022

  ![Visual Studio Code](https://img.shields.io/badge/Visual%20Studio%20Code-0078d7.svg?style=for-the-badge&logo=visual-studio-code&logoColor=white)
  ![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white)
  ![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
  ![JavaScript](https://img.shields.io/badge/javascript-%23323330.svg?style=for-the-badge&logo=javascript&logoColor=%23F7DF1E)
  ![NodeJS](https://img.shields.io/badge/node.js-6DA55F?style=for-the-badge&logo=node.js&logoColor=white)
  ![MySQL](https://img.shields.io/badge/mysql-4479A1.svg?style=for-the-badge&logo=mysql&logoColor=white)

  ![Git](https://img.shields.io/badge/git-%23F05033.svg?style=for-the-badge&logo=git&logoColor=white)
  ![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)
  ![Notion](https://img.shields.io/badge/Notion-%23000000.svg?style=for-the-badge&logo=notion&logoColor=white)

  ![Windows 11](https://img.shields.io/badge/Windows%2011-%230079d5.svg?style=for-the-badge&logo=Windows%2011&logoColor=white)
  

----

## TODO List
### MainForm
+ ~~MainWindow - { "Main", "Post", "Solution", "Customer Service", "Personal Setting", ".Admin Menu" }~~
    + { "Main" } = ("Title", "Time", "Connected user", "Notice")
    + { "Post" } = (~~"Add Post"~~, "Read Post", "Remove Post", "Edit Post", "Search Post")
    + { "Solution" } = ("GM's Answer", "YM's Tip", "OWL's Tip")
    + { "Customer Service" } = ("YM's Tip", "Easy Copy", "Templete")
    + { "Personal Setting" } = ("Change NickName", "Change Password", "Change Profile")
    + { "Admin Menu" } = ("Post Modify", "Permission Modify", "User Modify")
+ 최근에 올라온 게시물 대시보드에서 확인할 수 있는 기능 추가
+ 대시보드 창 실시간 접속 유저 표기

### Post
+ ~~DB 통신 시스템 SQLite -> MySQL로 변경~~
+ ~~게시판 C# 코드 구현 글 작성 기능 글 목록 보기 기능 추가~~
+ 게시글 내용 확인 기능
+ 게시글 작성자에 한해 수정, 삭제 기능 (admin 포함)
+ ~~Emoji 출력~~

## 설치한 NuGet

+ System.Net.Http.Json (DB 통신)
+ MahApps.Metro (내용 미리보기)
+ ControlzEx (위 설치 시 같이 설치됨)
+ Emoji.Wpf (컬러 이모지)

## Day Work

<details>
<summary> 25-04-27 ( 1일차 ) </summary>
로그인 폼 디자인 <br>
회원가입 폼 디자인<br>
<br>
</details>

<details>
<summary>25-04-30 ( 2일차 ) </summary>
 로그인 & 회원가입 서버 DB 시스템 구현 <br>
 메인 폼 디자인 <br>
<br>
</details>

<details>
<summary>25-05-01 ( 3일차 ) </summary>
회원가입 시 별명 설정<br>
대시보드 & 게시판 xaml 디자인<br>
메인폼 별명 추가(이모지 출력)<br>
Emoji.wpf 사용을 위한 .NET 8.0 WPF로 변경 <br>
컬러 이모지 구현 <br>
<br>

</details>

<details>
<summary>25-05-02 ( 4일차 ) </summary>
SQLite -> MySQL 변환<br>
게시판 디자인 <br>
<br>

</details>

<details>
<summary>25-05-03 ( 5일차 ) </summary>
게시판 디자인 수정<br>
게시판 글 작성 기능 구현 <br>
게시판 글 목록 출력 <br>
로그인 & 회원가입 방식 개선 <br>
<br>

</details>