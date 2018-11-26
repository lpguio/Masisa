namespace EnroladorStandAlone
{
    partial class LoadingDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingDialog));
            this.pgbTotal = new System.Windows.Forms.ProgressBar();
            this.pgbActual = new System.Windows.Forms.ProgressBar();
            this.lblTotal = new DevExpress.XtraEditors.LabelControl();
            this.lblActual = new DevExpress.XtraEditors.LabelControl();
            this.panel = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panel)).BeginInit();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgbTotal
            // 
            this.pgbTotal.Location = new System.Drawing.Point(12, 5);
            this.pgbTotal.Name = "pgbTotal";
            this.pgbTotal.Size = new System.Drawing.Size(360, 20);
            this.pgbTotal.TabIndex = 0;
            // 
            // pgbActual
            // 
            this.pgbActual.Location = new System.Drawing.Point(56, 50);
            this.pgbActual.Name = "pgbActual";
            this.pgbActual.Size = new System.Drawing.Size(360, 23);
            this.pgbActual.TabIndex = 0;
            // 
            // lblTotal
            // 
            this.lblTotal.Location = new System.Drawing.Point(199, 31);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(24, 13);
            this.lblTotal.TabIndex = 1;
            this.lblTotal.Text = "Total";
            // 
            // lblActual
            // 
            this.lblActual.Location = new System.Drawing.Point(203, 79);
            this.lblActual.Name = "lblActual";
            this.lblActual.Size = new System.Drawing.Size(30, 13);
            this.lblActual.TabIndex = 1;
            this.lblActual.Text = "Actual";
            // 
            // panel
            // 
            this.panel.Controls.Add(this.pgbTotal);
            this.panel.Controls.Add(this.lblActual);
            this.panel.Controls.Add(this.lblTotal);
            this.panel.Controls.Add(this.pgbActual);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(384, 111);
            this.panel.TabIndex = 2;
            // 
            // LoadingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 111);
            this.ControlBox = false;
            this.Controls.Add(this.panel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(400, 150);
            this.MinimumSize = new System.Drawing.Size(400, 150);
            this.Name = "LoadingDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cargando datos";
            ((System.ComponentModel.ISupportInitialize)(this.panel)).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pgbTotal;
        private System.Windows.Forms.ProgressBar pgbActual;
        private DevExpress.XtraEditors.LabelControl lblTotal;
        private DevExpress.XtraEditors.LabelControl lblActual;
        private DevExpress.XtraEditors.PanelControl panel;
    }
}