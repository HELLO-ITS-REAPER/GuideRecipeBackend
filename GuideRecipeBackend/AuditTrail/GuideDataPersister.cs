using GO;
using GO.Guide;
using GO.Guide.Actions;
using GO.Mes.AuditTrail;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuideRecipeBackend.AuditTrail
{
    [
        ValueContainerObjectType(new[] { 
            typeof(NativeAction), 
            typeof(Recipe) 
        })
    ]
    public class GuideDataPersister : IObjectPersister
    {

        // We expect the auxData to contain information on the specific type of action
        public object CreateObject(Type objectType, string parentObjectId, string auxData)
        {
            if (objectType == typeof(NativeAction))
            {
                return "{ name: \"new\" }";
            }
            if(objectType == typeof(Recipe))
            {
                return "{ actions: [], name: \"new\" }";
            }
            return null;
        }

        public void DeleteValueContainer(Type objectType, string objectId)
        {
            throw new NotImplementedException();
        }

        public object GetObjectId(Type objectType, object data)
        {
            return -1;
        }

        public object UpdateValueContainer(Type objectType, string objectId, string changedPropertyName, object changedPropertyValue)
        {
            var man = GO.AppManager.Current.ServiceManager.GetService<GuideManager>();
            var action = man.LoadAction(Guid.Parse(objectId));

            var result = action.GetType().GetProperty(changedPropertyName).GetValue(action);
            action.GetType().GetProperty(changedPropertyName).SetValue(action, changedPropertyValue, null);
            man.SaveAction(action, "Inline editing");
            return result;
        }

        public void UpdateValueContainerCollection(Type parentType, string parentId, string collectionName, string childId, string updateType)
        {
            throw new NotImplementedException();
        }

        #region obsolete ValueContainer methods
        [Obsolete("Replaced by CreateObject")]
        public ValueContainer CreateValueContainer(Type objectType, string parentObjectId)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Replaced by GetObjectId")]
        public object GetId(Type objectType, ValueContainer container)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
