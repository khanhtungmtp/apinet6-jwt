
using apinet6._Repositories.Interfaces;
using apinet6._Repositories.Repository;
using apinet6.Models;
using Microsoft.EntityFrameworkCore;

namespace apinet6.Configurations
{
    public static class DatabaseConfig
    {
        public static void AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            // var area = configuration.GetSection("Appsetting:Area").Value;
            services.AddDbContext<Learn_DBContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            var _jwtSetting = configuration.GetSection("JWTSetting");
            services.Configure<JWTSetting>(_jwtSetting);
            // generate fresh token
            var _context = services.BuildServiceProvider().GetService<Learn_DBContext>();
            services.AddSingleton<IRefreshTokenGeneratorRepository>(provider => new RefreshTokenGeneratorRepository(_context));
        }
    }
}