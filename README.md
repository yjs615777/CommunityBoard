## CommunityBoard
사내 커뮤니티 게시판 프로젝트
ASP.NET Core MVC + Entity Framework Core 기반의 웹 게시판 애플리케이션
사용자 회원가입 / 로그인 / 게시글 / 댓글 / 좋아요(중복방지) 기능을 구현한 프로젝트 입니다.

## 프로젝트 개요

- CommunityBoard는 회사 내부에서 사용할 수 있는
간단한 커뮤니티(게시판) 형태의 웹 애플리케이션입니다.

- 회원가입 / 로그인 (Auth) — 사용자는 이메일과 비밀번호로 계정을 생성하고 로그인할 수 있습니다.
- 게시글(Post) — 글 작성, 수정, 삭제, 조회 기능을 제공합니다.
- 댓글(Comment) — 게시글에 댓글을 작성하고 삭제할 수 있습니다.
- 좋아요(Like) — 댓글에 좋아요를 누를 수 있으며,
같은 사용자는 한 댓글에 한 번만 좋아요를 누를 수 있습니다 (중복방지 로직 포함).
- 관리자(Admin) — Role 기반 권한(Role: User / Admin) 구조로 관리 기능 확장 가능.

개발일지

1일차
프로젝트 생성 CommunityBoard
Miro 이용해 UML Diagram 완성
Figma로 기본 레이아웃 디자인 초안 완성
Entities 폴더 구성 완료 (User, Post, Comment, Like)
Models → Entities 폴더 구조 개편
