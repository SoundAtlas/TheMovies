using System;

namespace TheMovies.Core.Models
{
	public class Cinema
	{
		public string Name { get; set; }

		public List<Screening> Screenings { get; set; } = new List<Screening>(); //Screenings? er det det rette engelske ord for visninger?
	}
}