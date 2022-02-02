using GreetingService.Core;
using GreetingService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddXmlDataContractSerializerFormatters();
                //.AddJsonOptions(o =>
                //{
                //    o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                   
                //});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService, HardCodedUserService>();

builder.Services.AddScoped<IGreetingRepository, FileGreetingRepository>(c => {
    var config = c.GetService<IConfiguration>();
    return new FileGreetingRepository(config["FileRepositoryFilePath"]);
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();



