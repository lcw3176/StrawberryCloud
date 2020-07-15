# StrawberryCloud
클라우드 파일 저장 프로그램

## 데이터베이스: Sqlite
<details>
<summary>user 테이블</summary>
<div markdown="1">
<pre>
<code>

CREATE TABLE user(
        sid integer PRIMARY KEY AUTOINCREMENT,
        name varchar(30) not null,
        password varchar(20) not null)

</pre>
</code>
</div>
</details>


## 통신 규약
|CRUD|DataInfo|Method|Destination|Parameters|
|-----------|--------|--------|--------|--------|
|로그인|Login|GET|LoginView|아이디, 비밀번호|
|회원가입|예정|-|-|-|
|회원탈퇴|예정|-|-|-|
|로그인 성공 시 유저 파일 세팅|Init|GET|ProfileView|"null"|
|새로고침|Folder|GET|ProfileView|"null"|
|폴더 탐색|Folder|GET|ProfileView|폴더 경로|
|다운로드 요청|File|GET|DownloadView|파일 경로|
|다운로드 시작|File|DOWNLOAD|DownloadView|파일 이름, 인덱스|
|업로드 요청|File|POST|DownloadView|파일 경로|
|업로드 시작|File|UPLOAD|DownloadView|바이트 파일, 인덱스|
|업로드 종료 알림|File|UPLOADEND|DownloadView|파일 이름|
|파일 삭제|예정|-|-|-|


