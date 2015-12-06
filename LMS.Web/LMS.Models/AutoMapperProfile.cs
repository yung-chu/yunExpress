using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LMS.Data.Entity;

namespace LMS.Models
{
    public class AutoMapperProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModel"; }
        }

        protected override void Configure()
        {
            CreateMap<FeeTypeModel, FeeType>();
            CreateMap<FeeType, FeeTypeModel>();
            CreateMap<Country, CountryModel>();
            //CreateMap<ProductCategory, ProductCategoryModel>()
            //    .ForMember(p => p._parentId, dto => dto.MapFrom(p => p.ParentID))
            //    .ForMember(p => p.state, dto => dto.Ignore())
            //    .ForMember(p => p.IsLast, dto => dto.Ignore());
            //CreateMap<ProductCategoryModel, ProductCategory>()
            //    .ForMember(ent => ent.Products, dto => dto.Ignore());
            //CreateMap<Product, ProductModel>()
            //    .ForMember(p => p.ProductCategoryExt, dto => dto.Ignore());
            //CreateMap<ProductSKU, ProductSKUModel>();
            //CreateMap<ProductsLanguage, ProductsLanguageModel>();
            //CreateMap<ProductImage, ProductImageModel>();
            //CreateMap<ProductExt, ProductExtModel>();
            //CreateMap<SKUAttribute, SKUAttributeModel>();
            //CreateMap<ProductModel, Product>();
            //CreateMap<ProductPrice, ProductPriceModel>();
            //CreateMap<ProductCategoryMapping, ProductCategoryMappingModel>();
            //CreateMap<ProductCategoryMappingModel, ProductCategoryMapping>();

            CreateMap<Category, CategoryModel>();

            CreateMap<DictionaryTypeModel, DictionaryType>();
            CreateMap<DictionaryType, DictionaryTypeModel>();
        }
    }
}
