using Microsoft.EntityFrameworkCore;
using TaskerWebAPI.Models;

namespace TaskerWebAPI.Helpers
{
	public class TaskerContext : DbContext
	{
		public TaskerContext(DbContextOptions<TaskerContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
	}
}