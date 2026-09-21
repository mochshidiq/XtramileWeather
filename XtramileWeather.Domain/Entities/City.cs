using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtramileWeather.Domain.Entities
{
    public class City
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int CountryId { get; set; }

        public Country Country { get; set; } = null!;
    }
}
