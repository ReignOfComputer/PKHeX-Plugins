﻿using System;
using System.Windows.Forms;

namespace AutoModPlugins.GUI
{
    partial class SimpleHexEditor
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
            this.RTB_RAM = new System.Windows.Forms.RichTextBox();
            this.B_Update = new System.Windows.Forms.Button();
            this.PG_BlockView = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // RTB_RAM
            // 
            this.RTB_RAM.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RTB_RAM.Location = new System.Drawing.Point(9, 10);
            this.RTB_RAM.Name = "RTB_RAM";
            this.RTB_RAM.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.RTB_RAM.Size = new System.Drawing.Size(364, 292);
            this.RTB_RAM.TabIndex = 0;
            this.RTB_RAM.Text = "";
            // 
            // B_Update
            // 
            this.B_Update.Location = new System.Drawing.Point(9, 308);
            this.B_Update.Name = "B_Update";
            this.B_Update.Size = new System.Drawing.Size(363, 26);
            this.B_Update.TabIndex = 1;
            this.B_Update.Text = "Update";
            this.B_Update.UseVisualStyleBackColor = true;
            this.B_Update.Click += new System.EventHandler(this.Update_Click);
            // 
            // PG_BlockView
            // 
            this.PG_BlockView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PG_BlockView.Location = new System.Drawing.Point(9, 10);
            this.PG_BlockView.Name = "PG_BlockView";
            this.PG_BlockView.Size = new System.Drawing.Size(114, 130);
            this.PG_BlockView.TabIndex = 15;
            this.PG_BlockView.Visible = false;
            // 
            // SimpleHexEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 343);
            this.Controls.Add(this.PG_BlockView);
            this.Controls.Add(this.B_Update);
            this.Controls.Add(this.RTB_RAM);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SimpleHexEditor";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RAMEdit";
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.RichTextBox RTB_RAM;
        private Button B_Update;
        public PropertyGrid PG_BlockView;
    }
}