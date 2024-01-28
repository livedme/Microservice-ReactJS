using AutoMapper;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;

namespace Mango.Services.EmailAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig=new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, EmailLogger>();
                config.CreateMap<EmailLogger, ProductDto>();
            });

            return mappingConfig;
        }
    }
}
