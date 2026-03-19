using AutoMapper;
using BlogApp.Models;
using BlogApp.ViewModels;
using BlogApp.Web.Helper;
using BlogApp.Web.ViewModel;

namespace BlogApp.Web.AutoMapper.Profiles;

public class BlogAppAutoMapperProfile : Profile
{
    public BlogAppAutoMapperProfile()
    {
        CreateMap<Post, PostViewModel>()
            .ForMember(dest => dest.FeatureImage, opt => opt.MapFrom(x => ImagesHelper.GetImage(x.FeatureImagePath)))
            .ForMember(dest => dest.Categories, opt => opt.Ignore());

        CreateMap<PostViewModel, Post>()
            .ForMember(dest => dest.FeatureImagePath, opt => opt.MapFrom(x => ImagesHelper.GetImagePath(x.FeatureImage)));

        CreateMap<Comment, CommentViewModel>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.CanDelete,
                    opt => opt.Ignore());

        CreateMap<CommentViewModel, Comment>();

        CreateMap<Category, CategoryViewModel>()
            .ReverseMap();


    }

}
