using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CreateActivity
{
    public class CreateTask : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if(context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                if (entity.LogicalName != "account")
                    return;
                try
                {
                    
                    Entity task = new Entity("task");
                    task["subject"] = "Actualize details";
                    task["scheduledstart"] = DateTime.Now;
                    task["scheduledend"] = DateTime.Now.AddDays(7);
                    if(context.OutputParameters.Contains("id"))
                    {
                        Guid regardingObjectId = new Guid(context.OutputParameters["id"].ToString());
                        task["regardingobjectid"] = new EntityReference("account", regardingObjectId);
                    }
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                    tracingService.Trace("Create Task Plugin: Create the task activity. ");
                    service.Create(task);
                }
                catch(Exception ex)
                {
                    throw new InvalidPluginExecutionException($"An error occurred in CreateTask plug-in: {ex.Message} ", ex);
                }
            }
        }
    }
}
