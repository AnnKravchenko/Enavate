using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm;
using Microsoft.Xrm.Sdk;
using CrmEarlyBound;

namespace CreateActivity
{
    public class CreateCall: IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //get user(CreatedBy), account, phone, creation date(CreatedOn)

            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if (context.PostEntityImages.Contains("Image") && context.PostEntityImages["Image"] is Entity)
            {
                Entity entity = ((Entity)context.PostEntityImages["Image"]).ToEntity<Account>();
                Account postAccount = (Account)entity;
                
                //create phone call activity    
                try
                {
                    PhoneCall call = new PhoneCall();
                    call.Subject = "Call after creation an account";
                    call.ScheduledStart = DateTime.Now.AddDays(1);
                    call.ScheduledEnd = DateTime.Now.AddDays(1);
                    if(!string.IsNullOrEmpty(call.PhoneNumber))
                    {
                        call.PhoneNumber = postAccount.Telephone1.ToString();
                    }
                        
                    
                    ActivityParty from = new ActivityParty();
                    from.PartyId = new EntityReference("systemuser", context.UserId);
                    ActivityParty to = new ActivityParty();
                    to.PartyId = new EntityReference("account", postAccount.Id);

                    call.To = new ActivityParty[] { to };
                    call.From = new ActivityParty[] { from };
                    call.RegardingObjectId = new EntityReference("account",postAccount.Id);
                    
                    // Obtain the organization service reference.

                    tracingService.Trace("Creating phone call activity.");
                    service.Create(call);
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException($"Plugin error: {ex.Message}");
                }
            }
        }
    }
}
