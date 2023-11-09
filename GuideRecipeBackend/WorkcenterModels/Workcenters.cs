using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuideRecipeBackend.WorkcenterModels
{
    public interface Workcenters
    {
        string WorkcenterId { get; set; }
        string Created {  get; set; }
        string Descritption { get; set; }
        bool Enabled { get; set; }
        string Modified { get; set; }
        string ParentWorkcenterId { get; set; }
        bool Release {  get; set; }
        bool Schedule {  get; set; }
        string Workcenter {  get; set; }
        string WorkcenterControllerType { get; set; }
        List<Workcenters> Children { get; set; }
    }
}
