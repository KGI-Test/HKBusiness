using System.Windows.Forms;

namespace Warp.HKBusiness.Client
{
    public partial class ReportViewer : Form
    {
        public ReportViewer(string reportData)
        {
            InitializeComponent();
            txtViewer.Text = reportData;
            txtViewer.SelectionStart = 0;
            txtViewer.SelectionLength = 0;
        }
    }
}
