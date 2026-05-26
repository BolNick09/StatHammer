using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Simulation.Services;
using StatHammer.Server.Simulation.Dice.Services;
using StatHammer.Server.Simulation.Combat.Services;
using StatHammer.Server.Simulation.Battle.Services;
using StatHammer.Server.PageServices.Simulations;
using Microsoft.AspNetCore.Identity;
using StatHammer.Server.Models.Entities;
using StatHammer.Server.Data.Seed;
using StatHammer.Server.PageServices.Admin.Weapons;
using StatHammer.Server.PageServices.Admin.Abilities;
using StatHammer.Server.PageServices.Admin.Models;
using StatHammer.Server.PageServices.Admin.Wargears;
using StatHammer.Server.PageServices.Admin.Keywords;
using StatHammer.Server.PageServices.Admin.Units;
using StatHammer.Server.PageServices.Admin.SimulationResults;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;



namespace StatHammer.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Simulations");
                options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Введите JWT токен. Пример: Bearer eyJhbGciOi..."
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<StatHammerDbContext>()
            .AddDefaultTokenProviders();

            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];
            var jwtKey = builder.Configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtIssuer) ||
                string.IsNullOrWhiteSpace(jwtAudience) ||
                string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("JWT settings are not configured.");
            }

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme;
                    options.DefaultChallengeScheme = Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,

                        ValidateAudience = true,
                        ValidAudience = jwtAudience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(2)
                    };
                });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StatHammerDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IUnitRuntimeBuilder, UnitRuntimeBuilder>();

            builder.Services.AddScoped<IDiceExpressionParser, DiceExpressionParser>();
            builder.Services.AddScoped<IDiceRoller, DiceRoller>();
            builder.Services.AddSingleton<IRandomProvider, ThreadSafeRandomProvider>();

            builder.Services.AddScoped<ICombatDiceService, CombatDiceService>();

            builder.Services.AddScoped<IAttackResolver, AttackResolver>();
            builder.Services.AddScoped<IUnitAttackResolver, UnitAttackResolver>();

            builder.Services.AddScoped<IDamageAllocator, DamageAllocator>();
            builder.Services.AddScoped<IUnitCombatResolver, UnitCombatResolver>();

            builder.Services.AddScoped<IBattleSimulationService, BattleSimulationService>();

            builder.Services.AddScoped<IBattleBatchSimulationService, BattleBatchSimulationService>();
            builder.Services.AddScoped<IBattleBatchSimulationParallelService, BattleBatchSimulationParallelService>();
            builder.Services.AddScoped<IBattleResultPersistenceService, BattleResultPersistenceService>();

            builder.Services.AddScoped<ISimulationPageService, SimulationPageService>();
            builder.Services.AddScoped<IWeaponAdminPageService, WeaponAdminPageService>();
            builder.Services.AddScoped<IAbilityAdminPageService, AbilityAdminPageService>();

            builder.Services.AddScoped<IModelAdminPageService, ModelAdminPageService>();
            builder.Services.AddScoped<IWargearAdminPageService, WargearAdminPageService>();

            builder.Services.AddScoped<IKeywordAdminPageService, KeywordAdminPageService>();
            builder.Services.AddScoped<IUnitAdminPageService, UnitAdminPageService>();

            builder.Services.AddScoped<ISimulationResultAdminPageService, SimulationResultAdminPageService>();






            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapRazorPages();


            using (var scope = app.Services.CreateScope())
            {
                IdentitySeeder.SeedAsync(scope.ServiceProvider)
                    .GetAwaiter()
                    .GetResult();
            }

            app.Run();
        }
    }
}
