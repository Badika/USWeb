using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UpsCoolWeb.Components.Mvc
{
    public static class ModelStateDictionaryExtensions
    {
        public static Dictionary<String, String> Errors(this ModelStateDictionary modelState)
        {
            return modelState
                .Where(state => state.Value.Errors.Count > 0)
                .ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.Errors
                        .Select(model => model.ErrorMessage)
                        .FirstOrDefault(error => !String.IsNullOrEmpty(error))
            );
        }
    }
}
