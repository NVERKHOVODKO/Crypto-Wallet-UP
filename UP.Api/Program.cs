var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please insert token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        //Type = SecuritySchemeType.ApiKey,
        //BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "UP.Api API"
    });
});

builder.Services.AddScoped<IDbRepository, DbRepository>();
builder.Services.AddScoped<IHashHelpers, HashHelpers>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITransactionsRepository, TransactionsRepository>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; });
SQLitePCL.Batteries_V2.Init();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=up.db"));

builder.Services.AddCors();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
    };
});

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapControllers();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
        options.RoutePrefix = "swagger";
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    DataInitializer.Initialize(context);
}

app.Run();