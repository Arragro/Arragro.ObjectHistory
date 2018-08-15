using Arragro.ObjectHistory.RazorClassLib.Helpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace Arragro.ObjectHistory.RazorClassLib
{ 
    public class ObjectHistoryLinkTagHelper : TagHelper
	{
        public Type Type { get; set; }
        public string Id { get; set; }
        public string LinkText { get; set; } = "History";

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
            output.TagName = "a";
            output.Attributes.SetAttribute("href", $"/arragro-object-history/{ObjectHistoryHelper.GetObjectHistoryFullNameAndId(Type, Id)}");
            output.Content.SetContent(LinkText);
        }
	}
}