using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TixNova__Final
{
    public partial class SearchTopFamily : Form
    {
        public SearchTopFamily()
        {
            InitializeComponent();
        }

        private void fadingImageButton1_Click(object sender, EventArgs e)
        {
            // 1. Set the exact same scroll amount as the Right button
            int scrollAmount = 840;

            // 2. Move the track to the right (reveals movies on the left)
            movieTrackPanel.Left += scrollAmount;

            // 3. Stop perfectly at the beginning of the list
            if (movieTrackPanel.Left > 0)
            {
                movieTrackPanel.Left = 0;
            }
        }

        private void fadingImageButton2_Click(object sender, EventArgs e)
        {
            // 1. Set the scroll amount for a "Page"
            // Change 840 if your pictures in this form are a different size!
            int scrollAmount = 840;

            // 2. Move the track to the left (reveals movies on the right)
            movieTrackPanel.Left -= scrollAmount;

            // 3. Stop perfectly at the end of the list
            int maxScrollLimit = viewportPanel.Width - movieTrackPanel.Width;
            if (movieTrackPanel.Left < maxScrollLimit)
            {
                movieTrackPanel.Left = maxScrollLimit;
            }
        }

        private void roundedPictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void blueGradientLabel1_Click(object sender, EventArgs e)
        {

        }

        private void gradientLabel2_Click(object sender, EventArgs e)
        {

        }

        private void roundedPictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void roundedPictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void roundedPictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void roundedPictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void roundedPictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void gradientLabel1_Click(object sender, EventArgs e)
        {

        }

        private void roundedPictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void roundedPictureBox8_Click(object sender, EventArgs e)
        {

        }

        private void roundedPictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void roundedPictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void roundedPictureBox11_Click(object sender, EventArgs e)
        {

        }

        private void movieTrackPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void viewportPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
    }
}
