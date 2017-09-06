using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1
{
    public static class ProgramExt
    {
        public static IWebHost ConfigureMigration(this IWebHost webhost)
        {
            using (var scope = webhost.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                using(var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    dbContext.Database.Migrate();
                }
            }

            return webhost;
        }
    }
}
