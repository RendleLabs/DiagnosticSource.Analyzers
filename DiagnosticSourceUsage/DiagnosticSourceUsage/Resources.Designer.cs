﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiagnosticSourceUsage {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DiagnosticSourceUsage.Resources", typeof(Resources).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DiagnosticSource.Write argument does not match IsEnabled argument.
        /// </summary>
        internal static string MismatchGuardDescription {
            get {
                return ResourceManager.GetString("MismatchGuardDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DiagnosticSource.Write({0}, ...) does not match IsEnabled({1}).
        /// </summary>
        internal static string MismatchGuardMessageFormat {
            get {
                return ResourceManager.GetString("MismatchGuardMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mismatched DiagnosticSource.IsEnabled argument.
        /// </summary>
        internal static string MismatchGuardTitle {
            get {
                return ResourceManager.GetString("MismatchGuardTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Calls to DiagnosticSource.Write should be guarded with IsEnabled.
        /// </summary>
        internal static string NoGuardDescription {
            get {
                return ResourceManager.GetString("NoGuardDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Call to DiagnosticSource.Write should be guarded with IsEnabled.
        /// </summary>
        internal static string NoGuardMessageFormat {
            get {
                return ResourceManager.GetString("NoGuardMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unguarded DiagnosticSource.Write.
        /// </summary>
        internal static string NoGuardTitle {
            get {
                return ResourceManager.GetString("NoGuardTitle", resourceCulture);
            }
        }
    }
}
