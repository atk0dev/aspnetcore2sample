using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IPieRepository
    {
        IEnumerable<Pie> Pies { get; }

        IEnumerable<Pie> PiesOfTheWeek { get; }

        Pie GetPieById(int pieId);
    }
}
