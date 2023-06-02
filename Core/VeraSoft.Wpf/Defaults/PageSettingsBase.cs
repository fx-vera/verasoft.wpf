using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace VeraSoft.Wpf.Defaults
{
    public class PageSettingsBase : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageSettingsBase"/> class.
        /// </summary>
        public PageSettingsBase()
        {
            Left = double.NaN;
            Top = double.NaN;
            Guid = System.Guid.NewGuid().ToString("B");
        }

        void IDisposable.Dispose()
        {
        }
        #region Basic properties

        /// <summary>
        /// Gets or sets the left position.
        /// </summary>
        /// <value>
        /// The left position.
        /// </value>
        public double Left { get; set; }

        /// <summary>
        /// Gets or sets the top position.
        /// </summary>
        /// <value>
        /// The top position.
        /// </value>
        public double Top { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public Size Size { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        [Required(ErrorMessage = "Guid is required")]
        public string Guid { get; set; }

        #endregion Basic properties

    }
}
