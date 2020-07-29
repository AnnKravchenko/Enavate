using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using CrmEarlyBound;
//using Task = CrmEarlyBound.Task;

namespace Business_Logic
{
    public class BL
    {
        public static void ActivityInTime(bool CallInTime, bool TaskInTime,Guid AccountId, IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            CrmServiceContext xrm = new CrmServiceContext(service);
            try
            {
                Account account = xrm.AccountSet.Where(p => p.Id == AccountId).Single();
                if (TaskInTime && CallInTime)
                    account.new_NewClientProccess = Account_new_NewClientProccess.Passedintime;
                else
                    account.new_NewClientProccess = Account_new_NewClientProccess.Passednotintime;
                xrm.UpdateObject(account);
                xrm.SaveChanges();
                /*if (activity is Task)
                {
                    var related = xrm.PhoneCallSet.Where(p => p.RegardingObjectId.Id == account.Id).First();
                    IsCompleted = related.StateCode == PhoneCallState.Completed;
                    if (IsCompleted)
                        ActivityInTime = (int)DateTime.Compare((DateTime)related.ScheduledEnd, (DateTime)related.ActualEnd) >= 0;
                }
                else if (activity is PhoneCall)
                {
                    var related = xrm.TaskSet.Where(p => p.RegardingObjectId.Id == account.Id).First();
                    IsCompleted = related.StateCode == TaskState.Completed;
                    if (IsCompleted)
                        ActivityInTime = (int)DateTime.Compare((DateTime)related.ScheduledEnd, (DateTime)related.ActualEnd) >= 0;
                }
                if (IsCompleted)
                {
                    if (InTime && ActivityInTime)
                        account.new_NewClientProccess = Account_new_NewClientProccess.Passedintime;
                    else
                        account.new_NewClientProccess = Account_new_NewClientProccess.Passednotintime;
                    xrm.UpdateObject(account);
                    xrm.SaveChanges();
                }*/
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred in BussinessLogic.dll: {ex.Message}");
            }
         }
    }
}
