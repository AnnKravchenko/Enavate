using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection; 
using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Business_Logic;


namespace CallTask
{
    public class TaskCall : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            CrmServiceContext xrm = new CrmServiceContext(service);
            if (context.PostEntityImages.Contains("PostTask") && context.PostEntityImages["PostTask"] is Entity)
            {
                CrmEarlyBound.Task task = context.PostEntityImages["PostTask"].ToEntity<CrmEarlyBound.Task>();
                if (task.StateCode is TaskState.Completed)
                {
                    try
                    {
                        bool TaskInTime = (int)DateTime.Compare((DateTime)task.ScheduledEnd, (DateTime)task.ActualEnd) >= 0;//>=0 in time, <0 not in time
                        
                        //Account account = xrm.AccountSet.Where(p => p.Id == task.RegardingObjectId.Id).First();           
                        PhoneCall related = xrm.PhoneCallSet.Where(c => c.RegardingObjectId.Id == task.RegardingObjectId.Id).First();

                        if (related.StateCode is PhoneCallState.Completed)
                        {
                            bool CallInTime = (int)DateTime.Compare((DateTime)related.ScheduledEnd, (DateTime)related.ActualEnd) >= 0;
                            BL.ActivityInTime(CallInTime, TaskInTime, task.RegardingObjectId.Id, serviceProvider);
                            /*if (TaskInTime && CallInTime)
                                account.new_NewClientProccess = Account_new_NewClientProccess.Passedintime;
                            else
                                account.new_NewClientProccess = Account_new_NewClientProccess.Passednotintime;
                            service.Update(task);
                            xrm.UpdateObject(account);
                            xrm.SaveChanges();*/
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException($"An error occurred in Task plug-in: {ex.Message}");
                    }
                }
            }
        }
    }
}
