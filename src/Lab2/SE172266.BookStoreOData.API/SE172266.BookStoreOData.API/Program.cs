using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using SE172266.BookStoreOData.API.Repository;
using SE172266.ProductManagement.Repo.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddDbContext<BookStoreODataDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreODataDB"));
});

builder.Services.AddControllers().AddOData(options => options.Select().Filter()
    .Count().OrderBy().Expand().SetMaxTop(100)
    .AddRouteComponents("odata", GetEdmModel()));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseODataBatching();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
    modelBuilder.EntitySet<Book>("Books");
    modelBuilder.EntitySet<Press>("Presses");
    return modelBuilder.GetEdmModel();
}
