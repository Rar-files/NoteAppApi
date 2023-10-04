using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add configuration to the container.
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>{
    config.SwaggerDoc("v1", new OpenApiInfo{ Title = "NoteAppAPI", Version = "v1", Description = "" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NoteAppAPIv1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
