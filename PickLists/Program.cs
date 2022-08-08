using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo {
         Title = "School API",
         Description = "School",
         Version = "v1" });
});


builder.Services.AddDbContextPool<SchoolContext>(opts => {
    opts.UseInMemoryDatabase("SchoolDB");
});

var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>();
optionsBuilder.UseInMemoryDatabase("SchoolDB");
using var db = new SchoolContext(optionsBuilder.Options);

db.Students.Add(new Student { FirstName = "Dillon", Grade = LetterGrade.None });
db.SaveChanges();
var app = builder.Build();

app.MapGet("/", async (SchoolContext db) => await db.Students.ToListAsync());

app.MapPost("/", async (SchoolContext db, Student student) => {
    await db.Students.AddAsync(student);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.UseSwagger();

app.Run();

public class SchoolContext : DbContext {
    public SchoolContext(DbContextOptions<SchoolContext> opts)
        :base(opts)  
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Student>()
            .Property(e => e.Grade)
            .HasConversion(
                c => c.ToString(),
                c => Enum.Parse<LetterGrade>(c)
            );
    }

    public DbSet<Student> Students { get; set; }
}
public class Student {
    public int StudentId { get; set; }

    public string FirstName { get; set; }

    public LetterGrade Grade { get; set; }
}

public enum LetterGrade
{
    None = 0,   
    Abe = 1,
    Bce = 2,
    Cde = 3,
    Dfds = 4,
    Fasdf = 5
}