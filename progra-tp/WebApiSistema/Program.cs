using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaAcademico.Data;
using SistemaRepository.Data.Implementations;
using SistemaRepository.Data.Interfaces;
using SistemaRepository.Services.Implementations;
using SistemaRepository.Services.Interfaces;
using System.Text;
// Añade esto cerca de las líneas "builder.Services..."
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IAlumnoRepository, AlumnoRepository>();
builder.Services.AddScoped<IServiceAlumno, ServiceAlumno>();
builder.Services.AddScoped<IDashBoardEncargado, EncargadoRepository>();
builder.Services.AddScoped<IServiceEncargado, ServiceEncargado>();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "MiApi", // Debe coincidir con el issuer que pusiste en GenerateToken
            ValidAudience = "MIApi", // Debe coincidir con el audience que pusiste en GenerateToken
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EstaEsUnaClaveSuperSecretaYLargaDeMasDe32Caracteres123!")) // La misma clave secreta
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
       policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

app.UseCors("AllowAll");


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
