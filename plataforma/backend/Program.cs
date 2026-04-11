using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Banco de dados ────────────────────────────────────────────────────────────
var connStr = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not found.");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

// ── CORS (permite o nginx frontal) ────────────────────────────────────────────
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Garante que a tabela existe na inicialização
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

// ── Endpoints ─────────────────────────────────────────────────────────────────

// POST /entrada  { "mensagem": "..." }
app.MapPost("/entrada", async (EntradaRequest req, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.Mensagem))
        return Results.BadRequest("O campo 'mensagem' é obrigatório.");

    var entrada = new Entrada { Mensagem = req.Mensagem, CriadoEm = DateTime.UtcNow };
    db.Entradas.Add(entrada);
    await db.SaveChangesAsync();

    return Results.Created($"/entrada/{entrada.Id}", entrada);
});

// GET /entrada  — lista todos os registros (útil para validação)
app.MapGet("/entrada", async (AppDbContext db) =>
    Results.Ok(await db.Entradas.OrderByDescending(e => e.CriadoEm).ToListAsync()));

// Health-check
app.MapGet("/health", () => Results.Ok("ok"));

app.Run();

// ── Modelos ───────────────────────────────────────────────────────────────────
record EntradaRequest(string Mensagem);

class Entrada
{
    public int      Id        { get; set; }
    public string   Mensagem  { get; set; } = string.Empty;
    public DateTime CriadoEm  { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Entrada> Entradas => Set<Entrada>();
}
