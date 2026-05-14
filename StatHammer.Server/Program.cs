using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Data;
using StatHammer.Server.Simulation.Services;
using StatHammer.Server.Simulation.Dice.Services;
using StatHammer.Server.Simulation.Combat.Services;
using StatHammer.Server.Simulation.Battle.Services;



namespace StatHammer.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddRazorPages();

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

            


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();
            app.MapRazorPages();

            app.Run();
        }
    }
}
