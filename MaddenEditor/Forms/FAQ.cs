using System.Windows.Forms;
using MaddenEditor.Core;

namespace MaddenEditor.Forms
{
    public partial class FAQ : UserControl, IEditorForm
    {
        public EditorModel model = null;
        public bool isInitializing = false;

        #region IEditorForm Members

        public EditorModel Model
        {
            set { model = value; }
        }

        public void InitialiseUI()
        {
            isInitializing = true;



            isInitializing = false;
        }

        public void CleanUI()
        {

        }

        #endregion

        public FAQ()
        {
            InitializeComponent();
        }


    }
}
