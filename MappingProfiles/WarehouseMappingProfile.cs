using AutoMapper;
using warehouseapp.Data.Models;
using warehouseapp.Enums;
using warehouseapp.ViewModels;
using warehouseapp.ViewModels.Product;
using warehouseapp.ViewModels.Category;
using warehouseapp.ViewModels.Tag;
using warehouseapp.ViewModels.Partner;
using warehouseapp.ViewModels.Order;
using warehouseapp.ViewModels.Inventory;

namespace warehouseapp.Mappings
{
    public class WarehouseMappingProfile : Profile
    {
        public WarehouseMappingProfile()
        {

            //products

            CreateMap<ProductRequestViewModel, Product>()
                .ForMember(d => d.UnitOfMeasurement,
                    o => o.MapFrom(s =>
                        s.UnitOfMeasurement.HasValue
                            ? s.UnitOfMeasurement.Value
                            : UnitOfMeasurementEnum.Piece
                    ))

                .ForMember(d => d.QuantityInStock, o => o.MapFrom(_ => 0))
                .ForMember(d => d.ReorderLevel, o => o.MapFrom(_ => 0))
                .ForMember(d => d.ProductTags, o => o.Ignore())
                .ForMember(d => d.OrderItems, o => o.Ignore())
                .ForMember(d => d.InventoryTransactions, o => o.Ignore());

            CreateMap<Product, ProductResponseViewModel>()
                .ForMember(d => d.CategoryName,
                    o => o.MapFrom(s =>
                        s.Category != null ? s.Category.Name : string.Empty))
                .ForMember(d => d.Tags,
                    o => o.MapFrom(s => s.ProductTags));

            CreateMap<Product, ProductEditResponseViewModel>();

            // tags

            CreateMap<ProductTagRequestViewModel, ProductTag>();

            CreateMap<ProductTag, ProductTagDisplayViewModel>()
                .ForMember(d => d.TagKey,
                    o => o.MapFrom(s => s.Tag.Key))
                .ForMember(d => d.Value,
                    o => o.MapFrom(s => s.TagValue.Value))
                .ForMember(d => d.TagValueId,
                    o => o.MapFrom(s => s.TagValueId));

            CreateMap<Tag, TagResponseViewModel>()
                .ForMember(d => d.Values,
                    o => o.MapFrom(s =>
                        s.Values
                         .OrderBy(v => v.Value)
                         .Select(v => v.Value)));

            // categories

            CreateMap<CategoryRequestViewModel, Category>()
                .ForMember(d => d.SubCategories, o => o.Ignore())
                .ForMember(d => d.Products, o => o.Ignore());

            CreateMap<Category, CategoryResponseViewModel>();

            CreateMap<Category, SubCategoryViewModel>();
            CreateMap<SubCategoryViewModel, Category>()
                .ForMember(d => d.SubCategories, o => o.Ignore())
                .ForMember(d => d.Products, o => o.Ignore());

            //partners

            CreateMap<PartnerRequestViewModel, Partner>();

            CreateMap<Partner, PartnerResponseViewModel>();

            //orders

            CreateMap<OrderRequestViewModel, Order>()
                .ForMember(d => d.OrderItems,
                    o => o.MapFrom(s => s.Items))
                .ForMember(d => d.Partner, o => o.Ignore())
                .ForMember(d => d.OrderItems, o => o.Ignore());

            CreateMap<OrderItemRequestViewModel, OrderItem>()
                .ForMember(d => d.Product, o => o.Ignore());

            CreateMap<OrderItem, OrderItemResponseViewModel>()
                .ForMember(d => d.ProductName,
                    o => o.MapFrom(s => s.Product.Name));

            CreateMap<Order, OrderResponseViewModel>()
                .ForMember(d => d.PartnerName,
                    o => o.MapFrom(s => s.Partner.Name))
                .ForMember(d => d.Items,
                    o => o.MapFrom(s => s.OrderItems));

            // inventory
            CreateMap<InventoryTransactionRequestViewModel, InventoryTransaction>()
                .ForMember(d => d.Product, o => o.Ignore())
                .ForMember(d => d.Partner, o => o.Ignore());

            CreateMap<InventoryTransaction, InventoryTransactionResponseViewModel>()
                .ForMember(d => d.ProductName,
                    o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.PartnerName,
                    o => o.MapFrom(s => s.Partner.Name))
                .ForMember(d => d.ReturnReason,
                    o => o.MapFrom(s =>
                        string.IsNullOrWhiteSpace(s.ReturnReason)
                            ? (ReturnReasonEnum?)null
                            : Enum.Parse<ReturnReasonEnum>(s.ReturnReason)
                    ));
        }
    }
}
