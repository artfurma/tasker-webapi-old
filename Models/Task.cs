using System.Collections.Generic;

namespace TaskerWebAPI.Models
{
	public class Task
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public IList<Task> Children { get; set; }
	}
}