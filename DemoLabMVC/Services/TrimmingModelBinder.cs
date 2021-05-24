using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;
namespace LabDemoWebASPMVC.Services
{
    /// <summary>
    ///   Service dùng thể triming các khoảng trống cho các input trong ứng dụng
    /// </summary>
    /// <Modified>
    /// Name Date Comments
    /// khanhnn 5/24/2021 created
    /// </Modified>
    public class TrimmingModelBinder
    : IModelBinder
    {
        private readonly IModelBinder FallbackBinder;

        public TrimmingModelBinder(IModelBinder fallbackBinder)
        {
            FallbackBinder = fallbackBinder ?? throw new ArgumentNullException(nameof(fallbackBinder));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult.FirstValue is string str &&
                !string.IsNullOrEmpty(str))
            {
                if (bindingContext.ModelName == "Id" || bindingContext.ModelName == "Password")
                {

                    bindingContext.Result = ModelBindingResult.Success(str);
                }
                else
                    bindingContext.Result = ModelBindingResult.Success(str.Trim());
                return Task.CompletedTask;
            }

            return FallbackBinder.BindModelAsync(bindingContext);
        }
    }
}
