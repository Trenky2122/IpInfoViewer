using IpInfoViewer.Libs.Abstractions;
using IpInfoViewer.Libs.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IIpInfoViewerDbRepository>(
    new IpInfoViewerDbRepository("Server=127.0.0.1;Port=5432;Database=ipinfoviewerprocesseddb;User Id=postgres;Password=0000;Include Error Detail=true"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});
app.UseAuthorization();
app.MapControllers();

app.Run();
