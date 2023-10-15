using EmployeeManagementAPI.Data.Context;
using EmployeeManagementAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using EmployeeManagementAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<EmployeeContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnectionString")); // Never put connection string here. like . Always store your sensitive data or connection data inside your appsettings
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); //// Getting assemblies from Automapper. Assemblies is a list of namespaces
builder.Services.AddTransient<IMailService, MailService>(); // Configuring DI in Program.cs....MailController, Line 13. When someone ask for Interface 'IMailService' give him the instance of class 'MailService' becasue all the logic is there. 
//builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.AddControllers()
            .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);                        // This line configures the JSON serialization options for the controllers. Here, you are setting the PropertyNamingPolicy of the JsonSerializerOptions to null.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)                                          // Token Validation; pass inside (Which Authenticatio Scheme we using). This specifies the authentication scheme to be used, which in this case is JWT Bearer authentication.
    .AddJwtBearer(option =>                                                                                   // we use the '=>' expression to assign values to the properties inside class 'JwtBearerOptions' using instance 'options'. If we never use '=>' we need to assign the properties like "'options.validateIssuer', 'option.validateAudience'"
    {
        option.TokenValidationParameters = new TokenValidationParameters                                            // creating an instance of the class
        {
            ValidateIssuerSigningKey = false,  // true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:SecretKey").Value)), // Pass this same key in startup.cs.services.AddAuthentication & appsettings.json                         
            ValidateIssuer = false,           // false: issuer can change in the future
            ValidateAudience = false         // false: no audience (Angular, react, vue.js or javascript)
        }; // this configuration sets up JWT Bearer authentication without performing any signature, issuer, or audience validation
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyPolicy",
        builder =>
        {
            builder.AllowAnyOrigin();           // Allow any port address from angular
            builder.AllowAnyHeader();           // Allow any port address from Backend Api
            builder.AllowAnyMethod();           //  Allows requests to use any HTTP method (GET, POST, PUT, DELETE)
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();        // make static files available
app.UseHttpsRedirection();
app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();                                                                                                                             

app.MapControllers();

app.Run();
