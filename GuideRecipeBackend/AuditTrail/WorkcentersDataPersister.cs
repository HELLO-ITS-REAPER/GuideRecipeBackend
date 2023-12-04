using GO.Guide.Actions;
using GO.Guide;
using GO.Mes.AuditTrail;
using GO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GO.Global.Workcenters;
using GO.Oms.Shared.DataLayer;
using GO.Oms.Shared.Workcenter;

namespace GuideRecipeBackend.AuditTrail
{
    [
        ValueContainerObjectType(new[] {
            typeof(GO.Global.Workcenters.Workcenter),
        })
    ]
    public class WorkcentersDataPersister : IValueContainerPersister
    {
        public ValueContainer CreateValueContainer(Type objectType, string parentObjectId)
        {
            if (objectType == typeof(GO.Global.Workcenters.Workcenter))
            {
                ValueContainer valueContainer = null;
                var man = GO.AppManager.Current.ServiceManager.GetService<IWorkcenterManager>();
                var workcenter = man.CreateWorkcenter();
                workcenter.WorkcenterName = parentObjectId;
                workcenter.WorkcenterId = Guid.NewGuid();
                workcenter.Created = DateTime.Now;
                man.WriteWorkcenter(workcenter);
                
                return (ValueContainer)workcenter;
            }
            return null;
        }

        public void DeleteValueContainer(Type objectType, string objectId)
        {
            var workDB = GO.AppManager.Current.ServiceManager.GetService<IWorkcenterManager>();
            if (objectType == typeof(GO.Global.Workcenters.Workcenter))
            {
                var workcenter = workDB.GetWorkcenter(Guid.Parse(objectId), true);
                workDB.DeleteWorkcenter(workcenter);
            }
        }

        public object GetId(Type objectType, ValueContainer container)
        {
            return container.GetValue("workcenterId");
        }

        public object UpdateValueContainer(Type objectType, string objectId, string changedPropertyName, object changedPropertyValue)
        {
            var workDB = GO.AppManager.Current.ServiceManager.GetService<IWorkcenterManager>();
            IWorkcenter workcenter = null;
            if (objectType == typeof(GO.Global.Workcenters.Workcenter))
                workcenter = workDB.GetWorkcenter(Guid.Parse(objectId), true);

            var oldValue = workcenter.GetValue(changedPropertyName);
            if (oldValue != null && oldValue.Equals(changedPropertyValue)) return oldValue;


            workcenter.SetValue(changedPropertyName, changedPropertyValue);
            workDB.WriteWorkcenter(workcenter);
            return oldValue?.ToString();
        }

        public void UpdateValueContainerCollection(Type parentType, string parentId, string collectionName, string childId, string updateType)
        {
            var man = GO.AppManager.Current.ServiceManager.GetService<IWorkcenterManager>();
            //man.RegisterResource(resourceType:);
            throw new NotImplementedException();
        }
    }
}