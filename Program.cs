using AucWebAPI.Data;
using AucWebAPI.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AucWebAPI.Services.Implementations;
using AucWebAPI.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBidsService, BidsService>();
builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Or your Redis connection string
    options.InstanceName = "AuctionApp_"; // Prefix for keys
});


var jwtKey = "991e28211e65dfc8568534ef2f3fd73505d83922a0dc578da24a1edf1a6afb71ea5af75bda675040934fa5f983b0e3d2b8e0eb7ea83c56e14057b0a284e9b10ece47ed19d74158ebde55bff2f0a19ac29f48eb5404c112f437a62e2320c583c83d7ff0901a6bf4aed6a0aa28196dcb4240e25f2ec5f46e5bde0bb53fb02f065f5bd8750820a6f1545d29bff8a2eff51c4c632cc1524276de98f002039288b2da75b808e46f0df49f300cd559eea4896b3b1facd6272e8a34e70bf4621044a79c3096ff002a3b843f9e112a995216bb97e147cc1ccefe659764f56100548290450d57b0783fc7583a3f18e7a90910715bffc5aeec0e5947a9017714f45e39515787faf99472470097b0589bb9a3723aa0b03d8186c7a39cdbf6a4a89ff125dca0ed65b9dfbc885918eeb1ddf1aa3892d96c2faf2e5623b4ef5d7bbf047ca9ac5a8d75de83711fbb87af57f68ee7079755f24a65de5ba97b494e4df5fc5306ec6d4012fec87ff355a593e882e6e03a1038f3422a9aaa8fda283632d1403d9c88718f9e8106113c4aaebb00ab4fc1415ae02a0000b3c402321dc72bb255e7c37eee6f9e8a0ebd0d808499d41d9ee7fdf725d1842af4ef5ec5effefd86bbf3c4553de86149fec5c121aeb474653ef7258db27f26dfe90ad3a09238a6a887fe07bbbf080461d40e86683f5601fee26f73353272f9b0f4ea5d52a52547c90a0242c504";
var key = Encoding.UTF8.GetBytes(jwtKey);

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
        ValidIssuer = "Auc WebAPI",
        ValidAudience = "application",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddCors(builder =>
{
    builder.AddPolicy("AllowAllOrigins",
        options => options.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();