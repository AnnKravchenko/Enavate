using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using CrmEarlyBound;

namespace Workflow
{
    public class SendEmail : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            //throw new NotImplementedException();
            ITracingService tracingService = context.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);
            CrmServiceContext xrm = new CrmServiceContext(service);
            try
            {
                ActivityParty from = new ActivityParty();
                from.PartyId = new EntityReference("systemuser", workflowContext.UserId);
                //SystemUser to = xrm.SystemUserSet.Where(p => p.Id == UserInput.Get(context).Id).FirstOrDefault();
                ActivityParty to = new ActivityParty();
                to.PartyId = new EntityReference("systemuser", UserInput.Get(context).Id);

                Email email = new Email
                {
                    From = new ActivityParty[] { from },
                    To = new ActivityParty[] { to },
                    Subject = "Test Workflow",
                    Description = TextInput.Get(context),
                    DirectionCode = true,
                    ScheduledEnd = DateTime.Now.AddDays(1)
                };
                Guid emailId = service.Create(email);

                SendEmailRequest sendEmail = new SendEmailRequest();
                sendEmail.EmailId = emailId;
                sendEmail.TrackingToken = string.Empty;
                sendEmail.IssueSend = true;
                service.Execute(sendEmail);
                IsSuccess.Set(context,true);
            }
            catch(Exception ex)
            {
                //throw new InvalidPluginExecutionException($"An error occured in worlflow: {ex.Message}");
                IsSuccess.Set(context, false);
            }
        }
        //Input parameters
        [RequiredArgument]
        [Input("To User")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> UserInput { get; set; }

        [RequiredArgument]
        [Input("Text")]
        public InArgument<string> TextInput { get; set; }
        //Output parameters
        [Output("Completed")]
        public OutArgument<bool> IsSuccess { get; set; } 
    }
}
