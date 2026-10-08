var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var tareas = new List<Tarea>
{
    new(1, "aprender C#", "Completar modulo de Minimal APIs", "pendiente", "alta" )
};

app.MapGet("/api/tareas", (string?estado) =>
{
    if (string.IsNullOrEmpty(estado))return Results.Ok(tareas);
    var tareasFiltradas = tareas.Where(t => t.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase));
    return Results.Ok(tareasFiltradas);                                                              
});
//GET por id: buscar una tarea por su id
app.MapGet("/api/tareas/{id}", (int id) =>
{
    var tarea = tareas.FirstOrDefault(t => t.Id == id);
    return tarea is not null ? Results.Ok(tarea) : Results.NotFound("tarea no encontrada");
});
//POST: crear una nueva tarea
app.MapPost("/api/tareas", (CrearTareaDto dto) =>
{
   var nuevaTarea = new Tarea(tareas.Count + 1, dto.Titulo, dto.Descripcion, dto.Estado, dto.Prioridad);
   tareas.Add(nuevaTarea);
   return Results.Created($"/api/tareas/{nuevaTarea.Id}", nuevaTarea); 
});
//PUT: actualizar una tarea existente
app.MapPut("/api/tareas/{id}", (int id, CrearTareaDto dto) =>
{
    var tareaExistente = tareas.FirstOrDefault(t => t.Id == id);
    if (tareaExistente is null) return Results.NotFound("tarea no encontrada");

    var tareaActualizada = new Tarea(id, dto.Titulo, dto.Descripcion, dto.Estado, dto.Prioridad);
    tareas.Remove(tareaExistente);
    tareas.Add(tareaActualizada);

    return Results.Ok(tareaActualizada);
});
app.Run();

public record Tarea(int Id, string Titulo, string Descripcion, string Estado, string Prioridad);
public record CrearTareaDto(string Titulo, string Descripcion, string Estado, string Prioridad);