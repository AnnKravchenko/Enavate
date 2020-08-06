using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk;

namespace Plugins
{
    public class UserNote : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            CrmServiceContext xrm = new CrmServiceContext(service);
            if(context.PostEntityImages.Contains("PostNote") && context.PostEntityImages["PostNote"] is Entity)
            {
                new_usernote usernote = context.PostEntityImages["PostNote"].ToEntity<new_usernote>();
                try
                {
                    if(usernote.new_UserFullName != null)
                    {
                        SystemUser user = xrm.SystemUserSet.Where(p => p.FullName == usernote.new_UserFullName).FirstOrDefault();
                        usernote.new_NoteUserID = user.SystemUserId.ToString();
                    }
                    service.Update(usernote);
                }
                catch(Exception ex)
                {
                    throw new InvalidPluginExecutionException($"An error occurred in UserNote plug-in: {ex.Message}");
                }
            }
        }
    }
    public class UpdateNote : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            CrmServiceContext xrm = new CrmServiceContext(service);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                try
                {
                    Entity note = (Entity)context.InputParameters["Target"];//.ToEntity<Annotation>();
                    new_usernote usernote = xrm.new_usernoteSet.Where(p => p.new_NoteId == note.Id.ToString()).FirstOrDefault();
                    if (usernote != null && usernote.new_NoteUserID != null)
                    {
                        //Update ModifiedBy field

                        Guid UserId = new Guid(usernote.new_NoteUserID);
                        SystemUser user = xrm.SystemUserSet.Where(p => p.SystemUserId == UserId).FirstOrDefault();
                        note["modifiedby"] = user.ToEntityReference();    
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException($"An error occurred in UpdateNote plug-in: {ex.Message}");
                }
            }
        }
    }
}
