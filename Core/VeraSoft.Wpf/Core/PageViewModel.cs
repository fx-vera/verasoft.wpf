using VeraSoft.Wpf.Defaults;
using VeraSoft.Wpf.Managers;

namespace VeraSoft.Wpf.Core
{
    public abstract class PageViewModel : ViewModel, IViewModel // TODO FRAN: esto va en el sdk
    {
        protected PageViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the basic editor settings base.
        /// </summary>
        /// <value>
        /// The basic editor settings base.
        /// </value>
        public object EditorSettings { get; set; }

        /// <summary>
        /// Called when the page is added to a parent window
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            LoadEditorSettings();
            IsEditionMode = true;
        }

        /// <summary>
        /// Loads the editor sttings.
        /// </summary>
        protected void LoadEditorSettings()
        {
            if (EditorSettings != null && !(EditorSettings is PageSettingsBase))
                return;
        }
    }
}
