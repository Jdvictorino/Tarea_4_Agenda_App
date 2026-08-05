using Tarea_4_Agenda_App.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddSingleton<IContactService, ContactService>();

var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Redirigir la raíz al login
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run("http://localhost:5000");

