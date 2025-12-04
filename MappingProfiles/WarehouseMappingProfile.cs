using AutoMapper;
using warehouseapp.Data.Models;
using warehouseapp.ViewModels;

namespace warehouseapp.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductRequestViewModel, Product>();

            CreateMap<Product, ProductResponseViewModel>()
                .ForMember(dest => dest.CategoryName, 
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""));

             CreateMap<SubCategoryViewModel, Category>();

            CreateMap<Category, CategoryResponseViewModel>()
                .ForMember(dest => dest.SubCategories,
                    opt => opt.MapFrom(src => src.SubCategories));

            CreateMap<Category, SubCategoryViewModel>();
        
        }
    }
}
