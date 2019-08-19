using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using vbMaf.Windows.Forms;
using vbMaf.Windows.Forms.CommandBar;
using System.Drawing.Text;

namespace vbMaf.Windows.Forms
{
	/// <summary>
	/// Description résumée de RichEditBox.
	/// </summary>
	public class RichEditBox : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.RichTextBox richTextBox1;
		private vbMaf.Windows.Forms.CommandBar.CommandBarManager commandBarManager1;
		private vbMaf.Windows.Forms.CommandBar.CommandBar commandBar1;
		private vbMaf.Windows.Forms.CommandBar.CommandBarContextMenu contextMenu;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ComboBox ComboFont;
		private System.Windows.Forms.ComboBox ComboFontSize;
        private System.Windows.Forms.ComboBox ComboColor;
		private System.Timers.Timer timer;
        CommandBarCheckBox LeftAlign, Center, RightAlign; //, Justify;

		public RichEditBox()
		{
			// Cet appel est requis par le Concepteur de formulaires Windows.Forms.
			InitializeComponent();
			this.SetStyle(
				ControlStyles.SupportsTransparentBackColor,
				true);

			// TODO : ajoutez les initialisations après l'appel à InitializeComponent
			commandBarManager1 = new CommandBarManager();
			commandBar1 = new vbMaf.Windows.Forms.CommandBar.CommandBar(this.commandBarManager1, CommandBarStyle.ToolBar);

            //Création des combobox
			ComboFont = new ComboBox();
			ComboFont.DrawMode = DrawMode.OwnerDrawVariable;
            ComboFontSize = new ComboBox();
			ComboFontSize.Width = 45;
            ComboColor = new ComboBox();
            ComboColor.Width = 75;
			
			LoadFont();
			LoadFontSize();
            LoadColor();
			ComboFontSize.Text = "10";
            ComboColor.Text = "Noir";
			commandBar1.Items.AddComboBox("Font", ComboFont);
			commandBar1.Items.AddComboBox("Taille", ComboFontSize);
            commandBar1.Items.AddComboBox("Couleur", ComboColor);
			commandBar1.Items.AddSeparator();


            CommandBarCheckBox GChk = new CommandBarCheckBox(imageList1.Images[0], "Gras");
			GChk.Shortcut = Keys.Control | Keys.G;
            CommandBarCheckBox IChk = new CommandBarCheckBox(imageList1.Images[1], "Italic");
			IChk.Shortcut = Keys.Control | Keys.I;
            CommandBarCheckBox SChk = new CommandBarCheckBox(imageList1.Images[2], "Souligné");
			SChk.Shortcut = Keys.Control | Keys.U;

			commandBar1.Items.AddRange(new CommandBarCheckBox[] {GChk, IChk, SChk});
			commandBar1.Items.AddSeparator();

			LeftAlign	= new CommandBarCheckBox(imageList1.Images[3], "Aligner à gauche");
			Center		= new CommandBarCheckBox(imageList1.Images[4], "Centrer");
			RightAlign	= new CommandBarCheckBox(imageList1.Images[5], "Aligner à droite");

			LeftAlign.IsChecked = true;
			commandBar1.Items.AddRange(new CommandBarCheckBox[] {LeftAlign, Center, RightAlign/*, Justify*/});
			commandBar1.Items.AddSeparator();

            CommandBarButton Cut		= new CommandBarButton(imageList1.Images[8], "Couper", null, Keys.Control | Keys.X);
			CommandBarButton Copy	= new CommandBarButton(imageList1.Images[7], "Copier", null, Keys.Control | Keys.C);
			CommandBarButton Paste	= new CommandBarButton(imageList1.Images[9], "Coller",null, Keys.Control | Keys.V);
			commandBar1.Items.AddRange( new CommandBarButton[] {Cut, Copy, Paste});
			commandBar1.Items.AddSeparator();
			CommandBarButton Undo	= new CommandBarButton(imageList1.Images[10], "Annuler", null, Keys.Control | Keys.Z);
            CommandBarButton Redo = new CommandBarButton(imageList1.Images[11], "Refaire", null, Keys.Control | Keys.Y);
			commandBar1.Items.AddRange( new CommandBarButton[] {Undo, Redo});
			commandBar1.Items.AddSeparator();

            CommandBarButton Delete = new CommandBarButton(global::RichEditBox.Properties.Resources.delete, "Supprimer", null, Keys.Control | Keys.Delete);
            commandBar1.Items.AddRange(new CommandBarButton[] { Delete });
            commandBar1.Items.AddSeparator();
            CommandBarButton Print = new CommandBarButton(global::RichEditBox.Properties.Resources.imprimante, "Imprimer", null, Keys.Control | Keys.P);
            //commandBar1.Items.AddRange(new CommandBarButton[] { Print });


			// Menu contextuel
			contextMenu = new CommandBarContextMenu();
			contextMenu.Items.AddButton(imageList1.Images[10], "Annuler", new EventHandler(Undo_Click));
			contextMenu.Items.AddSeparator();
			contextMenu.Items.AddButton(null, "Sélectionner Tout", new EventHandler(SelectAll_Click));
			contextMenu.Items.AddButton(imageList1.Images[8], "Couper", new EventHandler(Cut_Click));
			contextMenu.Items.AddButton(imageList1.Images[7], "Copier", new EventHandler(Copy_Click));
			contextMenu.Items.AddButton(imageList1.Images[9], "Coller", new EventHandler(Paste_Click));
            contextMenu.Items.AddButton(imageList1.Images[12], "Supprimer", new EventHandler(Delete_Click));


			commandBarManager1.Dock = DockStyle.Top;
			commandBar1.Dock = DockStyle.Fill;

			commandBarManager1.CommandBars.Add(commandBar1);
			this.Controls.Add(commandBarManager1);
            //this.Controls.Add(contextMenu);

            // branchement du menu sur le click droit
            // GUILLAUME : on retire car ça plante dans l appli FGA_FRONT
			//this.richTextBox1.ContextMenu = contextMenu;
			this.richTextBox1.Dock = DockStyle.Fill;

			if (!DesignMode) {
				ComboFont.MeasureItem				+= new MeasureItemEventHandler(ComboFont_MeasureItem);
				ComboFont.DrawItem					+= new DrawItemEventHandler(ComboFont_DrawItem);
				GChk.Click							+= new EventHandler(GChk_Click);
				IChk.Click							+= new EventHandler(IChk_Click);
				SChk.Click							+= new EventHandler(SChk_Click);
				LeftAlign.Click						+= new EventHandler(LeftAlign_Click);
				Center.Click						+= new EventHandler(Center_Click);
				RightAlign.Click					+= new EventHandler(RightAlign_Click);
//				Justify.Click						+= new EventHandler(Justify_Click);
				Cut.Click							+= new EventHandler(Cut_Click);
				Copy.Click							+= new EventHandler(Copy_Click);
				Paste.Click							+= new EventHandler(Paste_Click);
				Undo.Click							+= new EventHandler(Undo_Click);
				Redo.Click							+= new EventHandler(Redo_Click);
                Delete.Click                        += new EventHandler(Delete_Click);
                Print.Click                         += new EventHandler(Print_Click);
				ComboFont.SelectedIndexChanged		+= new EventHandler(ComboFont_SelectedIndexChanged);
				ComboFontSize.SelectedIndexChanged	+= new EventHandler(ComboFontSize_SelectedIndexChanged);
                ComboColor.SelectedIndexChanged     += new EventHandler(ComboColorSelectedIndexChanged);

				timer = new System.Timers.Timer(100);
				timer.Elapsed						+= new System.Timers.ElapsedEventHandler(timer_Elapsed);
				timer.Start();
			}

			//ComboFont.Enabled = false;
		}


		public new Font Font {
			get {return richTextBox1.Font;}
			set {richTextBox1.Font = value;}
		}
		[Category("Behavior"), DefaultValue(false)]
		public bool ShowSelectionMargin {
			get {return richTextBox1.ShowSelectionMargin; }
			set {richTextBox1.ShowSelectionMargin = value;}
		}

		[Category("Behavior"), DefaultValue(true)]
		public bool ShowToolBar {
			get {return commandBarManager1.Visible;}
			set {commandBarManager1.Visible = value;}
		}
		[Browsable(true)]
		public override string Text {
			get {return richTextBox1.Text;}
			set {richTextBox1.Text = value;}
		}

		[Category("Behavior"), DefaultValue(RichTextBoxScrollBars.Both)]
		public RichTextBoxScrollBars ScrollBars {
			get {return richTextBox1.ScrollBars;}
			set {richTextBox1.ScrollBars = value;}
		}

		[Browsable(false)]
		public string Rtf {
			get {return richTextBox1.Rtf;}
			set {richTextBox1.Rtf = value;}
		}
		/// <summary> 
		/// Nettoyage des ressources utilisées.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			System.Windows.Forms.Application.DoEvents();
			if( disposing )
			{
				timer.Stop();
				//timer.Dispose();
				Application.DoEvents();
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Code généré par le Concepteur de composants
		/// <summary> 
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RichEditBox));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.AcceptsTab = true;
            this.richTextBox1.AutoWordSelection = true;
            this.richTextBox1.HideSelection = false;
            this.richTextBox1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.richTextBox1.Location = new System.Drawing.Point(120, 96);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ShowSelectionMargin = true;
            this.richTextBox1.Size = new System.Drawing.Size(192, 136);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "richTextBox1";
            this.richTextBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.richTextBox1_KeyPress);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            this.imageList1.Images.SetKeyName(11, "");
            this.imageList1.Images.SetKeyName(12, "");
            // 
            // RichEditBox
            // 
            this.Controls.Add(this.richTextBox1);
            this.Name = "RichEditBox";
            this.Size = new System.Drawing.Size(440, 368);
            this.Load += new System.EventHandler(this.RichEditBox_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void richTextBox1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			//((CommandBarCheckBox)commandBar1.Items[0]).IsChecked = richTextBox1.SelectionFont.Bold;
		}

		private void LoadFont() {
			FontComboItem item;
			FontCollection fontCollection = new InstalledFontCollection();
			ComboFont.Items.Clear();

			ComboFont.BeginUpdate();
			for (int i=0; i<fontCollection.Families.Length; i++) {
				
				if (!fontCollection.Families[i].IsStyleAvailable(FontStyle.Regular))
					continue;

				
				item = new FontComboItem(fontCollection.Families[i].Name);
				ComboFont.Items.Add(item);
			}
			ComboFont.EndUpdate();
			ComboFont.Text = this.Font.FontFamily.Name;

		}

		internal class FontComboItem {
			private Font font;
			public FontComboItem(Font font) {
				this.font = font;
			}

			public FontComboItem(string family) {
				this.font = new Font(family, 8);
			}

			public override string ToString() {
				return font.FontFamily.Name;
			}

			public Font Font {
				get {return font;}		
			}

		
		}

		private void GChk_Click(object sender, EventArgs e) {
//			if (richTextBox1.SelectionFont == null)
//				return;
			CommandBarCheckBox GButton = sender as CommandBarCheckBox;
			
			if (GButton.IsChecked) {
				ApplyStyle(FontStyle.Bold);
			} else {
				RemoveStyle(FontStyle.Bold);
			}
		}

		private void IChk_Click(object sender, EventArgs e) {
//			if (richTextBox1.SelectionFont == null)
//				return;
			CommandBarCheckBox GButton = sender as CommandBarCheckBox;

			if (GButton.IsChecked) {
				ApplyStyle(FontStyle.Italic);
			} else {
				RemoveStyle(FontStyle.Italic);
			}
		}

		private void SChk_Click(object sender, EventArgs e) {
//			if (richTextBox1.SelectionFont == null)
//				return;
			CommandBarCheckBox GButton = sender as CommandBarCheckBox;
			
			if (GButton.IsChecked) {
				ApplyStyle(FontStyle.Underline);
			} else {
				RemoveStyle(FontStyle.Underline);
			}
		}


        private void Delete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Really delete?", "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                richTextBox1.Text = "";
            }
        }


        private void Print_Click(object sender, EventArgs e)
        {
            
        }

		private void ApplyStyle(FontStyle style) {
			try {
				int start = richTextBox1.SelectionStart;
				int length = richTextBox1.SelectionLength;
				richTextBox1.SuspendLayout();

				RichTextBox rtb = new RichTextBox();
				rtb.Rtf = richTextBox1.SelectedRtf;
				for (int i=0; 
					i<rtb.TextLength;
					i++) {
					rtb.Select(i,1);

					rtb.SelectionFont = 
						new Font(rtb.SelectionFont.FontFamily.Name,
						rtb.SelectionFont.Size,
						rtb.SelectionFont.Style | style);
				}
				rtb.Select(0, rtb.TextLength);
				richTextBox1.SelectedRtf = rtb.SelectedRtf;
				richTextBox1.Select(start, length);

				richTextBox1.ResumeLayout();
				rtb.Dispose();
				rtb = null;
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

		private void RemoveStyle(FontStyle style) {
			try {
				int start = richTextBox1.SelectionStart;
				int length = richTextBox1.SelectionLength;
				RichTextBox rtb = new RichTextBox();
				richTextBox1.SuspendLayout();
				rtb.Rtf = richTextBox1.SelectedRtf;
				for (int i=0; 
					i<rtb.TextLength;
					i++) {
					rtb.Select(i,1);

					rtb.SelectionFont = 
						new Font(rtb.SelectionFont.FontFamily.Name,
						rtb.SelectionFont.Size,
						rtb.SelectionFont.Style & ~style);
				}
				rtb.Select(0, rtb.TextLength);
				richTextBox1.SelectedRtf = rtb.SelectedRtf;
				richTextBox1.Select(start, length);
				richTextBox1.ResumeLayout();
				rtb.Dispose();
				rtb = null;
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

		private void ApplyFont(Font font) {
			try {
				int start = richTextBox1.SelectionStart;
				int length = richTextBox1.SelectionLength;
				RichTextBox rtb = new RichTextBox();
				richTextBox1.SuspendLayout();
				rtb.Rtf = richTextBox1.SelectedRtf;
				for (int i=0; 
					i<rtb.TextLength;
					i++) {
					rtb.Select(i,1);

					rtb.SelectionFont = 
						new Font(font.Name,
						font.Size,
						rtb.SelectionFont.Style);
				}
				rtb.Select(0, rtb.TextLength);
				richTextBox1.SelectedRtf = rtb.SelectedRtf;
				richTextBox1.Select(start, length);
				richTextBox1.ResumeLayout();
				rtb.Dispose();
				rtb = null;
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

        private void ApplyColor(Color color)
        {
            try
            {
                int start = richTextBox1.SelectionStart;
                int length = richTextBox1.SelectionLength;
                RichTextBox rtb = new RichTextBox();
                richTextBox1.SuspendLayout();
                rtb.Rtf = richTextBox1.SelectedRtf;
                for (int i = 0;
                    i < rtb.TextLength;
                    i++)
                {
                    rtb.Select(i, 1);
                    rtb.SelectionColor = color;                       
                }
                rtb.Select(0, rtb.TextLength);
                richTextBox1.SelectedRtf = rtb.SelectedRtf;
                richTextBox1.Select(start, length);
                richTextBox1.ResumeLayout();
                rtb.Dispose();
                rtb = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


		private void ComboFont_MeasureItem(object sender, MeasureItemEventArgs e) {
			try {
				FontComboItem item = ComboFont.Items[e.Index] as FontComboItem;

				if (item == null)
					return;

				Size size = e.Graphics.MeasureString(item.Font.FontFamily.Name, item.Font).ToSize();
				e.ItemHeight = ComboFont.ItemHeight;
				e.ItemWidth = size.Width;

				ComboFont.DropDownWidth = Math.Max(ComboFont.DropDownWidth, size.Width);
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

		private void ComboFont_DrawItem(object sender, DrawItemEventArgs e) {
			try {
				FontComboItem item = ComboFont.Items[e.Index] as FontComboItem;

				if (item == null)
					return;

				e.DrawBackground();
			
				e.Graphics.DrawString(item.Font.Name, new Font(item.Font.FontFamily.Name, this.Font.Size), 
					new SolidBrush(e.ForeColor), e.Bounds, StringFormat.GenericDefault);
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}

		}

        private void ComboColorSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ComboColor.DroppedDown) return;
                if (ComboColor.Text == "Rouge") { richTextBox1.SelectionColor = Color.Red; }
                else if (ComboColor.Text == "Vert") { richTextBox1.SelectionColor = Color.Green; }
                else if (ComboColor.Text == "Bleu") { richTextBox1.SelectionColor = Color.Blue; }
                else { richTextBox1.SelectionColor = Color.Black; }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }          

        }

		private void ComboFont_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				if (ComboFont.DroppedDown)
					return;
				int emSize = 8;
				if (ComboFontSize.Text != string.Empty &&
					(new System.Text.RegularExpressions.Regex("[0-9]+")).Match(ComboFontSize.Text).Success) {
					emSize = Convert.ToInt32(ComboFontSize.Text);
				}
				//richTextBox1.SelectionFont = new Font(ComboFont.Text, emSize);
				ApplyFont(new Font(ComboFont.Text, emSize));
				//LoadFontSize();
				ComboFontSize.Text = emSize.ToString();
				richTextBox1.Select();
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}


		private void ComboFontSize_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				if (ComboFontSize.DroppedDown)
					return;

				if (ComboFontSize.Text != string.Empty &&
					(new System.Text.RegularExpressions.Regex("[0-9]+")).Match(ComboFontSize.Text).Success) {
					//richTextBox1.SelectionFont = new Font(ComboFont.Text, Convert.ToInt32(ComboFontSize.Text));
					ApplyFont(new Font(ComboFont.Text, Convert.ToInt32(ComboFontSize.Text)));
				}
				richTextBox1.Select();
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}

		}

		private void LoadFontSize() {
			try {
				if (DesignMode)
					return;
				ComboFontSize.Items.Clear();
				ComboFontSize.Items.Add(6);
				ComboFontSize.Items.Add(7);
				ComboFontSize.Items.Add(8);
				ComboFontSize.Items.Add(9);
				ComboFontSize.Items.Add(10);
				ComboFontSize.Items.Add(11);
				ComboFontSize.Items.Add(12);
				ComboFontSize.Items.Add(14);
				ComboFontSize.Items.Add(16);
				ComboFontSize.Items.Add(18);
				ComboFontSize.Items.Add(20);
				ComboFontSize.Items.Add(22);
				ComboFontSize.Items.Add(24);
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

        private void LoadColor()
        {
            try
            {
                if (DesignMode)
                    return;
                ComboColor.Items.Clear();
                ComboColor.Items.Add("Noir");
                ComboColor.Items.Add("Bleu");
                ComboColor.Items.Add("Rouge");
                ComboColor.Items.Add("Vert");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

		private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			try {
				if (!this.Bounds.Contains(MousePosition) && !this.richTextBox1.Focused) {
					//commandBarManager1.Visible = !autoHideToolBar;
					return;
				}

				//commandBarManager1.Visible = true;

				if (!ComboFont.DroppedDown && !ComboFontSize.DroppedDown && !ComboColor.DroppedDown) {
					((CommandBarCheckBox)commandBar1.Items[3]).IsChecked = richTextBox1.SelectionFont.Bold;
					((CommandBarCheckBox)commandBar1.Items[4]).IsChecked = richTextBox1.SelectionFont.Italic;
					((CommandBarCheckBox)commandBar1.Items[5]).IsChecked = richTextBox1.SelectionFont.Underline;

					((CommandBarCheckBox)commandBar1.Items[7]).IsChecked = (richTextBox1.SelectionAlignment == HorizontalAlignment.Left);
					((CommandBarCheckBox)commandBar1.Items[8]).IsChecked = (richTextBox1.SelectionAlignment == HorizontalAlignment.Center);
					((CommandBarCheckBox)commandBar1.Items[9]).IsChecked = (richTextBox1.SelectionAlignment == HorizontalAlignment.Right);
					//((CommandBarCheckBox)commandBar1.Items[10]).IsChecked = (richTextBox1.SelectionAlignment == (HorizontalAlignment.Left | HorizontalAlignment.Right));

					commandBar1.Items[12].IsEnabled = (richTextBox1.SelectionLength>0);
					commandBar1.Items[13].IsEnabled = (richTextBox1.SelectionLength>0);
					commandBar1.Items[14].IsEnabled = 
						richTextBox1.CanPaste(DataFormats.GetFormat(DataFormats.Text)) ||
						richTextBox1.CanPaste(DataFormats.GetFormat(DataFormats.Rtf));

					commandBar1.Items[16].IsEnabled = richTextBox1.CanUndo;
					commandBar1.Items[17].IsEnabled = richTextBox1.CanRedo;
                    //commandBar1.Items[18].IsEnabled = richTextBox1.dele;

					contextMenu.Items[0].IsEnabled = richTextBox1.CanUndo;
					contextMenu.Items[3].IsEnabled = commandBar1.Items[12].IsEnabled;
					contextMenu.Items[4].IsEnabled = commandBar1.Items[13].IsEnabled;
					contextMenu.Items[5].IsEnabled = commandBar1.Items[14].IsEnabled;

					ComboFont.Text = richTextBox1.SelectionFont.FontFamily.Name;
					ComboFontSize.Text = ((int)richTextBox1.SelectionFont.Size).ToString();
                    ComboColor.Text = (richTextBox1.SelectionColor).ToString();

				}
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}

		}

		private void LeftAlign_Click(object sender, EventArgs e) {
			if (DesignMode)
				return;
			richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
			RightAlign.IsChecked = false;
			Center.IsChecked = false;
		}

		private void Center_Click(object sender, EventArgs e) {
			if (DesignMode)
				return;
			richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
			RightAlign.IsChecked = false;
			LeftAlign.IsChecked = false;
		}

		private void RightAlign_Click(object sender, EventArgs e) {
			if (DesignMode)
				return;
			richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
			LeftAlign.IsChecked = false;
			Center.IsChecked = false;
		}

		private void Justify_Click(object sender, EventArgs e) {
			if (DesignMode)
				return;

		}

		private void Cut_Click(object sender, EventArgs e) {
			if (DesignMode)
				return;
			richTextBox1.Cut();
		}

		private void Copy_Click(object sender, EventArgs e) {
			if (DesignMode)
				return;
			richTextBox1.Copy();
		}

		private void Paste_Click(object sender, EventArgs e) {
			if (DesignMode)
				return;
			richTextBox1.Paste();
		}

		private void Undo_Click(object sender, EventArgs e) {
			if (DesignMode)
				return;
			richTextBox1.Undo();
		}

		private void Redo_Click(object sender, EventArgs e) {
			if (DesignMode)
				return;
			richTextBox1.Redo();
		}

		private void RichEditBox_Load(object sender, System.EventArgs e) {
			this.richTextBox1.Select();
			this.richTextBox1.Select(this.richTextBox1.TextLength,0);
			ComboFont.Text = this.Font.FontFamily.Name;
			ComboFontSize.Text = this.Font.Size.ToString();
            ComboColor.Text = "Noir";

		}
		private void SelectAll_Click(object sender, EventArgs e) {
			richTextBox1.SelectAll();
		}
	}
}
