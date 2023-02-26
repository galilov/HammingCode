using System;
using System.Windows.Forms;

namespace HammingCodeDemo
{
    public partial class MainForm : Form
    {
        private readonly DataPresenter _dataPresenter;

        public MainForm()
        {
            InitializeComponent();
            _dataPresenter = new DataPresenter();
        }

        private void PictBox_Paint(object sender, PaintEventArgs e)
        {
            _dataPresenter.DisplayTo(e.Graphics, PictBox.Width, PictBox.Height);
        }

        private void PictBox_Resize(object sender, EventArgs e)
        {
            PictBox.Invalidate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _dataPresenter.SetBlockLength(16);
            PictBox.Invalidate();
        }

        private void PrevBtn_Click(object sender, EventArgs e)
        {
            _dataPresenter.PrevStep();
            PictBox.Invalidate();
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            _dataPresenter.NextStep();
            PictBox.Invalidate();
        }

        private void PictBox_MouseClick(object sender, MouseEventArgs e)
        {
            _dataPresenter.Click(e.Location);
            PictBox.Invalidate();
        }
    }
}