using Mango.Services.EmailAPI.Models.Dto;

namespace Mango.Services.EmailAPI.Service.IService
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
    }
}
