using Microsoft.EntityFrameworkCore;
using DepartamentosAPI.Models.Entities;
using DepartamentosAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DepartamentosAPI.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<ActividadRepository>();
builder.Services.AddTransient<DepartamentoRepository>();
builder.Services.AddSingleton<JWTHelper>();
builder.Services.AddDbContext<ItesrcneActividadesContext>(x =>
x.UseMySql("server=204.93.216.11;database=itesrcne_actividades;user=itesrcne_deptos;password=sistemaregistrotec24",
Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.3.29-mariadb")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer
    (
        x=>
        {
            var issuer = builder.Configuration.GetSection("Jwt").GetValue<string>("Iusser");
            var audience = builder.Configuration.GetSection("Jwt").GetValue<string>("Audience");
            var secret = builder.Configuration.GetSection("Jwt").GetValue<string>("Secret");
            x.TokenValidationParameters = new()
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? "")),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true
            };
        }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
