
using apinet6.Models;
using Microsoft.EntityFrameworkCore;

namespace apinet6.Configurations
{
    public static class DatabaseConfig
    {
        public static void AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration){
            if(services == null) throw new ArgumentNullException (nameof(services));
            // var area = configuration.GetSection("Appsetting:Area").Value;
            services.AddDbContext<Learn_DBContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}