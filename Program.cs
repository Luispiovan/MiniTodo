using System.Text;
using MiniTodo;
using MiniTodo.data;
using MiniTodo.Models;
using MiniTodo.Models.Repositories;
using MiniTodo.Models.Services;
using MiniTodo.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(Settings.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization(options =>
{
   options.AddPolicy("Admin", policy => policy.RequireRole("manager"));
   options.AddPolicy("Employee", policy => policy.RequireRole("employee"));
});
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.UseEndpoints( endpoints =>
{
    endpoints.MapControllers();
});

app.MapPost("/v1/login", (User model) =>
{
    var user = UserRepository.Get(model.Username, model.Password);
    if (user == null)
    {
        return Results.NotFound(new 
        {
            message = "Invalid username or password"
        });
    }

    var token = TokenService.GenerateToken(user);
    user.Password = "";
    return Results.Ok(new
    {
        user = user,
        token = token
    });
});

app.MapGet("v1/todos", (ClaimsPrincipal user, AppDbContext context) =>
{
    var todos = context.Todos?.ToList();

    return Results.Ok(todos);
}).Produces<Todo>().RequireAuthorization();

app.MapPost("v1/todos", (ClaimsPrincipal user, AppDbContext context, CreateTodoViewModels request) =>
{
    var title = request.Title;

    var todo = request.MapTo();
    if (!request.IsValid)
    {
        return Results.BadRequest(request.Notifications);
    }

    context.Todos?.Add(todo);
    context.SaveChanges();

    return Results.Created($"v1/todos/{todo.Id}", todo);
}).RequireAuthorization();

app.Run();