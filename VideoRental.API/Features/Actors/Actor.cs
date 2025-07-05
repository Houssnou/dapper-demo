using System.Collections.Generic;

namespace VideoRental.API.Entities
{
	public class Actor
	{
		public int ActorId { get; set; }
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
	}
}
