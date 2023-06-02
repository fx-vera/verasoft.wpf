using System.ComponentModel.Composition;
using VeraSoft.Wpf.Extensions;

namespace VeraSoft.Wpf.Defaults
{
    [Export(typeof(ProfileSettings))]
    public class ProfileSettings : PageSettingsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileSettings"/> class.
        /// </summary>
        public ProfileSettings()
        {
            PageIds = new System.Collections.Generic.List<string>();
            Language = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the owner identifier.
        /// </summary>
        /// <value>
        /// The owner identifier.
        /// </value>
        public int OwnerId { get; set; }
        /// <summary>
        /// Gets or sets the name of the owner.
        /// </summary>
        /// <value>
        /// The name of the owner.
        /// </value>
        public string OwnerName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is public.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is public; otherwise, <c>false</c>.
        /// </value>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the page ids.
        /// </summary>
        /// <value>
        /// The page ids.
        /// </value>
        public System.Collections.Generic.List<string> PageIds { get; set; }
        public string Language { get; set; }
        public string LanguageNative { get; set; }
        public bool IsEnglish { get; set; }
        public string ThemeColorScheme { get; set; }
        public string ThemeBaseColorScheme { get; set; }
    }
}