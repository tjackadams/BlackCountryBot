using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlackCountryBot.Web.Components
{
    public class Display<TValue> : ComponentBase
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        [Parameter] public Expression<Func<TValue>> For { get; set; }

        protected override void OnParametersSet()
        {
            if (For == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a value for the " +
                                                    $"{nameof(For)} parameter.");
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddContent(2, GetStringValue());
            builder.CloseElement();
        }

        private string GetStringValue()
        {
            var expression = (MemberExpression)For.Body;
            if (!(expression.Member.GetCustomAttribute(typeof(DisplayFormatAttribute)) is DisplayFormatAttribute stringFormatAttribute))
            {
                var func = For.Compile();
                var result = func();

                return result == null ? string.Empty : func().ToString();
            }

            var stringFormat = stringFormatAttribute.DataFormatString;
            var compiledExpression = For.Compile();
            return string.Format(stringFormat, compiledExpression());
        }
    }
}
