using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// Observable collection that synchronizes changes back to an underlying model list.
    /// Provides observable collection functionality for any IList.
    /// </summary>
    /// <typeparam name="TModel">Type of models in the collection.</typeparam>
    public class WrappedObservableCollection<TModel> : ObservableCollection<TModel>

    {
        private IList<TModel> _models;
        private bool _synchDisabled;

        /// <summary>
        /// Constructor
        /// </summary>
        /// Determines whether the collection of ViewModels should be
        /// fetched from the model collection on construction
        /// </param>
        public WrappedObservableCollection()
        {
            // Register change handling for synchronization
            // from ViewModels to Models
            CollectionChanged += ViewModelCollectionChanged;

        }

        /// <summary>
        /// Loads models from the underlying list into the observable collection.
        /// </summary>
        /// <param name="models">The underlying model list to synchronize with.</param>
        public void LoadModels(IList<TModel> models)
        {
            _models = models;

            _synchDisabled = true;
            Clear();
            foreach (TModel model in models)
                Add(model);
            _synchDisabled = false;
        }

        /// <summary>
        /// CollectionChanged event of the ViewModelCollection
        /// </summary>
        public override sealed event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { base.CollectionChanged += value; }
            remove { base.CollectionChanged -= value; }
        }

        private void ViewModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_synchDisabled)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var m in e.NewItems.OfType<TModel>())
                        _models.Insert(e.NewStartingIndex, m);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var m in e.OldItems.OfType<TModel>())
                        _models.Remove(m);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    int curIndex = e.NewStartingIndex;
                    foreach (var m in e.NewItems.OfType<TModel>())
                    {
                        _models[curIndex] = m;
                        curIndex++;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();
                    //break;
                case NotifyCollectionChangedAction.Reset:
                    _models.Clear();
                    foreach (var m in e.NewItems.OfType<TModel>())
                        _models.Add(m);
                    break;
            }
        }

    }
}
