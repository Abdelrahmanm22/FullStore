using AutoMapper;
using Store.Models;

namespace Store.MappingProfiles
{
    public class ProductProfile:Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductViewModel, Product>().ReverseMap();
        }
    }
}
