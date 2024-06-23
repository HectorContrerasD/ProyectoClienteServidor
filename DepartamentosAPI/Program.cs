using Microsoft.EntityFrameworkCore;
using DepartamentosAPI.Models.Entities;
using DepartamentosAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DepartamentosAPI.Helpers;
using DepartamentosAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ItesrcneActividadesContext>(x =>
x.UseMySql("server=labsystec.net;database=labsyste_doubled;user=labsyste_doubled;password=xs~o714N5",
Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.3.29-mariadb")));
builder.Services.AddTransient<ActividadRepository>();
builder.Services.AddTransient<DepartamentoRepository>();
builder.Services.AddSingleton<JWTHelper>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer
    (
        x=>
        {
            var issuer = builder.Configuration.GetSection("JWT").GetValue<string>("Issuer");
            var audience = builder.Configuration.GetSection("JWT").GetValue<string>("Audience");
            var secret = builder.Configuration.GetSection("JWT").GetValue<string>("Secret");
            x.TokenValidationParameters = new()
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? "")),
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
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificacionesHub>("/notificacionesHub");
app.MapControllers();

app.Run();
