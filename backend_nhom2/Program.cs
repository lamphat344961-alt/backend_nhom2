using backend_nhom2.Data;
using backend_nhom2.Domain;
using backend_nhom2.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*") // Cho phép mọi nguồn gốc, có thể thay đổi trong môi trường production
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// Cấu hình DbContext, thống nhất sử dụng AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình xác thực JWT (Authentication)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("Khóa JWT (Jwt:Key) chưa được cấu hình trong file appsettings.json");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Cấu hình phân quyền (Authorization)
builder.Services.AddAuthorization();

// Thêm Controllers và cấu hình xử lý tham chiếu vòng tròn (quan trọng!)
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();

// Lấy cấu hình Swagger hoàn chỉnh nhất, cho phép nhập JWT token
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Smart Inventory API", Version = "v1" });

    // Thêm định nghĩa bảo mật cho JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập JWT token theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Tự động áp dụng migration và seed dữ liệu khi ứng dụng khởi động
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate(); // Tự động áp dụng migration

        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { RoleName = "Owner" },
                new Role { RoleName = "Driver" }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("Đã seed dữ liệu mặc định cho bảng Roles.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi trong quá trình khởi tạo và seed database.");
    }
}

// Cấu hình middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();