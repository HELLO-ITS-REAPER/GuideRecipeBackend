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
using GO.Global.Workcenters.Extension;
using GO.Data;
using GO.WcsManager;

namespace GuideRecipeBackend
{
    public class WorkcentersController : JsonRpcController
    {
        public WorkcentersController()
        {
            FuncSet.Add("getAllWorkcenters", onGetAllWorkcenters);
            FuncSet.Add("getAllLocations", onGetAllLocations);
            FuncSet.Add("createWorkcenter", onCreateWorkcenter);
            FuncSet.Add("deleteWorkcenter", onDeleteWorkcenter);
            FuncSet.Add("updateWorkcenterProperty", onUpdateWorkcenterProperty);
            FuncSet.Add("cleanData", onCleanData);
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
                    WorkcenterType = workcenter.WorkcenterType,
                    WorkcenterControllerType = workcenter.WorkcenterControllerType,
                    Location = new Locations
                    {
                        LocationKey = workcenter.LocationKey,
                        LocationType = workcenter.LocationType
                    },
                    Children = new List<WorkcenterModels.Workcenter>()
                };
                workcentersList.Add(convertedWorkcenter);
            }

            List<WorkcenterModels.Workcenter> organizedWorkcenters = new List<WorkcenterModels.Workcenter>();

            // Define a function to recursively build the hierarchy
            void HierarchyBuilder(WorkcenterModels.Workcenter parent)
            {
                var children = workcentersList.Where(w => w.ParentWorkcenterId == parent.WorkcenterId).ToList();
                foreach (var child in children)
                {
                    parent.Children.Add(child);
                    HierarchyBuilder(child);
                }
            }

            // Find root workcenters (those without parents)
            var rootWorkcenters = workcentersList.Where(w => string.IsNullOrEmpty(w.ParentWorkcenterId)).ToList();

            // Build hierarchy starting from root workcenters
            foreach (var root in rootWorkcenters)
            {
                organizedWorkcenters.Add(root);
                HierarchyBuilder(root);
            }

            return organizedWorkcenters;
        }

        private object onGetAllLocations(dynamic arg)
        {
            var man = ServiceManager.GetService<ILocationManager>();
            var locationCollection = man.GetLocations();
            List<Locations> locationsList = new List<Locations>();

            foreach (var location in locationCollection)
            {
                var convertedLocations = new Locations
                {
                    LocationKey = location.Key,
                    LocationType = location.Type,
                };
                locationsList.Add(convertedLocations);
            }
            return locationsList;
        }

        private object onCreateWorkcenter(dynamic arg)
        {
            var man = ServiceManager.GetService<GO.Global.Workcenters.WorkcenterManager>();
            var newWorkcenter = man.CreateWorkcenter();
            newWorkcenter.WorkcenterId = Guid.NewGuid();
            newWorkcenter.WorkcenterName = (string)arg.workcenterName;
            newWorkcenter.Created = DateTime.Now;
            man.WriteWorkcenter(newWorkcenter);
            var convertedWorkcenter = new WorkcenterModels.Workcenter
            {
                WorkcenterId = newWorkcenter.WorkcenterId.ToString(),
                ParentWorkcenterId = newWorkcenter.ParentWorkcenterId.ToString(),
                Created = newWorkcenter.Created.ToString(),
                Description = newWorkcenter.Description,
                Enabled = newWorkcenter.Enabled,
                Modified = newWorkcenter.Modified.ToString(),
                Release = newWorkcenter.Release,
                Schedule = newWorkcenter.Schedule,
                WorkcenterName = newWorkcenter.WorkcenterName,
                WorkcenterType = newWorkcenter.WorkcenterType,
                WorkcenterControllerType = newWorkcenter.WorkcenterControllerType,
                Children = new List<WorkcenterModels.Workcenter>()
            };
            return convertedWorkcenter;
        }

        private object onDeleteWorkcenter(dynamic arg)
        {
            var man = ServiceManager.GetService<GO.Global.Workcenters.WorkcenterManager>();
            var getWorkcenter = man.GetWorkcenter((string)arg.name, true);
            man.DeleteWorkcenter(getWorkcenter);
            return getWorkcenter;
        }

        private object onCleanData(dynamic arg)
        {
            var man = ServiceManager.GetService<GO.Global.Workcenters.WorkcenterManager>();
            return man.GetWorkcenters();
        }

        private object onUpdateWorkcenterProperty(dynamic arg)
        {

            var man = ServiceManager.GetService<GO.Global.Workcenters.WorkcenterManager>();
            var data = man.GetWorkcenter((string)arg.name, true);
            data.SetValue((string)arg.property, (string)arg.newValue);
            man.WriteWorkcenter(data);
            var convertedWorkcenter = new WorkcenterModels.Workcenter
            {
                WorkcenterId = data.WorkcenterId.ToString(),
                ParentWorkcenterId = data.ParentWorkcenterId.ToString(),
                Created = data.Created.ToString(),
                Description = data.Description,
                Enabled = data.Enabled,
                Modified = data.Modified.ToString(),
                Release = data.Release,
                Schedule = data.Schedule,
                WorkcenterName = data.WorkcenterName,
                WorkcenterType = data.WorkcenterType,
                WorkcenterControllerType = data.WorkcenterControllerType,
                Location = new Locations
                {
                    LocationKey = data.LocationKey,
                    LocationType = data.LocationType
                },
                Children = new List<WorkcenterModels.Workcenter>()
            };
            return convertedWorkcenter;
        }
    }
}
