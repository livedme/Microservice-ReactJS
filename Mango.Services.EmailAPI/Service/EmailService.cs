
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Service.IService;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Service
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> dbOptions;

        public EmailService(DbContextOptions<AppDbContext> _dbOptions)
        {
            this.dbOptions = _dbOptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested");
            message.AppendLine("<br/>Total " + cartDto.CartHeader.CartTotal);
            message.AppendLine("<br/>");
            message.AppendLine("<ul>");

            foreach (var cart in cartDto.CartDetails)
            {
                message.AppendLine("<li>");
                message.AppendLine(cart.Product.Name + " x " + cart.Count);
                message.AppendLine("</li>");
            }
            message.AppendLine("</ul>");

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }

        public async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLogger = new EmailLogger()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };

                await using var dbConext = new AppDbContext(dbOptions);
                await dbConext.EmailLoggers.AddAsync(emailLogger);
                await dbConext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}

