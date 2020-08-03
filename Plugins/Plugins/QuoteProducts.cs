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
                if (context.PostEntityImages.Contains("PostQuote") && context.PostEntityImages["PostQuote"] is Entity)
                {
                    Quote quote = context.PostEntityImages["PostQuote"].ToEntity<Quote>();
                    List<OpportunityProduct> OpportunityProductList = xrm.OpportunityProductSet.Where(p => p.OpportunityId == quote.OpportunityId).ToList();
                    List<QuoteDetail> QuoteProductList = xrm.QuoteDetailSet.Where(p => p.QuoteId.Id == quote.Id).ToList();
                    if (OpportunityProductList.Count == QuoteProductList.Count)
                    {
                        for (int i = 0; i < QuoteProductList.Count(); i++)
                        {
                            OpportunityProduct prod = OpportunityProductList.Where(p => p.ProductDescription == QuoteProductList.ElementAt(i).ProductDescription).FirstOrDefault();
                            if(prod != null)
                            {
                                QuoteProductList.ElementAt(i).new_Comment = prod.new_Comment;
                                QuoteProductList.ElementAt(i).new_Country = prod.new_Country;
                                xrm.UpdateObject(QuoteProductList.ElementAt(i));
                            }
                        }
                        xrm.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred in QuoteProducts plug-in: {ex.Message}");
            }
            
            /*
             * get quote id from image
             * => opportunity => opportunity products => comment and country field
             * (quote products where quoteid = image.id) -> chnge comment and country 
            */
            
        }
    }
}
