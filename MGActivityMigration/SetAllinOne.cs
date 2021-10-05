using Microsoft.Xrm.Sdk;
using System;

namespace DeltaN.BusinessSolutions.ActivityMigration
{
    public class SetAllinOne : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public SetAllinOne(string unsecureConfig, string secureConfig)
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecureConfig;
        }
        #endregion
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            
            try
            {
                tracer.Trace("Plugin started");

                Entity entity = (Entity)context.InputParameters["Target"];
                tracer.Trace("Entity found");
                DateTime modifiedOn = DateTime.Now;

                if (context.PreEntityImages.Contains("preImage"))
                {
                    Entity preImageEntity = context.PreEntityImages["preImage"];
                    tracer.Trace("preImage found");

                    if (preImageEntity.Contains("dnbs_overriddenmodifiedon"))
                    {
                        tracer.Trace("dnbs_OverridenModifiedOn contains data, " + preImageEntity["dnbs_overriddenmodifiedon"]);

                        entity["modifiedon"] = preImageEntity["dnbs_overriddenmodifiedon"];
                        tracer.Trace("ModifiedOn overwritten with dnbs_OverridenModifiedOn");
                    }
                } else
                { 
                    if (entity.Contains("dnbs_overriddenmodifiedon") && entity.Contains("modifiedon") == false)
                    {
                        tracer.Trace("ModifiedOn contains no data");

                        entity.Attributes.Add("modifiedon", entity["dnbs_overriddenmodifiedon"]);
                        tracer.Trace("ModifiedOn added with dnbs_OverridenModifiedOn, " + entity["dnbs_overriddenmodifiedon"]);
                    }
                    else if (entity.Contains("dnbs_overriddenmodifiedon") && entity.Contains("modifiedon"))
                    {
                        tracer.Trace("ModifiedOn already contains data");

                        entity["modifiedon"] = entity["dnbs_overriddenmodifiedon"];
                        tracer.Trace("ModifiedOn overwritten with dnbs_OverridenModifiedOn, " + entity["dnbs_overriddenmodifiedon"]);
                    }
                }

                if (context.PreEntityImages.Contains("preImage"))
                {
                    Entity preImageEntity = context.PreEntityImages["preImage"];
                    tracer.Trace("preImage found");

                    if (preImageEntity.Contains("modifiedby") && preImageEntity.Contains("dnbs_overriddenmodifiedby"))
                    {
                        tracer.Trace("dnbs_overriddenmodifiedby has value: " + preImageEntity["dnbs_overriddenmodifiedby"]);

                        entity["modifiedby"] = preImageEntity["dnbs_overriddenmodifiedby"];
                        tracer.Trace("modifiedby overwritten with dnbs_overriddenmodifiedby");
                    }
                } else
                {
                    if (entity.Contains("modifiedby") == false && entity.Contains("dnbs_overriddenmodifiedby"))
                    {
                        entity.Attributes.Add("modifiedby", entity["dnbs_overriddenmodifiedby"]);
                    }
                    else if (entity.Contains("modifiedby") && entity.Contains("dnbs_overriddenmodifiedby"))
                    {
                        tracer.Trace("dnbs_overriddenmodifiedby value: " + entity["dnbs_overriddenmodifiedby"]);

                        entity["modifiedby"] = entity["dnbs_overriddenmodifiedby"];
                        tracer.Trace("modifiedby overwritten withdnbs_overriddenmodifiedby");
                    }
                }

                if (entity.Contains("createdby") && entity.Contains("dnbs_overriddencreatedby"))
                {
                    tracer.Trace("dnbs_overriddencreatedby value: " + entity["dnbs_overriddencreatedby"]);

                    entity["createdby"] = entity["dnbs_overriddencreatedby"];
                    tracer.Trace("createdby overwritten with dnbs_overriddencreatedby");
                }

                tracer.Trace("Plugin done");

            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}
