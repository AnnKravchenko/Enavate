using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using CrmEarlyBound;

namespace Plugins
{
    public class QuoteProducts : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            CrmServiceContext xrm = new CrmServiceContext(service);
            try
            {
                if (context.PostEntityImages.Contains("PostQuoteProduct") && context.PostEntityImages["PostQuoteProduct"] is Entity)
                {
                    QuoteDetail QuoteProduct = context.PostEntityImages["PostQuoteProduct"].ToEntity<QuoteDetail>();
                    Quote quote = xrm.QuoteSet.Where(p => p.Id == QuoteProduct.QuoteId.Id).FirstOrDefault();
                    OpportunityProduct oppProd;
                    //Looking for opportunity product in related opportunity 
                    if (!(bool)QuoteProduct.IsProductOverridden)//if QuoteProduct is existing product
                    { oppProd = xrm.OpportunityProductSet.Where(p => p.ProductId == QuoteProduct.ProductId && p.OpportunityId == quote.OpportunityId).FirstOrDefault();}
                    else//if QuoteProduct is write-in product
                    { oppProd = xrm.OpportunityProductSet.Where(p => p.OpportunityId == quote.OpportunityId && p.ProductDescription == QuoteProduct.ProductDescription).FirstOrDefault(); }
                    //Updating custom fields
                    if (oppProd != null)
                    {
                        QuoteProduct.new_Comment = oppProd.new_Comment;
                        QuoteProduct.new_Country = oppProd.new_Country;
                    }
                    service.Update(QuoteProduct);
                }
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred in QuoteProducts plug-in: {ex.Message}");
            }   
        }
    }
}
