using Microsoft.EntityFrameworkCore;
using OpenEduSyncShare.Api.ExceptionHandling;
using OpenEduSyncShare.Application.Interfaces;
using OpenEduSyncShare.Application.Services;
using OpenEduSyncShare.Infrastructure.Persistence;
using OpenEduSyncShare.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IAnswersService, AnswersService>();
builder.Services.AddScoped<IAnswersRepository, AnswersRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();