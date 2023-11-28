using GO.Global.DataLayer;
using GO.Global.Products;
using GO.Global.Workcenters;
using GO.Mes.AuditTrail;
using GO.Mes.Shared;
using GO.Mes.Shared.Coordinator;
using GO.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace GO.Mes.Web
{
    public class AuditTrailController : JsonRpcController
    {
        private Dictionary<string, Type> m_supportedTypes;
        public AuditTrailController()
        {
            m_supportedTypes = new Dictionary<string, Type>() {
                { "Ability", typeof(Ability)},
                { "AbilityParameterGroup", typeof(AbilityParameterGroup)},
                { "AbilityRequiredParameter", typeof(AbilityRequiredParameter)},
                { "ConditionalExitPath", typeof(ConditionalExitPath)},
                { "ExecutionCondition", typeof(ExecutionCondition)},
                { "Material", typeof(WorkModelMaterialSpecification)},
                { "MaterialClass", typeof(MaterialClass) },
                { "MaterialDefinition", typeof(MaterialDefinition) },
                { "JobOrderParameter", typeof(JobOrderParameter) },
                { "Product", typeof(Product)},
                { "Reason", typeof(Reason) },
                { "WorkModel", typeof(WorkModel)},
                { "WorkModelAbilitySpecification", typeof(WorkModelAbilitySpecification) },
                { "WorkModelAbilitySpecificationParameter", typeof(WorkModelAbilitySpecificationParameter) },
                { "WorkModelMaterialDefinition", typeof(WorkModelMaterialDefinition)},
                { "WorkModelMaterialDefinitionProperty", typeof(WorkModelMaterialDefinitionProperty)},
                { "WorkModelMaterialClass", typeof(WorkModelMaterialClass)},
                { "WorkModelMaterialClassProperty", typeof(WorkModelMaterialClassProperty)},
                { "WorkModelMapping", typeof(WorkModelMapping) },
                { "WorkModelMesAction", typeof(WorkModelMesAction) },
                { "WorkModelMesActionParameter", typeof(WorkModelMesActionParameter) },
                { "WorkModelMilestone", typeof(WorkModelMilestone)},
                // GO.Guide types
                { "Recipe", typeof(GO.Guide.Recipe)},
                { "Action", typeof(GO.Guide.Actions.NativeAction)}
                };
            FuncSet.Add("createAuditEntry", OnCreateAuditEntry);
            FuncSet.Add("deleteObject", OnDeleteObject);
            FuncSet.Add("getNewObject", OnGetNewObject);
            FuncSet.Add("hierarchyChange", OnHierarchyChange);
            FuncSet.Add("propertyChange", OnPropertyChange);
            FuncSet.Add("addComment", OnAddComment);
            FuncSet.Add("loadAuditTrail", OnLoadAuditTrail);
        }

     

        [HttpPost]
        [Route("api/mes/audittrail")]
        public object Main([FromBody] JsonRpcRequest request)
        {
            try
            {
                //Info.WriteLine($"{request.Method}, {request.Params}");
                return FuncSet.Invoke(request, e => Error.WriteLine(e.Exception));
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occurred during call to {request.Method}: {ex.Message}", ex);
            }
        }


        private object OnCreateAuditEntry(dynamic @params)
        {
            var objectType = (string)@params.objectType;
            var objectId = (string)@params.objectId;
            var userId = (object)@params.userId;

            if (!m_supportedTypes.ContainsKey(objectType))
                throw new NotSupportedException($"The object type {objectType} is not supported by the AuditTrailController");

            var auditTrailEntry = new AuditTrailEntry()
            {
                ObjectType = objectType,
                ObjectId = objectId,
                ParameterPath = "",
                UserId = userId?.ToString(),
                UpdateType = ValueContainerUpdateType.Create.ToString()
            };
            var result = ServiceManager.GetService<AuditTrailDatabase>().AddAuditTrailEntry(auditTrailEntry);

            return result;
        }

        private object OnDeleteObject(dynamic @params)
        {
            var objectType = (string)@params.objectType;
            var objectId = (string)@params.objectId;
            var userId = (object)@params.userId;

            if (!m_supportedTypes.ContainsKey(objectType))
                throw new NotSupportedException($"The object type {objectType} is not supported by the MasterDataController");
            ServiceManager.GetService<ValueContainerPersisterService>().DeleteValueContainer(m_supportedTypes[objectType], objectId, userId?.ToString());

            return true;
        }

        private object OnGetNewObject(dynamic @params)
        {
            var objectType = (string)@params.objectType;
            var userId = (object)@params.userId;
            var parentId = (string)@params.parentObjectId;
            var auxData = (string)@params.auxData;

            if (!m_supportedTypes.ContainsKey(objectType))
                throw new NotSupportedException($"The object type {objectType} is not supported by the MasterDataController");
            var result = ServiceManager.GetService<ValueContainerPersisterService>().CreateValueContainer(m_supportedTypes[objectType], userId?.ToString(), "", parentId, auxData);


            return result;
        }

        private object OnHierarchyChange(dynamic @params)
        {
            var parentType = (string)@params.parentType;
            var parentId = (string)@params.parentId;
            var collectionName = (string)@params.collectionName;
            var childId = (object)@params.childId;
            var childType = (string)@params.childType;
            var userId = (object)@params.userId;
            var changeType = (string)@params.changeType;

            var persister = GO.AppManager.Current.ServiceManager.GetService<ValueContainerPersisterService>();
            if (!m_supportedTypes.ContainsKey(parentType))
                throw new NotSupportedException($"Property changed for unknown type: {parentType} with id {parentId}");
            persister.UpdateValueContainerCollection(m_supportedTypes[parentType], parentId, collectionName, childId?.ToString(), childType, changeType, userId?.ToString());

            return true;
        }

        private object OnLoadAuditTrail(dynamic @params)
        {
            var objectType = (string)@params.objectType;
            var objectId = (string)@params.objectId;

            return ServiceManager.GetService<IAuditTrailDatabase>().LoadAuditTrail(objectType, objectId);
        }

        private object OnPropertyChange(dynamic @params)
        {
            var objectType = (string)@params.objectType;
            var objectId = (string)@params.objectId;
            var paramPath = (string)@params.parameterPath;
            var paramValue = (object)@params.parameterValue;
            var userId = (object)@params.userId;
            var changeType = (object)@params.changeType;

            var persister = GO.AppManager.Current.ServiceManager.GetService<ValueContainerPersisterService>();
            if (!m_supportedTypes.ContainsKey(objectType))
                throw new NotSupportedException($"Property changed for unknown type: {objectType} with id {objectId}");
            persister.UpdateValueContainer(m_supportedTypes[objectType], objectId, paramPath, paramValue, userId?.ToString());

            return true;
        }

        private object OnAddComment(dynamic @params)
        {
            var entryType = (string)@params.entryType;
            var entryId = (long)@params.entryId;
            var comment = (string)@params.comment;

            if(entryType == "AuditTrailEntry")
            {
                ServiceManager.GetService<AuditTrailDatabase>().AddAuditTrailEntryComment(entryId, comment);
            }
            if(entryType == "AuditTrailHiearchyEntry")
            {
                ServiceManager.GetService<AuditTrailDatabase>().AddAuditTrailHierarchyEntryComment(entryId, comment);

            }

            return true;
        }

    }
}
