using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SE172266.ProductManagement.API.Extentions;
using SE172266.ProductManagement.API.Extentions.Auth;
using SE172266.ProductManagement.API.Extentions.kebab_case;
using SE172266.ProductManagement.API.Middleware;
using SE172266.ProductManagement.Repo;
using SE172266.ProductManagement.Repo.Entities;
using SE172266.ProductManagement.Repo.Repository;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new KebabCaseNamingPolicy();
        options.JsonSerializerOptions.DictionaryKeyPolicy = new KebabCaseNamingPolicy();
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<MyStoreDBContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("MyStoreDB");
    options.UseSqlServer(connectionString).AddInterceptors(new SoftDeleteInterceptor());
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<MyStoreDBContext>()
.AddDefaultTokenProviders();

builder.Services.AddTransient<UnitOfWork>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lab3", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.SchemaFilter<KebabCaseSchemaFilter>();
    c.ParameterFilter<KebabCaseParameterFilter>();
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleInitializer.InitializeRoles(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ApiResponseMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
