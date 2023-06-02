using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace VeraSoft.Wpf.Extensions
{
    /// <summary>
    /// Represents a collection that is observable, and allows adding or removing a 
    /// range of items, or replacing the current ones with another set
    /// </summary>
    /// <typeparam name = "T">The type of elements contained in the collection.</typeparam>
    public interface IObservableCollection<T> : IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>
        ///   Adds a collection of items, notifying only one global change.
        /// </summary>
        /// <param name = "items">The items.</param>
        /// <param name="allowDuplicates">Optional parameter. True if duplicates are allowed. Otherwise, false</param>
        void AddRange(IEnumerable<T> items, bool allowDuplicates = true);

        /// <summary>
        ///   Removes a collection of items, notifying only one global change.
        /// </summary>
        /// <param name = "items">The items.</param>
        void RemoveRange(IEnumerable<T> items);

        /// <summary>
        /// Clears the current collection and replaces it with another, notifying only one global change.
        /// </summary>
        /// <param name="items"></param>
        void ReplaceRange(IEnumerable<T> items);
    }
}
