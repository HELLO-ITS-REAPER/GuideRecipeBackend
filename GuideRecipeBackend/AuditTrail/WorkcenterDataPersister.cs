using GO;
using GO.Guide;
using GO.Guide.Actions;
using GO.Guide.Actions.Serialization;
using GO.Guide.DataLayer;
using GO.Mes.AuditTrail;
using GO.Windows.Extension;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuideRecipeBackend.Models;
using GO.Global.Workcenters;
using GO.Mes.Web;
using GO.Security;

namespace GuideRecipeBackend.AuditTrail
{
    [
        ValueContainerObjectType(new[] {
            typeof(Workcenter)
        })
    ]
    public class WorkcenterDataPersister : IObjectPersister
    {

        // We expect the auxData to contain information on the specific type of action

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

        public void DeleteValueContainer(Type objectType, string objectId)
        {
            throw new NotImplementedException();
        }

        public object CreateObject(Type objectType, string parentObjectId, string auxData)
        {
            throw new NotImplementedException();
        }

        public object GetObjectId(Type objectType, object data)
        {
            throw new NotImplementedException();
        }

        public object UpdateValueContainer(Type objectType, string objectId, string changedPropertyName, object changedPropertyValue)
        {
            throw new NotImplementedException();
        }

        public void UpdateValueContainerCollection(Type parentType, string parentId, string collectionName, string childId, string updateType)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
