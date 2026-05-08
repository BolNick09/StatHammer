using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Simulation.Services;
using StatHammer.Server.Simulation.Dice.Services;
using StatHammer.Server.Simulation.Combat.Services;



namespace StatHammer.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StatHammerDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IUnitRuntimeBuilder, UnitRuntimeBuilder>();

            builder.Services.AddScoped<IDiceExpressionParser, DiceExpressionParser>();
            builder.Services.AddScoped<IDiceRoller, DiceRoller>();

            builder.Services.AddScoped<ICombatDiceService, CombatDiceService>();
            builder.Services.AddScoped<IAttackResolver, AttackResolver>();
            builder.Services.AddScoped<IUnitAttackResolver, UnitAttackResolver>();
            builder.Services.AddScoped<IDamageAllocator, DamageAllocator>();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
