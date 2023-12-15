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
using GO.Global.Workcenters.Extension;
using GO.Data;
using GO.WcsManager;
using GO.Oms.Shared.DataLayer;
using GO.Oms.Shared.Workcenter.Extension;
using GO.Extension;
using GO.Mes.Web;
using GO.Mes.AuditTrail;
using GO.Global.AuditTrail;
using GuideRecipeBackend.AuditTrail;
using System.Activities.Validation;
using GO.Mes;
using System.Resources;
using GO.ProductionAssets.Datalayer;

namespace GuideRecipeBackend
{
    public class WorkcentersController : JsonRpcController
    {
        public WorkcentersController()
        {
            FuncSet.Add("getAllWorkcenters", onGetAllWorkcenters);
            FuncSet.Add("getAllLocations", onGetAllLocations);
            FuncSet.Add("getAllWorkcenterTypes", onGetAllWorkcenterTypes);
            FuncSet.Add("getAllAbilities", onGetAllAbilities);
            FuncSet.Add("createWorkcenter", onCreateWorkcenter);
            FuncSet.Add("deleteWorkcenter", onDeleteWorkcenter);
            FuncSet.Add("createWorkTask", onCreateWorkTask);
            FuncSet.Add("deleteWorkTask", onDeleteWorkTask);
            FuncSet.Add("updateWorkcenterProperty", onUpdateWorkcenterProperty);
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

        private object onGetAllWorkcenters(dynamic arg)
        {
            var man = ServiceManager.GetService<GO.Global.Workcenters.WorkcenterManager>();
            return man.GetWorkcenters();
        }

        private object onGetAllWorkcenterTypes(dynamic arg)
        {
            var allWorkcenterTypes = typeof(WorkcenterTypes).GetFields()
            .Select(f => (string)f.GetValue(null)).ToList();

            return allWorkcenterTypes;
        }

        private object onGetAllLocations(dynamic arg)
        {
            var man = ServiceManager.GetService<ILocationManager>();
            return man.GetLocations();
        }

        private object onGetAllAbilities(dynamic arg)
        {
            var man = ServiceManager.GetService<IAbilityClient>();
            return man.GetAbilities();
        }

        private object onCreateWorkcenter(dynamic arg)
        {
            var persister = ServiceManager.GetService<ValueContainerPersisterService>();
            var workcenter = persister.CreateValueContainer(typeof(GO.Global.Workcenters.Workcenter), userId: (string)arg.userId, remark: "Create new workcenter");
            return workcenter;
        }


        private object onDeleteWorkcenter(dynamic arg)
        {
            var persister = ServiceManager.GetService<ValueContainerPersisterService>();
            persister.DeleteValueContainer(typeof(GO.Global.Workcenters.Workcenter), (string)arg.id, (string)arg.userId, "Deleting a Workcenter");
            return arg.id;
        }

        private object onCreateWorkTask(dynamic arg)
        {
            var man = ServiceManager.GetService<IWorkcenterManager>();
            var workcenter = man.GetWorkcenter((Guid)(arg.workcenterId), true);
            IWorkcenterResource newWorkTask = new WorkcenterResource();
            newWorkTask.ResourceName = (string)arg.taskName;
            workcenter.WorkcenterResources.Add(newWorkTask);
            man.WriteWorkcenter(workcenter);
            return workcenter.WorkcenterResources;
        }

        private object onDeleteWorkTask(dynamic arg)
        {
            var man = ServiceManager.GetService<IWorkcenterManager>();
            var workcenter = man.GetWorkcenter((Guid)(arg.workcenterId), true);
            var removingTask = workcenter.WorkcenterResources.Find(x => x.WorkcenterResourceId.ToString() == (string)arg.taskId);
            workcenter.WorkcenterResources.Remove(removingTask);
            man.WriteWorkcenter(workcenter);
            return workcenter.WorkcenterResources;
        }

        private object onUpdateWorkcenterProperty(dynamic arg)
        {
            var persister = ServiceManager.GetService<ValueContainerPersisterService>();
            persister.UpdateValueContainer(typeof(GO.Global.Workcenters.Workcenter), 
                (string)arg.id, (string)arg.property, (object)arg.newValue, (string)arg.userId, $"Updating {arg.property} value");
            return null;
        }
    }
}
