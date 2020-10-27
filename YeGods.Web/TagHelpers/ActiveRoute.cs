namespace YeGods.Web
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.AspNetCore.Mvc.ViewFeatures;
  using Microsoft.AspNetCore.Razor.TagHelpers;

  [HtmlTargetElement(Attributes = "is-active-route")]
  public class ActiveRoute : TagHelper
  {
    private IDictionary<string, string> routeValues;

    [HtmlAttributeName("asp-action")]
    public string Action { get; set; }

    [HtmlAttributeName("asp-controller")]
    public string Controller { get; set; }

    [HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix = "asp-route-")]
    public IDictionary<string, string> RouteValues
    {
      get => this.routeValues ?? (this.routeValues =
               new Dictionary<string, string>(
                 StringComparer.OrdinalIgnoreCase));
      set => this.routeValues = value;
    }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      base.Process(context, output);

      if (this.ShouldBeActive())
      {
        MakeActive(output);
      }

      output.Attributes.RemoveAll("is-active-route");
    }

    private bool ShouldBeActive()
    {
      string currentController = this.ViewContext.RouteData.Values["Controller"].ToString();
      string currentAction = this.ViewContext.RouteData.Values["Action"].ToString();

      if (!string.IsNullOrWhiteSpace(this.Controller) && !string.Equals(this.Controller, currentController, StringComparison.CurrentCultureIgnoreCase))
      {
        return false;
      }

      if (!string.IsNullOrWhiteSpace(this.Action) && !string.Equals(this.Action, currentAction, StringComparison.CurrentCultureIgnoreCase))
      {
        return false;
      }

      return this.RouteValues.All(routeValue => this.ViewContext.RouteData.Values.ContainsKey(routeValue.Key) && this.ViewContext.RouteData.Values[routeValue.Key].ToString() == routeValue.Value);
    }

    private static void MakeActive(TagHelperOutput output)
    {
      var classAttribute = output.Attributes.FirstOrDefault(a => a.Name == "class");
      if (classAttribute == null)
      {
        classAttribute = new TagHelperAttribute("class", "active");
        output.Attributes.Add(classAttribute);
      }
      else if (classAttribute.Value == null || classAttribute.Value.ToString().IndexOf("active", StringComparison.Ordinal) < 0)
      {
        output.Attributes.SetAttribute("class", classAttribute.Value == null
          ? "active"
          : classAttribute.Value + " active");
      }
    }
  }
}
