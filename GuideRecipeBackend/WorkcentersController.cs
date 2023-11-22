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

namespace GuideRecipeBackend
{
    public class WorkcentersController : JsonRpcController
    {
        public WorkcentersController()
        {
            FuncSet.Add("getAllWorkcenters", onGetAllWorkcenters);
            FuncSet.Add("getAllLocations", onGetAllLocations);
            FuncSet.Add("getAllWorkcenterTypes", onGetAllWorkcenterTypes);
            FuncSet.Add("createWorkcenter", onCreateWorkcenter);
            FuncSet.Add("deleteWorkcenter", onDeleteWorkcenter);
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
            return new List<string>
            {
                GO.ProductionAssets.Datalayer.WorkcenterTypes.Enterprise,
                GO.ProductionAssets.Datalayer.WorkcenterTypes.Site,
                GO.ProductionAssets.Datalayer.WorkcenterTypes.Area,
                GO.ProductionAssets.Datalayer.WorkcenterTypes.Workcenter,
                GO.ProductionAssets.Datalayer.WorkcenterTypes.Workcell
            };
        }

        private object onGetAllLocations(dynamic arg)
        {
            var man = ServiceManager.GetService<ILocationManager>();
            return man.GetLocations();
        }

        private object onCreateWorkcenter(dynamic arg)
        {
            var man = ServiceManager.GetService<GO.Global.Workcenters.WorkcenterManager>();
            var newWorkcenter = man.CreateWorkcenter();
            newWorkcenter.WorkcenterId = Guid.NewGuid();
            newWorkcenter.WorkcenterName = (string)arg.workcenterName;
            newWorkcenter.Created = DateTime.Now;
            man.WriteWorkcenter(newWorkcenter);
            return newWorkcenter;
        }

        private object onDeleteWorkcenter(dynamic arg)
        {
            var man = ServiceManager.GetService<GO.Global.Workcenters.WorkcenterManager>();
            var getWorkcenter = man.GetWorkcenter((string)arg.name, true);
            man.DeleteWorkcenter(getWorkcenter);
            return getWorkcenter;
        }

        private object onUpdateWorkcenterProperty(dynamic arg)
        {
            var man = ServiceManager.GetService<GO.Global.Workcenters.WorkcenterManager>();
            var data = man.GetWorkcenter((string)arg.name, true);
            data.SetValue((string)arg.property, (string)arg.newValue);
            man.WriteWorkcenter(data);
            return data;
        }
    }
}
