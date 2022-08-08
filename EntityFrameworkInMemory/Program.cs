using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo {
         Title = "PizzaStore API",
         Description = "Making the Pizzas you love",
         Version = "v1" });
});

builder.Services.AddDbContext<TodosContext>(options =>
    options.UseInMemoryDatabase("todos"));


var app = builder.Build();

app.MapGet("/", async (TodosContext db) => await db.Todos.ToListAsync());
app.MapPost("/", async (TodosContext db, Todo todo) => 
{
    await db.Todos.AddAsync(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/{todo.TodoId}", todo);
});

app.MapDelete("/{id}", async (TodosContext db, int id) => 
{
    var todo = await db.Todos.FindAsync(id);
    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();



public class TodosContext : DbContext 
{
    public TodosContext() {}

    public TodosContext(DbContextOptions<TodosContext> options)
            : base(options)
    {

    }

    public DbSet<Todo> Todos { get; set; }

}

public class Todo {
    public int TodoId { get; set; }
    public string Title { get; set; }

}

public class Position {
    public int PositionId { get; set; }
    public string FirstName { get; set; }

    public ICollection<Scope> Scopes { get; set; }
    public SalaryGradeLevel Sgl { get; set; }
}

public class SalaryGradeLevel {
    public int SalaryGradeLevelId { get; set; }
    public string Level { get; set; }
}

public class Scope {
    public int ScopeId { get; set; }
    public string Description { get; set; }
    public ICollection<Position> Positions { get; set; }
}