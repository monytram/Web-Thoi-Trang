using AutoMapper;
using WebThoiTrang.Areas.Admin.Models;
using WebThoiTrang.Models.Entities;

namespace WebThoiTrang.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Category Mappings
            CreateMap<Category, CategoryViewModel>()
                .ForMember(dest => dest.AvailableParentCategories, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<CategoryViewModel, Category>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Product Mappings
            CreateMap<Product, ProductViewModel>()
                .ForMember(dest => dest.AvailableCategories, opt => opt.Ignore());

            CreateMap<ProductViewModel, Product>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // ProductVariant Mappings
            CreateMap<ProductVariant, ProductVariantViewModel>();
            CreateMap<ProductVariantViewModel, ProductVariant>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // ProductImage Mappings
            CreateMap<ProductImage, ProductImageViewModel>();
            CreateMap<ProductImageViewModel, ProductImage>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Order Mappings
            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<OrderViewModel, Order>()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // OrderDetail Mappings
            CreateMap<OrderDetail, OrderDetailViewModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.Name))
                .ForMember(dest => dest.VariantInfo, opt => opt.MapFrom(src =>
                    $"{src.ProductVariant.Product.Name} - {src.ProductVariant.Size} - {src.ProductVariant.Color}"));

            CreateMap<OrderDetailViewModel, OrderDetail>();
        }
    }
}
