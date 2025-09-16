using Microsoft.EntityFrameworkCore;
using RickAndMorty.WebApp;
using RickAndMortyApp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();  
builder.Services.AddRazorPages();   
builder.Services.AddMvc();


builder.Services.AddDbContext<RickAndMortyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();
builder.Services.AddRickAndMortyServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapRazorPages();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
