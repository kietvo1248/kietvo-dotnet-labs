using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Warehouse.Application.Interface.IRepository;
using Warehouse.Application.Interface.IService;
using Warehouse.Application.Services;
using Warehouse.Infrastructure;
using Warehouse.Infrastructure.DBContext;
using Warehouse.Infrastructure.Repositories;
using Warehouse.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// 1. CẤU HÌNH CÁC DỊCH VỤ HỆ THỐNG (SERVICES CONTAINER)
// =========================================================================

// Đăng ký bộ điều hướng Controller-based API (đã tích hợp từ tùy chỉnh ban đầu)
builder.Services.AddControllers();

// Đăng ký bộ công cụ Swagger để sinh giao diện UI kiểm thử API trực quan
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Simple Warehouse API",
        Version = "v1",
        Description = "Hệ thống RESTful API quản lý kho hàng - Clean Architecture"
    });

    // 1. Định nghĩa chuẩn bảo mật JWT Bearer cho Swagger biết
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập Token của bạn theo đúng cấu trúc: Bearer {chuoi_token_cua_ban}"
    });

    // 2. Ép tất cả các Endpoint trên Swagger phải áp dụng cấu hình Token này khi chạy test
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// Lấy chuỗi cấu hình Connection String từ file appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký bối cảnh kết nối cơ sở dữ liệu (DbContext) sử dụng Microsoft SQL Server
builder.Services.AddDbContext<WarehouseDBContext>(options =>
    options.UseSqlServer(connectionString));

// ĐĂNG KÝ UNIT OF WORK: Trái tim điều phối dữ liệu của hệ thống
// Sử dụng cơ chế AddScoped: Mỗi một Request gửi lên sẽ tạo ra một phiên làm việc độc lập
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductService, ProductService>();

//builder.WebHost.UseUrls("http://localhost:6969");

// CẤU HÌNH HỆ THỐNG XÁC THỰC TOKEN 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

var app = builder.Build();

// =========================================================================
// 2. CẤU HÌNH ĐƯỜNG ỐNG XỬ LÝ REQUEST (HTTP REQUEST PIPELINE / MIDDLEWARE)
// =========================================================================

// Kích hoạt Swagger UI khi dự án đang chạy trong môi trường phát triển (Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple Warehouse API v1");
        //c.RoutePrefix = string.Empty;
    });
}

// Bắt buộc chuyển hướng các yêu cầu HTTP thông thường sang giao thức HTTPS bảo mật
app.UseHttpsRedirection();

// Các chốt chặn trung gian phục vụ cho cơ chế bảo mật danh tính & phân quyền về sau (JWT)
app.UseAuthentication();
app.UseMiddleware<RoleAuthorizationMiddleware>();
app.UseAuthorization();

// Tự động map các tuyến đường (Routes) từ API Controllers vào hệ thống xử lý
app.MapControllers();

// Chạy
app.Run();