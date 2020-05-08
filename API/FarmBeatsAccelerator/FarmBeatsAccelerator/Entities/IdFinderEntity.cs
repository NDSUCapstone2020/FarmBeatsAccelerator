using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmBeatsAccelerator.Entities
{
    public class IdFinderEntity : TableEntity
    {
        public int SkuId { get; set; }

    }
}
