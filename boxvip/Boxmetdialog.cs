using System;
using System.Drawing;
using System.Windows.Forms;

namespace BoxMetPlugin
{
    // =====================================================================
    //  BOXMET INPUT DIALOG — Giao diện nhập thông số trong AutoCAD
    // =====================================================================
    public class BoxMetDialog : Form
    {
        // Input fields
        private TextBox txtL, txtW, txtT, txtNobi;
        private ComboBox cmbMat;
        private RadioButton rbSame, rbDiff;
        private TextBox txtHAll, txtH1, txtH2, txtH3, txtH4;
        private Panel pnlSame, pnlDiff;
        private Label lblInfo;
        private Label lblNobiRef;
        private Button btnOK, btnCancel;

        public BoxMetDialog()
        {
            InitializeUI();
            UpdateNobiRef();
        }

        void InitializeUI()
        {
            // ── Form settings ──
            Text = "BOXMET — Flat Pattern Generator";
            Size = new Size(420, 580);
            MinimumSize = new Size(400, 540);
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 35, 45);
            ForeColor = Color.FromArgb(220, 225, 235);
            Font = new Font("Segoe UI", 9f);

            var scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(16, 12, 16, 8),
                BackColor = Color.Transparent
            };

            var flow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Width = 370,
                BackColor = Color.Transparent
            };

            // ── HEADER ──
            var header = new Label
            {
                Text = "BOXMET  ⬡  Sheet Metal Unfold",
                Font = new Font("Consolas", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 220, 140),
                AutoSize = false,
                Height = 36,
                Width = 370,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 8)
            };
            flow.Controls.Add(header);

            // ── SECTION: Kích thước ──
            flow.Controls.Add(MakeSection("▸  KÍCH THƯỚC HỘP"));
            flow.Controls.Add(MakeRow("Chiều Dài  L  (mm)", out txtL, "200"));
            flow.Controls.Add(MakeRow("Chiều Rộng W  (mm)", out txtW, "150"));
            flow.Controls.Add(MakeRow("Độ Dày     T  (mm)", out txtT, "1.5"));
            flow.Controls.Add(MakeRow("NOBI Offset    (mm)", out txtNobi, "0"));
            lblNobiRef = new Label
            {
                Text = "ⓘ Nobi tham khảo: —",
                ForeColor = Color.FromArgb(120, 200, 160),
                Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                AutoSize = true,
                MaximumSize = new Size(370, 0),
                Margin = new Padding(0, 0, 0, 8)
            };
            flow.Controls.Add(lblNobiRef);
            txtL.TextChanged += (s, e) => UpdateNobiRef();
            txtW.TextChanged += (s, e) => UpdateNobiRef();
            txtT.TextChanged += (s, e) => UpdateNobiRef();

            // ── SECTION: Vật liệu ──
            flow.Controls.Add(MakeSection("▸  VẬT LIỆU"));
            cmbMat = new ComboBox
            {
                Width = 370,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(22, 27, 40),
                ForeColor = Color.FromArgb(220, 225, 235),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 9.5f),
                Margin = new Padding(0, 2, 0, 8)
            };
            cmbMat.Items.AddRange(new[] {
                "SS  — Stainless Steel (Lead 3mm)",
                "SUS — High-grade SUS  (Lead 6mm)",
                "AL  — Aluminum        (Lead 6mm)",
                "MS  — Mild Steel      (Lead 6mm)",
            });
            cmbMat.SelectedIndex = 0;
            cmbMat.SelectedIndexChanged += (s, e) => { UpdateInfo(); UpdateNobiRef(); };
            flow.Controls.Add(cmbMat);

            // Info label
            lblInfo = new Label
            {
                Text = "ℹ  Lead-in: 3.0 mm  |  Slit: T + 0.2",
                ForeColor = Color.FromArgb(90, 180, 120),
                Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 8)
            };
            flow.Controls.Add(lblInfo);

            // ── SECTION: Chiều cao ──
            flow.Controls.Add(MakeSection("▸  CHIỀU CAO CÁNH"));

            var radioPanel = new Panel { Width = 370, Height = 28, Margin = new Padding(0, 0, 0, 4) };
            rbSame = new RadioButton
            {
                Text = "Cùng chiều cao",
                Checked = true,
                Location = new Point(0, 4),
                AutoSize = true,
                ForeColor = Color.FromArgb(200, 220, 200)
            };
            rbDiff = new RadioButton
            {
                Text = "Mỗi cạnh riêng",
                Checked = false,
                Location = new Point(140, 4),
                AutoSize = true,
                ForeColor = Color.FromArgb(200, 220, 200)
            };
            rbSame.CheckedChanged += (s, e) => ToggleHeightMode();
            radioPanel.Controls.AddRange(new Control[] { rbSame, rbDiff });
            flow.Controls.Add(radioPanel);

            // Single height
            pnlSame = new Panel { Width = 370, AutoSize = true };
            var fSame = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Width = 370 };
            fSame.Controls.Add(MakeRow("Chiều cao CHUNG (mm)", out txtHAll, "30"));
            pnlSame.Controls.Add(fSame);
            flow.Controls.Add(pnlSame);

            // Diff heights
            pnlDiff = new Panel { Width = 370, AutoSize = true, Visible = false };
            var fDiff = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Width = 370 };
            fDiff.Controls.Add(MakeRow("H  Dưới  — Bottom (mm)", out txtH1, "30"));
            fDiff.Controls.Add(MakeRow("H  Phải  — Right  (mm)", out txtH2, "30"));
            fDiff.Controls.Add(MakeRow("H  Trên  — Top    (mm)", out txtH3, "30"));
            fDiff.Controls.Add(MakeRow("H  Trái  — Left   (mm)", out txtH4, "30"));
            pnlDiff.Controls.Add(fDiff);
            flow.Controls.Add(pnlDiff);

            scroll.Controls.Add(flow);
            Controls.Add(scroll);

            // ── BOTTOM BUTTONS ──
            var btnPanel = new Panel
            {
                Height = 52,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(20, 24, 33),
                Padding = new Padding(16, 8, 16, 8)
            };

            btnOK = new Button
            {
                Text = "✔  VẼ VÀO AUTOCAD",
                Width = 200,
                Height = 36,
                Location = new Point(16, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(50, 170, 90),
                ForeColor = Color.FromArgb(10, 20, 10),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.None
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button
            {
                Text = "✖  Hủy",
                Width = 100,
                Height = 36,
                Location = new Point(226, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(80, 40, 40),
                ForeColor = Color.FromArgb(220, 180, 180),
                Font = new Font("Segoe UI", 9.5f),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            btnPanel.Controls.AddRange(new Control[] { btnOK, btnCancel });
            Controls.Add(btnPanel);

            AcceptButton = btnOK;
            CancelButton = btnCancel;
        }

        // ── UI Helpers ──
        Panel MakeSection(string title)
        {
            var p = new Panel
            {
                Width = 370,
                Height = 24,
                Margin = new Padding(0, 10, 0, 4),
                BackColor = Color.FromArgb(20, 90, 160, 100)
            };
            var lbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 200, 130),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(4, 0, 0, 0)
            };
            p.Controls.Add(lbl);
            return p;
        }

        Panel MakeRow(string label, out TextBox tb, string def)
        {
            var p = new Panel { Width = 370, Height = 46, Margin = new Padding(0, 2, 0, 2) };
            var lbl = new Label
            {
                Text = label,
                Top = 2,
                Left = 0,
                AutoSize = true,
                ForeColor = Color.FromArgb(150, 170, 160),
                Font = new Font("Segoe UI", 8.5f)
            };
            tb = new TextBox
            {
                Text = def,
                Width = 370,
                Top = 20,
                Left = 0,
                BackColor = Color.FromArgb(22, 28, 40),
                ForeColor = Color.FromArgb(220, 230, 210),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 10.5f)
            };
            p.Controls.AddRange(new Control[] { lbl, tb });
            return p;
        }

        void ToggleHeightMode()
        {
            pnlSame.Visible = rbSame.Checked;
            pnlDiff.Visible = rbDiff.Checked;
            if (rbDiff.Checked)
                txtH1.Text = txtH2.Text = txtH3.Text = txtH4.Text = txtHAll.Text;
        }

        void UpdateNobiRef()
        {
            if (lblNobiRef == null) return;
            if (!double.TryParse(txtT.Text, out double t) || t <= 0)
            {
                lblNobiRef.Text = "ⓘ Nhập độ dày T để tra Nobi tham khảo.";
                return;
            }
            double.TryParse(txtL.Text, out double l);
            double.TryParse(txtW.Text, out double w);
            double bend = Math.Max(l, w);
            string mat = cmbMat.Text.Split('—')[0].Trim().Split(' ')[0];
            var r = NobiLookup.Lookup(mat, t, bend);
            if (r != null && r.Found)
            {
                txtNobi.Text = r.Nobi.ToString("0.##");
                lblNobiRef.Text = $"ⓘ Tham khảo: Nobi = {r.Nobi}  ({r.V}, L uốn {r.RangeText}){r.ConfirmNote}";
            }
            else
            {
                lblNobiRef.Text = $"ⓘ {(r?.Message ?? "Không tra được Nobi")} — giữ số tay.";
            }
        }

        void UpdateInfo()
        {
            bool isSS = cmbMat.SelectedIndex == 0;
            double lead = isSS ? 3.0 : 6.0;
            lblInfo.Text = $"ℹ  Lead-in: {lead:F1} mm  |  Slit: T + 0.2";
        }

        void BtnOK_Click(object sender, EventArgs e)
        {
            if (!Validate()) return;
            DialogResult = DialogResult.OK;
            Close();
        }

        new bool Validate()
        {
            var fields = new (TextBox tb, string name)[] {
                (txtL,"Chiều Dài"),(txtW,"Chiều Rộng"),(txtT,"Độ Dày"),(txtNobi,"NOBI"),
            };
            foreach (var (tb, name) in fields)
            {
                if (!double.TryParse(tb.Text, out double v) || v <= 0)
                {
                    MessageBox.Show($"⚠  Giá trị '{name}' không hợp lệ!\nNhập số dương.",
                        "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tb.Focus(); tb.SelectAll();
                    return false;
                }
            }
            double h = rbSame.Checked
                ? (double.TryParse(txtHAll.Text, out double hv) ? hv : -1)
                : (double.TryParse(txtH1.Text, out double h1) ? h1 : -1);
            if (h <= 0)
            {
                MessageBox.Show("⚠  Chiều cao không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        public BoxParams GetParams()
        {
            double hAll = double.TryParse(txtHAll.Text, out double v) ? v : 30;
            string mat = cmbMat.Text.Split('—')[0].Trim().Split(' ')[0];

            return new BoxParams
            {
                Length = double.Parse(txtL.Text),
                Width = double.Parse(txtW.Text),
                Thickness = double.Parse(txtT.Text),
                Material = mat,
                Nobi = double.TryParse(txtNobi.Text, out double nv) ? nv : 0,
                H_Bottom = rbSame.Checked ? hAll : double.Parse(txtH1.Text),
                H_Right = rbSame.Checked ? hAll : double.Parse(txtH2.Text),
                H_Top = rbSame.Checked ? hAll : double.Parse(txtH3.Text),
                H_Left = rbSame.Checked ? hAll : double.Parse(txtH4.Text),
            };
        }
    }
}