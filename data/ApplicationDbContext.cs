using Microsoft.EntityFrameworkCore;

namespace recipesAtYourFingertipsRev0.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}