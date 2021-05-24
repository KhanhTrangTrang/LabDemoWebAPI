using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace LabDemoWebASPMVC.Services
{
    /// <summary>
    ///   Service dùng thể triming các khoảng trống cho các input trong ứng dụng
    /// </summary>
    /// <Modified>
    /// Name Date Comments
    /// khanhnn 5/24/2021 created
    /// </Modified>
    public class TrimmingModelBinderProvider
    : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.Metadata.IsComplexType && context.Metadata.ModelType == typeof(string))
            {
                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                return new TrimmingModelBinder(new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory));
            }

            return null;
        }
    }
}
