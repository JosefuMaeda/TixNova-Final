using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TixNova__Final // Make sure this matches your project's namespace!
{
    public class SubheadingGradientLabel : Label
    {
        public SubheadingGradientLabel()
        {
            // Reduces flickering during redrawing
            this.DoubleBuffered = true;
            // Set background to transparent so it blends with your dark UI
            this.BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Set high-quality text rendering for smooth edges
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Don't try to draw if there's no text or the control has no size
            if (string.IsNullOrEmpty(this.Text) || this.ClientRectangle.Width <= 0 || this.ClientRectangle.Height <= 0)
                return;

            // Measure the text height so the vertical gradient fits perfectly from the top to the bottom of the letters
            SizeF textSize = e.Graphics.MeasureString(this.Text, this.Font);
            Rectangle gradientRect = new Rectangle(0, 0, this.ClientRectangle.Width, (int)textSize.Height);

            // Prevent crashes if height calculation is too small
            if (gradientRect.Height <= 0) gradientRect.Height = 1;

            // Define the colors matching your image: White top, bright cyan bottom
            Color topColor = Color.White;
            Color bottomColor = Color.FromArgb(45, 200, 255);

            // Create a vertical gradient brush
            using (LinearGradientBrush brush = new LinearGradientBrush(gradientRect, topColor, bottomColor, LinearGradientMode.Vertical))
            {
                // Set up string formatting to respect the label's alignment properties
                using (StringFormat sf = new StringFormat())
                {
                    // Map the WinForms ContentAlignment to StringAlignment
                    sf.Alignment = GetHorizontalAlignment(this.TextAlign);
                    sf.LineAlignment = GetVerticalAlignment(this.TextAlign);

                    // Draw the text using our gradient brush
                    e.Graphics.DrawString(this.Text, this.Font, brush, this.ClientRectangle, sf);
                }
            }
        }

        // Helper method to convert label's TextAlign to StringAlignment (Horizontal)
        private StringAlignment GetHorizontalAlignment(ContentAlignment align)
        {
            if (align == ContentAlignment.TopLeft || align == ContentAlignment.MiddleLeft || align == ContentAlignment.BottomLeft)
                return StringAlignment.Near;
            if (align == ContentAlignment.TopCenter || align == ContentAlignment.MiddleCenter || align == ContentAlignment.BottomCenter)
                return StringAlignment.Center;
            return StringAlignment.Far;
        }

        // Helper method to convert label's TextAlign to StringAlignment (Vertical)
        private StringAlignment GetVerticalAlignment(ContentAlignment align)
        {
            if (align == ContentAlignment.TopLeft || align == ContentAlignment.TopCenter || align == ContentAlignment.TopRight)
                return StringAlignment.Near;
            if (align == ContentAlignment.MiddleLeft || align == ContentAlignment.MiddleCenter || align == ContentAlignment.MiddleRight)
                return StringAlignment.Center;
            return StringAlignment.Far;
        }
    }
}