using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XtramileWeather.Domain.Entities
{    
    public class FavoriteCity
    {
        public int Id { get; set; }

        public int CityId { get; set; }

        public City City { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
