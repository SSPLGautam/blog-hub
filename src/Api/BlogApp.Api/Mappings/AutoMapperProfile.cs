using AutoMapper;

using BlogApp.Models;
using BlogApp.Shared.Dto;

namespace BlogApp.Api.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<PostCreatedDto, Post>()
            .ForMember(dest => dest.Authore, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.FeatureImagePath, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

        CreateMap<Post, PostResponceDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Authore))
            .ForMember(dest => dest.FeatureImagePath, opt => opt.MapFrom(src => src.FeatureImagePath));


        CreateMap<Category, CategoryDto>();
        CreateMap<PostLike, LikeResponseDto>();
        CreateMap<Post, PostDetailDto>()
      .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Authore))
      .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
      .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes))
      .ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src => src.CreatedByUserId)) 
      .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => src.IsPublished));       
        CreateMap<Comment, CommentResponseDto>()
           .ForMember(dest => dest.UserName,
               opt => opt.MapFrom(src => src.User.UserName));
    }
}
