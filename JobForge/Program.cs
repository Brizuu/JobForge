    using System.Security.Claims;
    using System.Text;
    using JobForge.Data;
    using JobForge.Models;
    using JobForge.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.Annotations;

    var builder = WebApplication.CreateBuilder(args);

    // builder.Services.AddOpenApi();
    builder.Services.AddSwaggerGen();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        });

    builder.Services.AddControllersWithViews(); // jeśli potrzebne dla Razor/Views
    // builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

    builder.Services.AddScoped<ICvService, CvService>();
    builder.Services.AddScoped<IJobOfferService, JobOfferService>();
    builder.Services.AddScoped<IContractService, ContractService>();
    builder.Services.AddScoped<ICourseService, CourseService>();
    builder.Services.AddScoped<IPublicFacility, PublicFacility>();
    builder.Services.AddScoped<IInternshipService, InternshipService>();
    builder.Services.AddScoped<IGrantService, GrantService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IChatService, ChatService>();



    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddSignalR();

    // builder.Services.AddScoped<CvService>();

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
                RoleClaimType = ClaimTypes.Role,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "JobForge",
            Version = "v1",
            Description = "Dokumentacja API z użyciem Swaggera"
        });
        c.EnableAnnotations();
        
        var jwtSecurityScheme = new OpenApiSecurityScheme
        {
            Scheme = "bearer",
            BearerFormat = "JWT",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Description = "Wpisz {token}",

            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };

        c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { jwtSecurityScheme, Array.Empty<string>() }
        });
    });

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        // Odpalenie migracji (synchronizowane)
        context.Database.Migrate();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        // Czekamy aż role się zseedują
        await RoleSeeder.SeedRolesAsync(roleManager);
    }


    app.UseAuthentication();
    app.UseAuthorization();

    
    builder.Services.AddHttpContextAccessor();
    app.MapHub<ChatHub>("/chathub");


    // app.MapOpenApi();
    app.UseHttpsRedirection();
    app.MapControllers();


    app.UseSwagger();  

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "JobForge API V1");
        options.RoutePrefix = "swagger"; 
        options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
    });

    app.UseStaticFiles();


    app.Run();

