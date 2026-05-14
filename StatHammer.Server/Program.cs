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
            builder.Services.AddSwaggerGen();

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
