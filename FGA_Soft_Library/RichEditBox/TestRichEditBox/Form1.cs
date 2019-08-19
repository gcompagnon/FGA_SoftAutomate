using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace TestRichEditBox
{
	/// <summary>
	/// Description résumée de Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private vbMaf.Windows.Forms.RichEditBox richEditBox1;
        private vbMaf.Windows.Forms.RichEditBox richEditBox3;
        //private vbMaf.Windows.Forms.CommandBar.CommandBarManager commandBarManager1;
        private vbMaf.Windows.Forms.RichEditBox richEditBox2;
        private Button button1;
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Requis pour la prise en charge du Concepteur Windows Forms
			//
			InitializeComponent();

			//
			// TODO : ajoutez le code du constructeur après l'appel à InitializeComponent
			//
		}

		/// <summary>
		/// Nettoyage des ressources utilisées.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Code généré par le Concepteur Windows Form
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
            this.richEditBox1 = new vbMaf.Windows.Forms.RichEditBox();
            this.richEditBox3 = new vbMaf.Windows.Forms.RichEditBox();
            //this.commandBarManager1 = new vbMaf.Windows.Forms.CommandBar.CommandBarManager();
            this.richEditBox2 = new vbMaf.Windows.Forms.RichEditBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richEditBox1
            // 
            this.richEditBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richEditBox1.Location = new System.Drawing.Point(32, 16);
            this.richEditBox1.Name = "richEditBox1";
            this.richEditBox1.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1036{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft S" +
                "ans Serif;}}\r\n\\viewkind4\\uc1\\pard\\f0\\fs17 richEditBox1\\par\r\n}\r\n";
            this.richEditBox1.ShowSelectionMargin = true;
            this.richEditBox1.Size = new System.Drawing.Size(526, 162);
            this.richEditBox1.TabIndex = 0;
            // 
            // richEditBox3
            // 
            this.richEditBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richEditBox3.Location = new System.Drawing.Point(32, 409);
            this.richEditBox3.Name = "richEditBox3";
            this.richEditBox3.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft Sans Serif;}}" +
                "\r\n\\viewkind4\\uc1\\pard\\lang1036\\f0\\fs17 richEditBox3\\par\r\n}\r\n";
            this.richEditBox3.ShowSelectionMargin = true;
            this.richEditBox3.Size = new System.Drawing.Size(526, 157);
            this.richEditBox3.TabIndex = 1;
            // 
            // commandBarManager1
            // 
            //this.commandBarManager1.Dock = System.Windows.Forms.DockStyle.Top;
            //this.commandBarManager1.Location = new System.Drawing.Point(0, 0);
            //this.commandBarManager1.Name = "commandBarManager1";
            //this.commandBarManager1.Size = new System.Drawing.Size(570, 0);
            //this.commandBarManager1.TabIndex = 2;
            //this.commandBarManager1.TabStop = false;
            //this.commandBarManager1.Text = "commandBarManager1";
            //// 
            // richEditBox2
            // 
            this.richEditBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richEditBox2.Location = new System.Drawing.Point(32, 198);
            this.richEditBox2.Name = "richEditBox2";
            this.richEditBox2.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft Sans Serif;}}" +
                "\r\n\\viewkind4\\uc1\\pard\\lang1036\\f0\\fs17 richEditBox2\\par\r\n}\r\n";
            this.richEditBox2.ShowSelectionMargin = true;
            this.richEditBox2.Size = new System.Drawing.Size(526, 185);
            this.richEditBox2.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(-49, 226);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(570, 578);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richEditBox2);
            //this.Controls.Add(this.commandBarManager1);
            this.Controls.Add(this.richEditBox3);
            this.Controls.Add(this.richEditBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Point d'entrée principal de l'application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

        private void Form1_Load(object sender, EventArgs e)
        {

        }
	}
}
