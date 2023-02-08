using IpInfoViewer.Libs.Implementation;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Implementation.Map;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IIpInfoViewerDbRepository>(
    new IpInfoViewerDbRepository(builder.Configuration["IpInfoViewerProcessedConnectionString"]));
builder.Services.AddSingleton<IMFileDbRepository, MFileDbRepository>();
builder.Services.AddSingleton<IMapFacade, MapFacade>();
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
