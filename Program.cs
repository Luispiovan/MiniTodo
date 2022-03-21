using MiniTodo.data;
using MiniTodo.ViewModels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("v1/todos", (AppDbContext context) => {
    var todos = context.Todos?.ToList();
    
    return Results.Ok(todos);
}).Produces<Todo>();

app.MapPost("v1/todos", (AppDbContext context, CreateTodoViewModels request) => {
    var title = request.Title;
    
    var todo = request.MapTo();
    if(!request.IsValid){
        return Results.BadRequest(request.Notifications);
    }

    context.Todos?.Add(todo);
    context.SaveChanges();
    
    return Results.Created($"v1/todos/{todo.Id}", todo);
});

app.Run();
