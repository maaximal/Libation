﻿using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LibationWinForms.Dialogs.Login
{
	public partial class CaptchaDialog : Form
	{
		public string Answer { get; private set; }

		private MemoryStream ms { get; }
		private Image image { get; }

		public CaptchaDialog(byte[] captchaImage)
		{
			InitializeComponent();
			this.FormClosed += (_, __) => { ms?.Dispose(); image?.Dispose(); };

			ms = new MemoryStream(captchaImage);
			image = Image.FromStream(ms);
			this.captchaPb.Image = image;
		}

		private void submitBtn_Click(object sender, EventArgs e)
		{
			Answer = this.answerTb.Text;

			Serilog.Log.Logger.Information("Submit button clicked: {@DebugInfo}", new { Answer });

			DialogResult = DialogResult.OK;
			// Close() not needed for AcceptButton
		}
	}
}