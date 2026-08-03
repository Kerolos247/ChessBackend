using ChessBackend.Hubs;
using ChessBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. تحديد الـ URL والبورت (5000) ليتطابق تماماً مع الـ WPF Client
builder.WebHost.UseUrls("http://localhost:5000");

// 2. Add services to the container (Controllers & OpenAPI)
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 3. Add SignalR & RoomManager Services
builder.Services.AddSignalR();
builder.Services.AddSingleton<RoomManager>(); // للحفاظ على حالة الغرف في الـ Memory

// 4. Add CORS (عشان يسمح للـ WPF Client بالاتصال بدون أي Blocking)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ملاحظة: تم إيقاف app.UseHttpsRedirection() لأن الـ WPF Client يتصل عبر HTTP عادي (http://localhost:5000)
// app.UseHttpsRedirection();

// تفعيل السياسات الخاصة بـ CORS
app.UseCors();

app.UseAuthorization();

// 5. Map Endpoints (Controllers & SignalR Hub)
app.MapControllers();
app.MapHub<ChessHub>("/gameHub"); // الرابط اللي هيتصل بيه الـ WPF Client

app.Run();