using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using CrmEarlyBound;

namespace CustomCalculation
{
    public class CalculationOpportunity :IPlugin
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
                    if(entity.LogicalName=="opportunity")
                    {
                        Opportunity opportunity = xrm.OpportunitySet.Where(p => p.Id == entity.Id).FirstOrDefault();
                        List<OpportunityProduct> products = xrm.OpportunityProductSet.Where(p => p.OpportunityId.Id == opportunity.Id).ToList();
                        decimal detailAmount = (decimal)0;
                        decimal Tax = (decimal)0;
                        decimal lessfreight = (decimal)0;
                        decimal total = (decimal)0;
                        decimal charity = (decimal)0;    
                        //detail amount and Total Tax
                        if (products.Count != 0)
                        {
                            foreach (OpportunityProduct prod in products)
                            {
                                decimal discount = prod.ManualDiscountAmount != null ? prod.ManualDiscountAmount.Value : (decimal)0;
                                decimal customDiscount = prod.new_CustomDiscount != null ? prod.new_CustomDiscount.Value : (decimal)0;
                                detailAmount += prod.BaseAmount.Value - discount - customDiscount;
                                Tax += prod.Tax != null ? prod.Tax.Value : (decimal)0;
                            }
                        }
                        opportunity.TotalLineItemAmount = new Money(detailAmount);
                        opportunity.TotalTax = new Money(Tax);
                                
                        //Pre-Freight Amount
                        lessfreight = detailAmount;
                        if (opportunity.DiscountPercentage != null)
                            lessfreight -= (decimal)detailAmount * (decimal)0.01 * opportunity.DiscountPercentage.Value;
                        if (opportunity.DiscountAmount != null)
                            lessfreight -= opportunity.DiscountAmount.Value;
                        opportunity.TotalAmountLessFreight = new Money(lessfreight);

                        //Total Amount
                        total = lessfreight;
                        if (opportunity.FreightAmount != null)
                            total += opportunity.FreightAmount.Value;
                        charity = lessfreight * (decimal)0.5;
                        opportunity.new_CharityContribution = new Money(charity);
                        total += Tax + charity;
                        opportunity.TotalAmount = new Money(total);

                        xrm.UpdateObject(opportunity);
                        xrm.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException($"An error occurred in Calculation Opportunity plug-in: {ex.Message}");
                }
            }
        }
        
    }
}
