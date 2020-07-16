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
|로그인 성공 시 유저 파일 세팅|Init|GET|ProfileView|"null"|
|새로고침|Folder|GET|ProfileView|"null"|
|폴더 탐색|Folder|GET|ProfileView|폴더 경로|
|다운로드 요청|File|GET|DownloadView|파일 경로|
|다운로드 시작|File|DOWNLOAD|DownloadView|파일 이름, 인덱스|
|업로드 요청|File|POST|DownloadView|파일 경로|
|업로드 시작|File|UPLOAD|DownloadView|바이트 파일, 인덱스|
|업로드 종료 알림|File|UPLOADEND|DownloadView|파일 이름|
|파일 삭제|File|DELETE|ProfileView|파일 경로, 파일 이름|

## 작동 방식 및 구현 기능
### 클라이언트
* WPF, MVVM 패턴 적용
* 소켓 통신(TCP)
* 폴더 생성 및 탐색 기능
* 파일 다운로드 및 업로드
* 다운로드 중 일시정지, 다시시작, 취소 가능

### 서버
#### Program.cs
* 유저 접속 시 ClientThread 생성과 함께 접속자 ip 표시

#### ClientThread.cs
* 유저 요청 감시, 알맞은 응답 제공함
* Reflection 사용으로 유저 요청에 맞는 함수 제공
* 유저가 보내왔던 Destination과 Method를 그대로 다시 전달, 알맞은 뷰에 적절한 동작이 가능하게 해줌

#### Routes.Index.cs
* 유저가 요청한 사항을 알맞은 역할로 분배 후 결과값 리턴
* Login(로그인 요청), Init(로그인 성공 시 정보 전달), File(파일에 관한 요청), Folder(폴더에 관련된 요청) 

#### Storage 폴더
* File.cs와 Folder.cs 존재, 역할 구분
* 존재하는 파일과 폴더의 이름 가져오기, 생성, 전달, 변경 등을 담당

#### Enumerate 폴더
* Datainfo.cs, Method.cs , Destination.cs로 구성
* Datainfo.cs: 전송받은 데이터가 진행하려는 일의 범위 ex) 로그인, 파일, 폴더..
* Method.cs: 데이터가 서버에 영향을 끼칠 방식 ex) 가져오기, 다운로드, 삭제, 업로드..
* Destination.cs: 처리된 데이터가 영향을 끼칠 클라이언트의 View ex)로그인 뷰, 다운로드 뷰..

#### Database 폴더
* 회원정보 DB 테이블과 유저 정보 읽어오기, 쓰기 등이 존재

## 작동 모습
### 홈 화면, 파일 업로드
<div>
  <img width="200" src="https://user-images.githubusercontent.com/59993347/87643772-3b244800-c786-11ea-9e82-02279dd4a1c0.png">
  <img width="200" src="https://user-images.githubusercontent.com/59993347/87643764-395a8480-c786-11ea-825d-9c252df80ae9.png">
 </div>

### 새 폴더 만들기 및 삭제, 다운로드
<div>
  <img width="200" src="https://user-images.githubusercontent.com/59993347/87643765-39f31b00-c786-11ea-901e-081c96a6463d.png">
  <img width="200" src="https://user-images.githubusercontent.com/59993347/87643770-3a8bb180-c786-11ea-899f-cd2a5fbf7009.png">
  <img width="200" src="https://user-images.githubusercontent.com/59993347/87643771-3a8bb180-c786-11ea-9ccb-3a5896e2c4d1.png">
 </div>
