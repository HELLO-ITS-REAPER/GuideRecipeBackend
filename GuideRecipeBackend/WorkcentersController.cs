using GO;
using GO.Guide;
using GO.Guide.Actions;
using GO.Guide.Actions.Serialization;
using GO.Guide.DataLayer;
using GO.Owin;
using GO.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using GuideRecipeBackend.Models;
using GO.Global.Workcenters;
using GO.Global.Workcenters.WorkcenterTypes;
using GO.Oms.Shared.Workcenter;
using GuideRecipeBackend.WorkcenterModels;

namespace GuideRecipeBackend
{
    public class WorkcentersController : JsonRpcController
    {
        public WorkcentersController()
        {
            FuncSet.Add("getAllWorkcenters", onGetAllWorkcenters);
        }

        [HttpPost]
        [Route("api/workcenters")]
        public object Main([FromBody] JsonRpcRequest request)
        {
            try
            {
                return FuncSet.Invoke(request, e => Error.WriteLine(e.Exception));
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occurred during call to {request.Method}: {ex.Message}", ex);
            }
        }

        private List<WorkcenterModels.Workcenter> onGetAllWorkcenters(dynamic arg)
        {
            var man = ServiceManager.GetService<GO.Global.Workcenters.WorkcenterManager>();
            var allWorkcenters = man.GetWorkcenters();
            List<WorkcenterModels.Workcenter> workcentersList = new List<WorkcenterModels.Workcenter>();

            foreach (var workcenter in allWorkcenters)
            {
                var convertedWorkcenter = new WorkcenterModels.Workcenter
                {
                    WorkcenterId = workcenter.WorkcenterId.ToString(),
                    ParentWorkcenterId = workcenter.ParentWorkcenterId.ToString(),
                    Created = workcenter.Created.ToString(),
                    Description = workcenter.Description,
                    Enabled = workcenter.Enabled,
                    Modified = workcenter.Modified.ToString(),
                    Release = workcenter.Release,
                    Schedule = workcenter.Schedule,
                    WorkcenterName = workcenter.WorkcenterName,
                    WorkcenterControllerType = workcenter.WorkcenterControllerType,
                    Children = new List<WorkcenterModels.Workcenter>() // Initialize Children
                };
                workcentersList.Add(convertedWorkcenter);
            }

            List<WorkcenterModels.Workcenter> organizedWorkcenters = new List<WorkcenterModels.Workcenter>();

            // Define a function to recursively build the hierarchy
            void BuildHierarchy(WorkcenterModels.Workcenter parent)
            {
                var children = workcentersList.Where(w => w.ParentWorkcenterId == parent.WorkcenterId).ToList();
                foreach (var child in children)
                {
                    parent.Children.Add(child);
                    BuildHierarchy(child);
                }
            }

            // Find root workcenters (those without parents)
            var rootWorkcenters = workcentersList.Where(w => string.IsNullOrEmpty(w.ParentWorkcenterId)).ToList();

            // Build hierarchy starting from root workcenters
            foreach (var root in rootWorkcenters)
            {
                organizedWorkcenters.Add(root);
                BuildHierarchy(root);
            }

            return organizedWorkcenters;
        }
    }
}
