using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace TixNova__Final
{
    public partial class TicketDetails : Form
    {
        // Fields
        private readonly BookingData _bookingData;
        private readonly List<string> _selectedSeats;

        // UI fields
        private Form _dropDownForm;
        private DateTime _menuLastClosedTime = DateTime.MinValue;
        private TixNovaMenuControl _menuContent;
        private CustomSearchMenu _searchMenu;
        private CustomMenu _sideMenu;

        private string movieTitle;
        private string cinemaName;
        private string showTime;
        private int ticketCount;
        private decimal grandTotal;

        // Constructor that accepts all data from BookingSeats
        public TicketDetails(BookingData bookingData = null, List<string> selectedSeats = null)
        {
            InitializeComponent();

            _bookingData = bookingData;
            _selectedSeats = selectedSeats ?? new List<string>();

            // Setup UI
            MakeRoundedGradientButton(MenuButton, Color.FromArgb(78, 199, 220), Color.FromArgb(7, 89, 179), 30);
            MakeRoundedGradientButton(SearchButton, Color.FromArgb(78, 199, 220), Color.FromArgb(7, 89, 179), 35);
            SetupAllLinkLabelsGlow();
            SetupMenu();

            this.DoubleBuffered = true;
            this.BackColor = Color.Black;

            SetupUI();
        }
        private void SetupMyBookingsUI(Panel mainContainer)
        {
            mainContainer.Controls.Clear();
            mainContainer.Size = new Size(600, 500); // Standard dashboard size

            // 1. Header Label
            Label lblTitle = new Label
            {
                Text = "MY BOOKINGS",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 255, 255),
                Location = new Point(20, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // 2. FlowLayoutPanel (The scrollable list)
            FlowLayoutPanel listPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 70),
                Size = new Size(560, 400),
                AutoScroll = true,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            // 3. Mock Data Loop (Replace this with your Database/List data)
            // Example: foreach(var booking in userBookings)
            for (int i = 0; i < 3; i++)
            {
                listPanel.Controls.Add(CreateBookingCard("Movie Title " + (i + 1), "May 10, 2024", "₱520.00"));
            }

            mainContainer.Controls.Add(lblTitle);
            mainContainer.Controls.Add(listPanel);
        }

        // Function to create individual booking rows
        private Panel CreateBookingCard(string title, string date, string price)
        {
            Panel card = new Panel
            {
                Size = new Size(530, 80),
                BackColor = Color.FromArgb(40, 255, 255, 255), // Slightly lighter glass
                Margin = new Padding(0, 0, 0, 10)
            };

            // Movie Title
            Label lblMovie = new Label
            {
                Text = title,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(15, 15),
                AutoSize = true
            };

            // Date and Price
            Label lblDetails = new Label
            {
                Text = $"{date} | {price}",
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 40),
                AutoSize = true
            };

            // "View" Button
            Button btnView = new Button
            {
                Text = "VIEW",
                Size = new Size(80, 30),
                Location = new Point(430, 25),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 180, 216)
            };

            // Styling the card with a small cyan left-border
            card.Paint += (s, e) => {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 255, 255)), 0, 0, 5, card.Height);
            };

            card.Controls.Add(lblMovie);
            card.Controls.Add(lblDetails);
            card.Controls.Add(btnView);

            return card;
        }
        private void SetupUI()
        {
            // Get data from BookingData or use defaults
            movieTitle = _bookingData?.MovieName ?? "Unknown Movie";
            cinemaName = _bookingData?.Cinema ?? "Unknown Cinema";
            showTime = _bookingData?.Schedule ?? "Unknown Time";
            string seats = _selectedSeats != null && _selectedSeats.Count > 0 ? string.Join(", ", _selectedSeats) : "None";
            ticketCount = _selectedSeats?.Count ?? 1;

            // Calculate prices
            decimal ticketTotal = (_bookingData?.TicketPrice ?? 250) * ticketCount;
            decimal snacksTotal = _bookingData?.TotalSnacksPrice ?? 0;
            decimal subtotal = ticketTotal + snacksTotal;
            decimal tax = subtotal * 0.12m;
            decimal serviceFee = 50.00m;
            grandTotal = subtotal + tax + serviceFee;

            int panelWidth = 550;
            int panelHeight = 720;

            // The Main Glass Panel
            Panel ticketPanel = new Panel
            {
                Size = new Size(panelWidth, panelHeight),
                Location = new Point((this.ClientSize.Width - panelWidth) / 2, (this.ClientSize.Height - panelHeight) / 2),
                BackColor = Color.FromArgb(20, 255, 255, 255)
            };

            // Custom Border and Header logic
            ticketPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = ticketPanel.ClientRectangle;
                int radius = 30;

                using (GraphicsPath path = GetRoundedPath(rect, radius))
                {
                    ticketPanel.Region = new Region(path);

                    // Draw Header Gradient (Blue section at the top)
                    Rectangle headerRect = new Rectangle(0, 0, rect.Width, 80);
                    using (LinearGradientBrush headerBrush = new LinearGradientBrush(headerRect, Color.FromArgb(0, 180, 216), Color.FromArgb(7, 89, 179), LinearGradientMode.Horizontal))
                    {
                        e.Graphics.FillRectangle(headerBrush, headerRect);
                    }

                    // Draw the Cyan border
                    using (Pen cyanPen = new Pen(Color.FromArgb(0, 255, 255), 3))
                    {
                        e.Graphics.DrawPath(cyanPen, path);
                    }
                }
            };

            // Add Labels for Title, Date, etc.
            AddDetailLabel(ticketPanel, "TICKET DETAILS", new Point(25, 25), true, 18);
            AddDetailLabel(ticketPanel, $"Title : {movieTitle}", new Point(35, 110));
            AddDetailLabel(ticketPanel, $"Cinema : {cinemaName}", new Point(35, 150));
            AddDetailLabel(ticketPanel, $"Time : {showTime}", new Point(35, 190));
            AddDetailLabel(ticketPanel, $"Seats : {seats}", new Point(35, 230));

            // Show snacks if any
            if (_bookingData?.Snacks != null && _bookingData.Snacks.Count > 0)
            {
                string snacksText = string.Join(", ", _bookingData.Snacks.Select(s => $"{s.Name} x{s.Quantity}"));
                AddDetailLabel(ticketPanel, $"Snacks : {snacksText}", new Point(35, 270));
                AddDetailLabel(ticketPanel, $"Snacks Total : ₱{snacksTotal:N2}", new Point(35, 310));
                // Number of tickets (adjust position if snacks exist)
                AddDetailLabel(ticketPanel, $"Tickets : {ticketCount}", new Point(35, 350));

                // Separator Line after snacks
                Panel line = new Panel { Size = new Size(panelWidth - 70, 2), Location = new Point(35, 390), BackColor = Color.FromArgb(0, 180, 216) };
                ticketPanel.Controls.Add(line);

                // Price details
                AddDetailLabel(ticketPanel, $"Ticket Total : ₱{ticketTotal:N2}", new Point(35, 430));
                AddDetailLabel(ticketPanel, $"Subtotal : ₱{subtotal:N2}", new Point(35, 465));
                AddDetailLabel(ticketPanel, $"Tax (12% VAT) : ₱{tax:N2}", new Point(35, 500));
                AddDetailLabel(ticketPanel, $"Service Fee : ₱{serviceFee:N2}", new Point(35, 535));

                // Second separator
                Panel line2 = new Panel { Size = new Size(panelWidth - 70, 1), Location = new Point(35, 570), BackColor = Color.FromArgb(100, 255, 255, 255) };
                ticketPanel.Controls.Add(line2);

                AddDetailLabel(ticketPanel, $"TOTAL AMOUNT : ₱{grandTotal:N2}", new Point(35, 600), true, 18);
            }
            else
            {
                // Number of tickets (no snacks)
                AddDetailLabel(ticketPanel, $"Tickets : {ticketCount}", new Point(35, 310));

                // Separator Line
                Panel line = new Panel { Size = new Size(panelWidth - 70, 2), Location = new Point(35, 360), BackColor = Color.FromArgb(0, 180, 216) };
                ticketPanel.Controls.Add(line);

                // Price details
                AddDetailLabel(ticketPanel, $"Ticket Total : ₱{ticketTotal:N2}", new Point(35, 400));
                AddDetailLabel(ticketPanel, $"Subtotal : ₱{subtotal:N2}", new Point(35, 435));
                AddDetailLabel(ticketPanel, $"Tax (12% VAT) : ₱{tax:N2}", new Point(35, 470));
                AddDetailLabel(ticketPanel, $"Service Fee : ₱{serviceFee:N2}", new Point(35, 505));

                // Second separator
                Panel line2 = new Panel { Size = new Size(panelWidth - 70, 1), Location = new Point(35, 540), BackColor = Color.FromArgb(100, 255, 255, 255) };
                ticketPanel.Controls.Add(line2);

                AddDetailLabel(ticketPanel, $"TOTAL AMOUNT : ₱{grandTotal:N2}", new Point(35, 575), true, 18);
            }

            // Add buttons at the bottom - PASS grandTotal as parameter
            AddBottomButtons(ticketPanel, panelWidth, grandTotal);

            this.Controls.Add(ticketPanel);
        }

        private void AddBottomButtons(Panel parent, int panelWidth, decimal grandTotal)
        {
            int btnWidth = 150;
            int btnHeight = 45;
            int btnY = parent.Height - 70;
            int spacing = 20;

            int totalButtonsWidth = (btnWidth * 2) + spacing;
            int startX = (panelWidth - totalButtonsWidth) / 2;

            // Confirm Payment button
            Button btnConfirm = new Button
            {
                Text = "CONFIRM PAYMENT",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point(startX, btnY),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 180, 216),
                FlatStyle = FlatStyle.Flat
            };

            // Cancel button
            Button btnCancel = new Button
            {
                Text = "CANCEL",
                Size = new Size(btnWidth, btnHeight),
                Location = new Point(startX + btnWidth + spacing, btnY),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(70, 70, 70),
                FlatStyle = FlatStyle.Flat
            };

            btnConfirm.Click += (s, e) =>
            {
                BookingData confirmedBooking = new BookingData
                {
                    MovieName = movieTitle,
                    Cinema = cinemaName,
                    Schedule = showTime,
                    TicketQuantity = ticketCount,
                    TicketPrice = grandTotal
                };

                // SAVE TO THE STATIC LIST (This makes it show up in MyBookings)
                BookingData.AllBookings.Add(confirmedBooking);
                // 1. CLEAR existing price details
                parent.Controls.Clear();

                // 2. RESIZE THE CONTAINER (Making it smaller and positioning it higher)
                // Old size: (550, 720). New Size: (500, 350)
                parent.Size = new Size(500, 350);

                // Position it under the header area (adjust Y offset as needed for your screen)
                parent.Location = new Point((this.ClientSize.Width - parent.Width) / 2, 220);

                // 3. Update the Title in the HEADER BAR (Instead of inside the box)
                // Since we cleared the panel, we only need to redraw the header text on the paint event.
                // However, to make it easier to maintain, we will just add a label to the TOP.
                Label lblHeaderTitle = new Label
                {
                    Text = "SUCCESS: BOOKING CONFIRMED",
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.TopCenter,
                    AutoSize = false,
                    Size = new Size(parent.Width, 50),
                    Location = new Point(0, 25), // Positioned inside the blue gradient header
                    BackColor = Color.Transparent // Let the gradient show through
                };

                // 4. Put "Thank You" Message INSIDE the box
                Label lblThankYou = new Label
                {
                    Text = "Thank you for using TixNova+",
                    Font = new Font("Segoe UI", 18, FontStyle.Bold), // A bit smaller than the main confirm title
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false,
                    Size = new Size(parent.Width, 60),
                    Location = new Point(0, 120), // Mid-top of the new smaller box
                    BackColor = Color.Transparent
                };

                Label lblInfo = new Label
                {
                    Text = "Digital ticket details are in 'My Bookings'.",
                    Font = new Font("Segoe UI", 15),
                    ForeColor = Color.FromArgb(180, 180, 180), // Slightly greyed out subtext
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false,
                    Size = new Size(parent.Width, 40),
                    Location = new Point(0, 170),
                    BackColor = Color.Transparent
                };

                // 5. Adjust navigation buttons for the new size
                // We are reusing the existing variables from AddBottomButtons scope
                btnWidth = 170;
                btnHeight = 60;
                int centerX = (parent.Width - (btnWidth * 2 + 20)) / 2;
                btnY = parent.Height - 80; // Position relative to the new bottom

                Button btnBookings = new Button
                {
                    Text = "MY BOOKINGS",
                    Size = new Size(btnWidth, btnHeight),
                    Location = new Point(centerX, btnY),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat
                };

                Button btnHome = new Button
                {
                    Text = "HOME",
                    Size = new Size(btnWidth, btnHeight),
                    Location = new Point(centerX + btnWidth + 20, btnY),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat
                };

                // Style the new buttons
                MakeRoundedGradientButton(btnBookings, Color.FromArgb(60, 60, 60), Color.FromArgb(30, 30, 30), 20);
                MakeRoundedGradientButton(btnHome, Color.FromArgb(0, 180, 216), Color.FromArgb(7, 89, 179), 20);

                // Button Actions
                btnBookings.Click += (sender, args) => {
                    MyBookings bookingsForm = new MyBookings();
                    bookingsForm.Show();
                    this.Hide(); // Hides the current TicketDetails form
                };

                btnHome.Click += (sender, args) => {
                    MainDashBoard mainForm = new MainDashBoard();
                    mainForm.Show();
                    this.Hide();
                };

                // 6. Assemble the new view
                parent.Controls.Add(lblHeaderTitle); // Title moves to the top header
                parent.Controls.Add(lblThankYou);    // Thank you goes inside the box
                parent.Controls.Add(lblInfo);
                parent.Controls.Add(btnBookings);
                parent.Controls.Add(btnHome);

                parent.Invalidate(); // Crucial to redraw the cyan border for the new size
            };

            btnCancel.Click += (s, e) =>
            {
                DialogResult result = MessageBox.Show("Are you sure you want to cancel?", "Cancel Booking",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Go back to seat selection
                    BookingSeats seatPicker = new BookingSeats();
                    seatPicker.Show();
                    this.Hide();
                }
            };

            // Apply gradient styling
            MakeRoundedGradientButton(btnConfirm, Color.FromArgb(0, 180, 216), Color.FromArgb(7, 89, 179), 25);
            MakeRoundedGradientButton(btnCancel, Color.FromArgb(80, 80, 85), Color.FromArgb(50, 50, 55), 25);

            parent.Controls.Add(btnConfirm);
            parent.Controls.Add(btnCancel);
        }

        private void AddDetailLabel(Panel parent, string text, Point loc, bool isBold = false, int size = 14)
        {
            Label lbl = new Label
            {
                Text = text,
                Location = loc,
                AutoSize = true,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", size, isBold ? FontStyle.Bold : FontStyle.Regular)
            };
            parent.Controls.Add(lbl);
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        // Add all the styling and menu methods from your original code...
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

        private void SetupMenu()
        {
            _menuContent = new TixNovaMenuControl { Location = new Point(0, 0) };
            _dropDownForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                Size = _menuContent.Size,
                BackColor = Color.Black,
                Region = new Region(_menuContent.GetRegionPath())
            };
            _dropDownForm.Controls.Add(_menuContent);
            _dropDownForm.HandleCreated += (s, e) => EnableBlur(_dropDownForm.Handle);
            _dropDownForm.Deactivate += (s, e) =>
            {
                _dropDownForm.Hide();
                _menuLastClosedTime = DateTime.Now;
            };
        }

        private void SetupAllLinkLabelsGlow()
        {
            foreach (Control control in this.Controls)
            {
                if (control is LinkLabel linkLabel)
                {
                    var originalColor = linkLabel.LinkColor;
                    Timer pulseTimer = null;
                    int pulseValue = 0;
                    bool increasing = true;

                    linkLabel.MouseEnter += (sender, e) =>
                    {
                        LinkLabel lbl = sender as LinkLabel;
                        pulseTimer = new Timer { Interval = 50 };
                        pulseTimer.Tick += (ts, te) =>
                        {
                            if (increasing)
                            {
                                pulseValue += 15;
                                if (pulseValue >= 255) increasing = false;
                            }
                            else
                            {
                                pulseValue -= 15;
                                if (pulseValue <= 100) increasing = true;
                            }
                            lbl.LinkColor = Color.FromArgb(255, 0, pulseValue, 255);
                        };
                        pulseTimer.Start();
                    };

                    linkLabel.MouseLeave += (sender, e) =>
                    {
                        LinkLabel lbl = sender as LinkLabel;
                        pulseTimer?.Stop();
                        pulseTimer?.Dispose();
                        lbl.LinkColor = originalColor;
                    };
                }
            }
        }

        private void MakeRoundedGradientButton(Button btn, Color startColor, Color endColor, int radius = 20)
        {
            btn.FlatStyle = FlatStyle.Popup;
            btn.FlatAppearance.BorderSize = 0;
            btn.Tag = new GradientInfo { StartColor = startColor, EndColor = endColor };

            var originalSize = btn.Size;
            var originalLocation = btn.Location;

            btn.MouseEnter += (sender, e) =>
            {
                btn.Size = new Size(btn.Width + 5, btn.Height + 5);
                btn.Location = new Point(btn.Location.X - 2, btn.Location.Y - 2);
                btn.Cursor = Cursors.Hand;
            };

            btn.MouseLeave += (sender, e) =>
            {
                btn.Size = originalSize;
                btn.Location = originalLocation;
                btn.Cursor = Cursors.Default;
            };

            btn.Paint += (sender, e) =>
            {
                Button b = sender as Button;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (var path = new GraphicsPath())
                {
                    Rectangle rect = new Rectangle(0, 0, b.Width - 1, b.Height - 1);
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
                    path.CloseFigure();
                    b.Region = new Region(path);

                    GradientInfo gradient = (GradientInfo)b.Tag;
                    using (var brush = new LinearGradientBrush(rect, gradient.StartColor, gradient.EndColor, LinearGradientMode.Vertical))
                        e.Graphics.FillPath(brush, path);

                    TextRenderer.DrawText(e.Graphics, b.Text, b.Font, rect, b.ForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            };
            btn.Resize += (sender, e) => btn.Invalidate();
        }

        private class GradientInfo
        {
            public Color StartColor { get; set; }
            public Color EndColor { get; set; }
        }

        // Navigation Handlers
        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MainDashBoard mainForm = new MainDashBoard();
            mainForm.Show();
            this.Hide();
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MoviesForm moviesForm = new MoviesForm();
            moviesForm.Show();
            this.Hide();
        }

        private void LinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CinemasForm cinemasForm = new CinemasForm();
            cinemasForm.Show();
            this.Hide();
        }

        private void LinkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShopForm shopForm = new ShopForm();
            shopForm.Show();
            this.Hide();
        }

        private void LinkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if ((DateTime.Now - _menuLastClosedTime).TotalMilliseconds < 100) return;

            if (_dropDownForm.Visible) _dropDownForm.Hide();
            else
            {
                int xOffset = (linkLabel5.Width - _menuContent.Width) / 2;
                Point screenLocation = linkLabel5.PointToScreen(new Point(xOffset, linkLabel5.Height + 5));
                _dropDownForm.Location = screenLocation;
                _dropDownForm.Show();
                _dropDownForm.BringToFront();
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (_searchMenu == null || _searchMenu.IsDisposed)
            {
                _searchMenu = new CustomSearchMenu();
                Point screenPos = SearchButton.PointToScreen(new Point(0, SearchButton.Height));
                _searchMenu.Location = new Point(screenPos.X - (_searchMenu.Width / 2) + (SearchButton.Width / 2), screenPos.Y + 10);
                _searchMenu.Show();
            }
            else
            {
                if (_searchMenu.Visible) _searchMenu.Hide();
                else _searchMenu.Show();
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            if (_sideMenu == null || _sideMenu.IsDisposed)
            {
                _sideMenu = new CustomMenu();
                Point screenPos = this.PointToScreen(new Point(this.Width - _sideMenu.Width - 20, 50));
                _sideMenu.Location = screenPos;
                _sideMenu.Show();
            }
            else
            {
                _sideMenu.Visible = !_sideMenu.Visible;
            }
        }
    }
}