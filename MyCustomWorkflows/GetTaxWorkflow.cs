using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System.Threading;
using System.Collections.ObjectModel;


namespace MyCustomWorkflows
{
    public class GetTaxWorkflow : CodeActivity
    {
        [Input("key")]
        public InArgument<string> Key { get; set; }

        [Output("Tax")]
        public OutArgument<string> Tax { get; set; } 
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            string key =  Key.Get(executionContext);

            // Get data from configuration entity
            // Call organization web service

            QueryByAttribute query = new QueryByAttribute("nw_configuration");
            query.ColumnSet = new ColumnSet(new String[] { "nw_value" });
            query.AddAttributeValue("nw_name", key);
            EntityCollection entityCollection = service.RetrieveMultiple(query);

            if(entityCollection.Entities.Count != 1)
            {
                tracingService.Trace("Something is wrong with configuration");
            }

            Entity config = entityCollection.Entities.FirstOrDefault();

            Tax.Set(executionContext, config.Attributes["nw_value"].ToString());

        }
    }
}
