using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Omnifactotum.Wpf
{
    /// <summary>
    ///     Provides helper methods for use in WPF projects.
    /// </summary>
    public static partial class WpfFactotum
    {
        #region Public Methods

        /// <summary>
        ///     Determines whether the code is running in the context of a designer.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the code is running in the context of a designer; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInDesignMode()
        {
            try
            {
                return (bool)DesignerProperties
                    .IsInDesignModeProperty
                    .GetMetadata(typeof(DependencyObject))
                    .DefaultValue;
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                {
                    throw;
                }

                return false;
            }
        }

        #endregion
    }
}