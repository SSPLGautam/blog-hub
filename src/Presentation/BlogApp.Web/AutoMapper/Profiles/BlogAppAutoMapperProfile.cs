using AutoMapper;
using BlogApp.Shared.Dto;
using BlogApp.ViewModels;
using BlogApp.Web.ViewModel;

public class BlogAppAutoMapperProfile : Profile
{
    public BlogAppAutoMapperProfile()
    {
       
        CreateMap<PostResponceDto, PostViewModel>()
            .ForMember(dest => dest.Authore, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.FeatureImage, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore());

        CreateMap<PostDetailDto, PostViewModel>()
            .ForMember(dest => dest.Authore, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.FeatureImage, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes));
            

        CreateMap<LikeResponseDto, LikeViewModel>();

        
        CreateMap<CommentResponseDto, CommentViewModel>();

 
        CreateMap<CategoryDto, CategoryViewModel>();

     
        CreateMap<PostViewModel, PostCreatedDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Authore));

        CreateMap<CommentViewModel, CommentCreateDto>();
    }
}    
 