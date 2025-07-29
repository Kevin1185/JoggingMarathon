using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jogging.Persistence.Models.Address
{
    public class SimpleAddress
    {       
        public int AddressId { get; set; }

        public string? Street { get; set; }

        public string City { get; set; } = null!;

        public string? HouseNumber { get; set; }

        public string? ZipCode { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Street ?? "").GetHashCode();
                hash = hash * 23 + (HouseNumber ?? "").GetHashCode();
                hash = hash * 23 + (ZipCode ?? "").GetHashCode();
                hash = hash * 23 + (City ?? "").GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is SimpleAddress other))
                return false;

            return Street == other.Street && HouseNumber == other.HouseNumber && ZipCode == other.ZipCode && City == other.City;
        }
    }
}
