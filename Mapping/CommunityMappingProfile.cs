using AutoMapper;
using CommunityBoard.Contracts.Response;
using CommunityBoard.Entities;

namespace CommunityBoard.Mapping
{
    public class CommunityMappingProfile : Profile
    {
        public CommunityMappingProfile()
        {
            // Post → 목록용
            CreateMap<Post, PostListItemDto>()
                .ForMember(d => d.AuthorName, m => m.MapFrom(s => s.Author.Name))
                .ForMember(d => d.CommentCount, m => m.Ignore()); // 서비스에서 집계


            // Post → 상세용
            CreateMap<Post, PostDetailDto>()
                .ForMember(d => d.AuthorName, m => m.MapFrom(s => s.Author.Name))
                .ForMember(d => d.Comments, m => m.Ignore()); // 서비스에서 CommentDto로 변환하여 채움

            // Comment → DTO
            CreateMap<Comment, CommentDto>()
                .ForMember(d => d.AuthorName, m => m.MapFrom(s => s.Author.Name))
                .ForMember(d => d.LikeCount, m => m.MapFrom(s => s.Likes.Count))
                .ForMember(d => d.LikedByCurrentUser, m => m.Ignore()); // JWT 붙인 뒤 계산

            // Like → 내가 좋아요한 댓글 목록용
            CreateMap<Like, LikeResponse>()
                .ForMember(d => d.CommentContent, m => m.MapFrom(s => s.Comment.Content))
                .ForMember(d => d.PostId, m => m.MapFrom(s => s.Comment.PostId))
                .ForMember(d => d.PostTitle, m => m.MapFrom(s => s.Comment.Post.Title))
                .ForMember(d => d.CreatedAt, m => m.Ignore()); // Like 엔티티에 CreatedAt 없으면 서비스에서 채움
        }
    }
}