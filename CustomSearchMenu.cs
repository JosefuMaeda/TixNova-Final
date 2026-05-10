using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq; // Required to search OpenForms

namespace TixNova__Final
{
    public class CustomSearchMenu : Form
    {
        private TextBox searchInput;

        public CustomSearchMenu()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.AllowTransparency = true;
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.Size = new Size(500, 340);

            InitializeSearchUI();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            EnableBlur(this.Handle);
        }

        #region Blur Logic API
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }

        internal enum AccentState
        {
            ACCENT_ENABLE_BLURBEHIND = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        private void EnableBlur(IntPtr hwnd)
        {
            var accent = new AccentPolicy { AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND };
            int accentStructSize = Marshal.SizeOf(accent);
            IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(hwnd, ref data);
            Marshal.FreeHGlobal(accentPtr);
        }
        #endregion

        private void InitializeSearchUI()
        {
            // --- The Search Bar ---
            searchInput = new TextBox
            {
                Text = "Search for Movies, Cinemas or People....",
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(22, 28, 40), // Matches the bottom of your gradient
                ForeColor = Color.LightGray,           // Brighter so it's visible
                Font = new Font("Segoe UI", 12f),
                Location = new Point(60, 55),          // Adjust this to center it in the capsule
                Width = 380
            };

            searchInput.Enter += (s, e) => { if (searchInput.Text.StartsWith("Search")) { searchInput.Text = ""; searchInput.ForeColor = Color.White; } };
            searchInput.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(searchInput.Text)) { searchInput.Text = "Search for Movies, Cinemas or People..."; searchInput.ForeColor = Color.Gray; } };

            this.Controls.Add(searchInput);
            searchInput.BringToFront();

            // --- Content Columns ---
            int col1X = 30;
            int col2X = 270;

            AddMenuLabel("Recent Searches", col1X, 110, 210, true);
            AddMenuLabel("Suggestions", col2X, 110, 210, true);

            string[] recent = { "Send Help", "Rated PG", "28 Years Later", "Rated PG-13" };
            for (int i = 0; i < recent.Length; i++)
                AddMenuLabel(recent[i], col1X, 155 + (i * 35), 210, false);

            string[] suggestions = { "Drama Movies", "Top Horror Movies", "Top Family Movies", "Top Animation Movies" };
            for (int i = 0; i < suggestions.Length; i++)
                AddMenuLabel(suggestions[i], col2X, 155 + (i * 35), 210, false);
        }

        private void AddMenuLabel(string text, int x, int y, int w, bool isHeader)
        {
            Label lbl = new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 30),
                Font = new Font("Segoe UI", isHeader ? 14f : 11.5f, isHeader ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHeader ? Color.FromArgb(103, 190, 217) : Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Cursor = isHeader ? Cursors.Default : Cursors.Hand
            };

            if (!isHeader)
            {
                lbl.MouseEnter += (s, e) => lbl.ForeColor = Color.FromArgb(0, 191, 255);
                lbl.MouseLeave += (s, e) => lbl.ForeColor = Color.White;

                // Attach the click event to any non-header label
                lbl.Click += MenuLabel_Click;
            }

            this.Controls.Add(lbl);
        }

        // ==========================================
        // REFACTORED: Click Event Logic Method
        // ==========================================
        private void MenuLabel_Click(object sender, EventArgs e)
        {
            if (!(sender is Label clickedLabel)) return;

            string selectedCategory = clickedLabel.Text;

            // Find the main dashboard instance
            var mainDashboard = Application.OpenForms.OfType<MainDashBoard>().FirstOrDefault();

            // Hide the search menu itself
            this.Hide();

            // Hide the dashboard safely
            mainDashboard?.Hide();

            switch (selectedCategory)
            {
                case "Top Horror Movies":
                    SearchTopHorror horrorForm = new SearchTopHorror();
                    horrorForm.FormClosed += (s, args) => mainDashboard?.Show();
                    horrorForm.Show();
                    break;

                case "Top Family Movies":
                    SearchTopFamily familyForm = new SearchTopFamily();
                    familyForm.FormClosed += (s, args) => mainDashboard?.Show();
                    familyForm.Show();
                    break;

                case "Top Animation Movies":
                    SearchAnime searchAnimeForm = new SearchAnime();
                    searchAnimeForm.FormClosed += (s, args) => mainDashboard?.Show();
                    searchAnimeForm.Show();
                    break;

                case "Drama Movies":
                    GenreDrama genreDramaForm = new GenreDrama();
                    genreDramaForm.FormClosed += (s, args) => mainDashboard?.Show();
                    genreDramaForm.Show();
                    break;

                case "Send Help":
                    SendHelpForm sendHelpForm = new SendHelpForm();
                    sendHelpForm.FormClosed += (s, args) => mainDashboard?.Show();
                    sendHelpForm.Show();
                    break;

                case "Rated PG":
                    RatedPGForm ratedPGForm = new RatedPGForm();
                    ratedPGForm.FormClosed += (s, args) => mainDashboard?.Show();
                    ratedPGForm.Show();
                    break;

                case "28 Years Later":
                    YearsLaterForm yearsLaterForm = new YearsLaterForm();
                    yearsLaterForm.FormClosed += (s, args) => mainDashboard?.Show();
                    yearsLaterForm.Show();
                    break;

                case "Rated PG-13":
                    RatedPG13 ratedPG13 = new RatedPG13();
                    ratedPG13.FormClosed += (s, args) => mainDashboard?.Show();
                    ratedPG13.Show();
                    break;

                default:
                    mainDashboard?.Show();
                    MessageBox.Show($"Coming soon: {selectedCategory} section!", "TixNova+ Search");
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Background Blur Tint
            using (GraphicsPath mainPath = CreateMenuPath(1, 1, Width - 2, Height - 2))
            {
                // Semi-transparent fill so the blur effect shows through
                using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(150, 10, 15, 25)))
                {
                    g.FillPath(bgBrush, mainPath);
                }

                using (Pen borderPen = new Pen(Color.FromArgb(103, 190, 217), 1.2f))
                {
                    g.DrawPath(borderPen, mainPath);
                }
            }

            // --- IMPROVED SEARCH BAR (GLASSY LOOK) ---
            Rectangle searchRect = new Rectangle(35, 42, 430, 45);
            using (GraphicsPath capsule = GetRoundedRect(searchRect, 22)) // 22 is half height for perfect circles
            {
                // Dark fill for the bar
                using (SolidBrush barBrush = new SolidBrush(Color.FromArgb(32, 38, 50)))
                    g.FillPath(barBrush, capsule);

                // Subtle inner glow/border for the search bar
                using (Pen barPen = new Pen(Color.FromArgb(60, 103, 190, 217), 1f))
                    g.DrawPath(barPen, capsule);
            }
        }

        private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private GraphicsPath CreateMenuPath(int x, int y, int width, int height)
        {
            int radius = 25;
            int arrowW = 24; int arrowH = 16;
            int arrowX = (width / 2) - (arrowW / 2);

            GraphicsPath path = new GraphicsPath();
            path.AddLine(radius, arrowH, arrowX, arrowH);
            path.AddLine(arrowX, arrowH, arrowX + (arrowW / 2), 0);
            path.AddLine(arrowX + (arrowW / 2), 0, arrowX + arrowW, arrowH);
            path.AddLine(arrowX + arrowW, arrowH, width - radius, arrowH);
            path.AddArc(width - radius, arrowH, radius, radius, 270, 90);
            path.AddLine(width, arrowH + radius, width, height - radius);
            path.AddArc(width - radius, height - radius, radius, radius, 0, 90);
            path.AddLine(width - radius, height, radius, height);
            path.AddArc(0, height - radius, radius, radius, 90, 90);
            path.AddLine(0, height - radius, 0, arrowH + radius);
            path.AddArc(0, arrowH, radius, radius, 180, 90);
            path.CloseFigure();
            return path;
        }
    }
}