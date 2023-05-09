﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infrastructure.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Infrastructure.Resources.Resources", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The e-mail is already confirmed.
        /// </summary>
        public static string EmailAlreadyConfirmed {
            get {
                return ResourceManager.GetString("EmailAlreadyConfirmed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please confirm your email: {0}.
        /// </summary>
        public static string EmailConfirmationMessage {
            get {
                return ResourceManager.GetString("EmailConfirmationMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Confirmation e-mail has been resent.
        /// </summary>
        public static string EmailConfirmationResendSuccess {
            get {
                return ResourceManager.GetString("EmailConfirmationResendSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to e-Mail confirmation : Flitch.
        /// </summary>
        public static string EmailConfirmationSubject {
            get {
                return ResourceManager.GetString("EmailConfirmationSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The e-mail has been successfully confirmed.
        /// </summary>
        public static string EmailConfirmationSuccess {
            get {
                return ResourceManager.GetString("EmailConfirmationSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Follow this link to reset your password: {0}.
        /// </summary>
        public static string EmailPasswordResetMessage {
            get {
                return ResourceManager.GetString("EmailPasswordResetMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password reset email has been queued. If you cannot see an email then consider requesting new message.
        /// </summary>
        public static string EmailPasswordResetMessageSent {
            get {
                return ResourceManager.GetString("EmailPasswordResetMessageSent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password reset : Flitch.
        /// </summary>
        public static string EmailPasswordResetSubject {
            get {
                return ResourceManager.GetString("EmailPasswordResetSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user with such e-mail already exists.
        /// </summary>
        public static string EmailTaken {
            get {
                return ResourceManager.GetString("EmailTaken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to e-mail was not confirmed by the user.
        /// </summary>
        public static string EmailWasNotConfirmed {
            get {
                return ResourceManager.GetString("EmailWasNotConfirmed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password has been successfully changed.
        /// </summary>
        public static string PasswordChangedSuccess {
            get {
                return ResourceManager.GetString("PasswordChangedSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified password is invalid.
        /// </summary>
        public static string PasswordInvalid {
            get {
                return ResourceManager.GetString("PasswordInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password has been successfully reset.
        /// </summary>
        public static string PasswordResetSuccess {
            get {
                return ResourceManager.GetString("PasswordResetSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The e-mail must be specified.
        /// </summary>
        public static string SpecifyEmail {
            get {
                return ResourceManager.GetString("SpecifyEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The confirmation token must be specified.
        /// </summary>
        public static string SpecifyEmailConfirmationToken {
            get {
                return ResourceManager.GetString("SpecifyEmailConfirmationToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New password must be specified.
        /// </summary>
        public static string SpecifyNewPassword {
            get {
                return ResourceManager.GetString("SpecifyNewPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Old password must be specified.
        /// </summary>
        public static string SpecifyOldPassword {
            get {
                return ResourceManager.GetString("SpecifyOldPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The password must be specified.
        /// </summary>
        public static string SpecifyPassword {
            get {
                return ResourceManager.GetString("SpecifyPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reset token must be specified.
        /// </summary>
        public static string SpecifyResetToken {
            get {
                return ResourceManager.GetString("SpecifyResetToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The username must be specified.
        /// </summary>
        public static string SpecifyUserName {
            get {
                return ResourceManager.GetString("SpecifyUserName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user doesn&apos;t exist.
        /// </summary>
        public static string UserDoesntExist {
            get {
                return ResourceManager.GetString("UserDoesntExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user with such username already exists.
        /// </summary>
        public static string UserNameTaken {
            get {
                return ResourceManager.GetString("UserNameTaken", resourceCulture);
            }
        }
    }
}
