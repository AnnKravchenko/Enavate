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
                try
                {
                    if(entity.LogicalName=="opportunityproduct")
                    {
                        //get opportunity product
                        OpportunityProduct product = xrm.OpportunityProductSet.Where(p => p.Id == entity.Id).FirstOrDefault();
                        if (product != null)
                        {
                            //base amount
                            decimal total = product.Quantity.Value * product.PricePerUnit.Value;
                            product.BaseAmount = new Money(total);
                            //discount and tax
                            product.new_CustomDiscount = new Money(total * (decimal)0.15);
                            decimal manualDiscount = product.ManualDiscountAmount != null ? product.ManualDiscountAmount.Value : (decimal)0;
                            decimal tax = product.Tax != null ? product.Tax.Value : (decimal)0;
                            //extended amount
                            product.ExtendedAmount = new Money((decimal)(product.BaseAmount.Value - product.new_CustomDiscount.Value - manualDiscount + tax));
                            //update product
                            xrm.UpdateObject(product);
                            xrm.SaveChanges();
                        }
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
