using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
//using Task = CrmEarlyBound.Task;
using Business_Logic;

namespace CallTask
{
    public class Call : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            CrmServiceContext xrm = new CrmServiceContext(service);
            if (context.PostEntityImages.Contains("PostCall") && context.PostEntityImages["PostCall"] is Entity)
            {
                PhoneCall call = context.PostEntityImages["PostCall"].ToEntity<PhoneCall>();
                if (call.StateCode is PhoneCallState.Completed)
                {
                    try 
                    {
                        //call.Description += $"\nPhone call was completed at {call.ActualEnd} \nDue date: {call.ScheduledEnd}";
                        bool CallInTime = (int)DateTime.Compare((DateTime)call.ScheduledEnd, (DateTime)call.ActualEnd) >= 0;//>=0 in time, <0 not in time
                        CrmEarlyBound.Task task = xrm.TaskSet.Where(p => p.RegardingObjectId.Id == call.RegardingObjectId.Id).ToList().FirstOrDefault();
                        if(task.StateCode is TaskState.Completed)
                        {
                            bool TaskInTime = (int)DateTime.Compare((DateTime)task.ScheduledEnd, (DateTime)task.ActualEnd) >= 0;
                            BL.ActivityInTime(CallInTime, TaskInTime, call.RegardingObjectId.Id, serviceProvider);
                        }
                        //BL.ActivityInTime(call, CallInTime, call.RegardingObjectId.Id, serviceProvider);
                        //get account from call
                        //EntityReference acc = (EntityReference)call.Attributes["regardingobjectid"];
                        //Entity accc = service.Retrieve(acc.LogicalName, acc.Id,  new ColumnSet(true));
                        //Account account = (Account)accc;
                        //ColumnSet columnSet = new ColumnSet(true);
                        //Account account = (Account)service.Retrieve("account", acc.Id, columnSet);
                        //Entity accEnt = (Entity)account;

                        //IEnumerable<Task> relatedTasks = account.Account_Tasks;
                        //Task related = (Task)relatedTasks.First<Task>();
                        /*
                        List<Task> relatedTasks = xrm.TaskSet.Where(c => c.RegardingObjectId.Id == account.Id).ToList();
                        Task related = relatedTasks.First();
                        //throw new InvalidPluginExecutionException(related == null ? "null" : "not null");
                        if (related.StateCode is TaskState.Completed)//Value cannot be null. Parameter name: source
                        {
                            bool TaskInTime = (int)DateTime.Compare((DateTime)related.ScheduledEnd, (DateTime)related.ActualEnd) >= 0;
                            if (CallInTime && TaskInTime)
                                account.new_NewClientProccess = Account_new_NewClientProccess.Passedintime;
                            else
                                account.new_NewClientProccess = Account_new_NewClientProccess.Passednotintime;
                            service.Update(call);
                            service.Update(account);
                        }
                        */
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException($"An error occurred in Call plug-in: {ex.Message}");
                    }
                }
            }
        }
    }
}
