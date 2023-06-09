﻿using System;
using VeraSoft.Wpf.Core.Components;

namespace VeraSoft.Wpf.Events
{
    public class FavouriteEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is favourite button; otherwise, <c>false</c>.
        /// </value>
        public bool IsCommand { get; set; }

        public IPluginItem Item { get; set; }
    }
}
