using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Extensions;
using Mango.Services.EmailAPI.Messaging;
using Mango.Services.EmailAPI.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var optionBuilder=new DbContextOptionsBuilder<AppDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new EmailService(optionBuilder.Options));
//IEmailService
//IMapper mapper=MappingConfig.RegisterMaps().CreateMapper();
//builder.Services.AddSingleton(mapper);
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// Add services to the container.
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSwaggerGen(option =>
//{
//    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme,
//        securityScheme: new OpenApiSecurityScheme()
//    {
//        Name = "Authorization",
//        Description = "Enter the Bearer Authrization string as following: `Bearer Generated-JWT Token`",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    option.AddSecurityRequirement(new OpenApiSecurityRequirement()
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference=new OpenApiReference
//                {
//                    Type=ReferenceType.SecurityScheme,
//                    Id=JwtBearerDefaults.AuthenticationScheme
//                }
//            },new string[]{ }
//        }
//    });
//});


//builder.AddAppAuthentication();

//builder.Services.AddAuthorization();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigration();
app.UseAzureServiceBusConsumer();
app.Run();


void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (db.Database.GetPendingMigrations().Count() > 0)
        {
            db.Database.Migrate();
        }
    }
}
