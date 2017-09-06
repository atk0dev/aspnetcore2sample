using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> Categories { get; }
    }
}
