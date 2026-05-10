using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace TixNova__Final // Make sure this matches your project's namespace
{
    public class FadingImageButton : PictureBox
    {
        private float _opacity = 0.0f; // Starts fully transparent
        private readonly Timer _fadeTimer;
        private bool _isHovered = false;

        public FadingImageButton()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.Transparent;
            this.SizeMode = PictureBoxSizeMode.Zoom;
            this.Cursor = Cursors.Hand;

            _fadeTimer = new Timer { Interval = 15 }; // Speed of the animation
            _fadeTimer.Tick += FadeTimer_Tick;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            _fadeTimer.Start();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            _fadeTimer.Start();
            base.OnMouseLeave(e);
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            // Increase opacity on hover
            if (_isHovered && _opacity < 1.0f)
            {
                _opacity += 0.08f; // Adjust this number to change fade-in speed
                if (_opacity > 1.0f) _opacity = 1.0f;
            }
            // Decrease opacity when mouse leaves
            else if (!_isHovered && _opacity > 0.0f)
            {
                _opacity -= 0.08f; // Adjust this number to change fade-out speed
                if (_opacity < 0.0f) _opacity = 0.0f;
            }
            else
            {
                _fadeTimer.Stop();
            }

            this.Invalidate(); // Forces the control to redraw
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // If there's an image and it's somewhat visible, draw it with opacity
            if (this.Image != null && _opacity > 0)
            {
                ColorMatrix matrix = new ColorMatrix { Matrix33 = _opacity }; // Manipulate the Alpha channel

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                pe.Graphics.DrawImage(this.Image,
                    new Rectangle(0, 0, this.Width, this.Height),
                    0, 0, this.Image.Width, this.Image.Height,
                    GraphicsUnit.Pixel, attributes);
            }
        }
    }
}