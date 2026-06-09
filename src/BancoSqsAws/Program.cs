using Amazon.SQS;
using BancoSqsAws.Configuration;
using BancoSqsAws.Data;
using BancoSqsAws.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ─── Configuração ───────────────────────────────────────────────────────
builder.Services.Configure<SqsSettings>(
    builder.Configuration.GetSection(SqsSettings.SectionName));

// ─── AWS ────────────────────────────────────────────────────────────────
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSQS>();

// ─── Banco de Dados ─────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── Serviços ───────────────────────────────────────────────────────────
builder.Services.AddScoped<ISqsService, SqsService>();

// ─── API ────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BancoSqsAws API", Version = "v1" });
});

var app = builder.Build();

// ─── Migrations automáticas ─────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ─── Pipeline HTTP ──────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.RoutePrefix = string.Empty);
}

app.MapControllers();
app.Run();
