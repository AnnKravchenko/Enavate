using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
namespace CustomCalculation
{
    public class Calculation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            CrmServiceContext xrm = new CrmServiceContext(service);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
            {

                EntityReference entity = (EntityReference)context.InputParameters["Target"];
                //throw new InvalidPluginExecutionException($"{entity.Id.ToString()} ");
                try
                {
                    switch (entity.LogicalName)
                    //if(entity.LogicalName=="opportunityproduct")
                    {
                        case "opportunity":

                            break;
                        case "opportunityproduct":
                            //OpportunityProduct product = entity.ToEntity<OpportunityProduct>();
                            OpportunityProduct product = xrm.OpportunityProductSet.Where(p => p.Id == entity.Id).FirstOrDefault();
                            
                            //throw new InvalidPluginExecutionException(product != null ? "not null" : " null");
                            decimal total = product.Quantity.Value * product.PricePerUnit.Value;
                            product.new_CustomDiscount = new Money(total * (decimal)0.15);
                            product.BaseAmount = new Money(total);
                            product.ExtendedAmount = new Money((decimal)(product.BaseAmount.Value - product.new_CustomDiscount.Value-product.ManualDiscountAmount.Value+product.Tax.Value));
                            xrm.UpdateObject(product);
                            xrm.SaveChanges();                            
                            break;
                        default:
                            break;
                    }
                }
                catch(Exception ex)
                {
                    throw new  InvalidPluginExecutionException($"An error occurred in Calculation plug-in: {ex.Message}");
                }
            }
        }
    }
}
