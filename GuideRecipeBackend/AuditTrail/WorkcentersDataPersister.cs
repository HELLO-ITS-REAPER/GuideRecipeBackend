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

namespace GuideRecipeBackend.AuditTrail
{
    [
        ValueContainerObjectType(new[] {
            typeof(Workcenter),
        })
    ]
    public class WorkcentersDataPersister : IValueContainerPersister
    {
        public ValueContainer CreateValueContainer(Type objectType, string parentObjectId)
        {
            ValueContainer valueContainer = new ValueContainer();
            var man = GO.AppManager.Current.ServiceManager.GetService<IWorkcenterDatabase>();

            if (objectType == typeof(Workcenter))
            {
                valueContainer = new WorkcenterData()
                {
                    Workcenter = parentObjectId,
                    WorkcenterId = Guid.NewGuid(),
                    Created = DateTime.Now,
                };
                man.WriteWorkcenter((WorkcenterData)valueContainer);
                return valueContainer;
            }
            return null;
        }

        public void DeleteValueContainer(Type objectType, string objectId)
        {
            var man = GO.AppManager.Current.ServiceManager.GetService<IWorkcenterDatabase>();
            if(objectType == typeof(Workcenter))
            {
                var workcenter = man.GetWorkcenter(objectId, true);
                man.DeleteWorkcenter(workcenter);
            }
        }

        public object GetId(Type objectType, ValueContainer container)
        {
            return container.GetValue("workcenterId");
        }

        public object UpdateValueContainer(Type objectType, string objectId, string changedPropertyName, object changedPropertyValue)
        {
            throw new NotImplementedException();
        }

        public void UpdateValueContainerCollection(Type parentType, string parentId, string collectionName, string childId, string updateType)
        {
            throw new NotImplementedException();
        }
    }
}