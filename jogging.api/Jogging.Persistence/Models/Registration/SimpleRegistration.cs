using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jogging.Persistence.Models.Registration
{
    public partial class SimpleRegistration
    {
        public int Id { get; set; }

        public short? RunNumber { get; set; }

        public string? RunTime { get; set; }

        public int? CompetitionPerCategoryId { get; set; }

        public bool Paid { get; set; }

        public int? PersonId { get; set; }

        public int? CompetitionId { get; set; }

    }
}
