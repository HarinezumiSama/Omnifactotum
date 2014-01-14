﻿using System;
using System.Linq;

namespace Omnifactotum.Annotations
{
    /// <summary>
    /// ASP.NET MVC attribute. Indicates that a parameter is an MVC model type.
    /// Use this attribute for custom wrappers similar to
    /// <c>System.Web.Mvc.Controller.View(String, Object)</c>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class AspMvcModelTypeAttribute : Attribute
    {
    }
}