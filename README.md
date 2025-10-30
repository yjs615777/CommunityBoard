## CommunityBoard
사내 커뮤니티 게시판 프로젝트
ASP.NET Core MVC + Entity Framework Core 기반의 웹 게시판 애플리케이션
사용자 회원가입 / 로그인 / 게시글 / 댓글 / 좋아요(중복방지) 기능을 구현한 연습 프로젝트 입니다.

## 프로젝트 개요

- CommunityBoard는 회사 내부에서 사용할 수 있는
간단한 커뮤니티(게시판) 형태의 웹 애플리케이션입니다.

- 회원가입 / 로그인 (Auth) — 사용자는 이메일과 비밀번호로 계정을 생성하고 로그인할 수 있습니다.
- 게시글(Post) — 글 작성, 수정, 삭제, 조회 기능을 제공합니다.
- 댓글(Comment) — 게시글에 댓글을 작성하고 삭제할 수 있습니다.
- 좋아요(Like) — 댓글에 좋아요를 누를 수 있으며,
같은 사용자는 한 댓글에 한 번만 좋아요를 누를 수 있습니다 (중복방지 로직 포함).
- 관리자(Admin) — Role 기반 권한(Role: User / Admin) 구조로 관리 기능 확장 가능.

## 개발일지

1일차
프로젝트 생성 CommunityBoard
Miro 이용해 UML Diagram 완성
Figma로 기본 레이아웃 디자인 초안 완성
Entities 폴더 구성 완료 (User, Post, Comment, Like)
Models → Entities 폴더 구조 개편

느낀점: UML 다이어그램과 Figma 디자인을 통해 전체 흐름을 미리 구상해보니,
설계가 얼마나 중요한지 깨달았다
Models → Entities 폴더 구조 개편으로 코드의 역할이 명확해지고
프로젝트가 훨씬 깔끔해졌다

2일차 
DB 연결 및 마이그레이션
CommunityContext 생성 및 MSSQL LocalDB 연결 성공
SQL Server Express 연결 오류(Error 26) → LocalDB로 전환하여 해결
→ 로컬 개발 환경에서는 LocalDB,
→ 배포 시 AWS RDS 또는 Azure SQL로 교체 예정
Request/Response DTO ,Paging DTO정의
Swagger UI 등록 코드 추가

느낀점: DB, Swagger, DTO — 세 가지가 갖춰지니 비로소 프로젝트 뼈대가 완성된 느낌이었다.

3일차
Service 계층 구현
IPostService, ICommentService, ILikeService 인터페이스 및 구현 클래스 추가
CRUD 및 비즈니스 로직 (조회, 등록, 수정, 삭제) 작성
Repository 계층과 연결 및 의존성 주입 완료
공통 구조 개선 (Result & Paging)
Result<T> 구조를 개선하여 ApiError 포함 → API 응답 일관성 확보
PageQuery, PagedResult 리팩터링 → Skip / Take 기반 페이지네이션 구현
query.Page, query.Size를 이용한 효율적 데이터 페이징 처리
Validation & Filter 연동
FluentValidation 기반의 입력 검증 로직 유지
ValidationFilter를 통해 전역 검증 에러를 Result<ApiError> 형태로 통일

느낀점: 단순 CRUD가 아니라, 응답 포맷, 검증, 매핑, 구조적 일관성을 고려하니 코드가 훨씬 체계적으로 변했다.
Result<T>와 PagedResult를 직접 리팩터링하면서, 실무 서비스 로직의 확장성 개념을 조금 체감했다.

4일차
AccountController (MVC)
Program.cs에 AddAuthentication, AddAuthorization, UseAuthentication, UseAuthorization, IAuthService DI 등록
AuthService 회원가입 및 로그인 검증 로직 완성
LoginRequest / RegisterRequest DTO Validation 적용
AccountController 구현 완료  회원가입(Register),로그인(Login),로그아웃(Logout)

느낀점: 인증(Auth) 로직은 일반적인 CRUD와 달리 Repository 없이 Service만으로 처리하는 구조가 실무 표준임을 이해했다.
