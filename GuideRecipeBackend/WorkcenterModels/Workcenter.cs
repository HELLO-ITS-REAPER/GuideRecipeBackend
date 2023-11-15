using GO.Global.Workcenters;
using GO.Global.Workcenters.WorkcenterTypes;
using GO.Oms.Shared.Workcenter;
using GO.WcsManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuideRecipeBackend.WorkcenterModels
{
    public class Workcenter
    {
        public string WorkcenterId { get; set; }
        public string ParentWorkcenterId { get; set; }
        public string Created { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public string Modified { get; set; }
        public bool Release { get; set; }
        public bool Schedule { get; set; }
        public string WorkcenterName { get; set; }
        public string WorkcenterType { get; set; }
        public string WorkcenterControllerType { get; set; }
        public string LocationKey { get; set; }
        public string LocationType { get; set; }
        public List<Workcenter> Children { get; set; } = new List<Workcenter>();
    }

    public class Locations
    {
        public string LocationKey { get; set; }
        public string LocationType { get; set; }
    }
}
