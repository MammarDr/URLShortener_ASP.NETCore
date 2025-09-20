using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.ComponentModel;
using System.Security.AccessControl;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace UrlShortener.Validation
{
    public class ArrayModelBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {

            var key = bindingContext.ModelName;
            if (string.IsNullOrEmpty(key))
                key = bindingContext.OriginalModelName;

            var providedValue = bindingContext.ValueProvider
                .GetValue(key).ToString();

            if (string.IsNullOrEmpty(providedValue))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // GetGenericArguments() → returns an array of the generic type parameters.
            // Enumerable<int> -> [ typeof(int) ] -> we grab first
            var genericType = bindingContext.ModelType.GetGenericArguments()[0];
            var x = bindingContext.ModelType.GetGenericArguments();
            var y = bindingContext.ModelType.GenericTypeArguments;
            var converter = TypeDescriptor.GetConverter(genericType);

            try
            {
                var objectArray = providedValue
                    .Split([","], StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => converter.ConvertFromString(x.Trim()))
                    .ToArray();

                var guidArray = Array.CreateInstance(genericType, objectArray.Length);
                objectArray.CopyTo(guidArray, 0);

                bindingContext.Model = guidArray;

                bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid array format.");
            }


            
            return Task.CompletedTask;
        }
    }

    public class ArrayModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Only act on IEnumerable<T>, T[], or List<T> from Route/Query
            if (context.Metadata.IsEnumerableType && context.BindingInfo.BindingSource?.DisplayName != "Body")
            {
                // List<int> -> int
                var elementType = context.Metadata.ElementType
                                  ?? context.Metadata.ModelType.GetElementType();

                if (elementType != null)
                {
                    // ArrayModelBinder<T> -> ArrayModelBinder<int> 
                    var binderType = typeof(ArrayModelBinder<>).MakeGenericType(elementType);

                    // ArrayModelBinder<int> -> IModelBinder
                    return (IModelBinder)Activator.CreateInstance(binderType)!;
                }
            }

            return null;
        }
    }
}
