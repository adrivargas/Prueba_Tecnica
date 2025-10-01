using Microsoft.EntityFrameworkCore;
using ProductService.Api.Data;
using ProductService.Api.Extensions;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));


builder.Services.AddControllers();


var cn = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddDbContext<AppDbContext>(opt =>
opt.UseSqlServer(cn));


builder.Services.AddRepositories();


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseCors();
app.MapControllers();


// Crear DB si no existe (solo dev)
using (var scope = app.Services.CreateScope())
{
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.EnsureCreated();
}


app.Run();