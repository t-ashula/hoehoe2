using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
	public partial class MessageForm
	{

		public new Windows.Forms.DialogResult ShowDialog(string message)
		{
			Label1.Text = message;

			// ラベルコントロールをセンタリング
			Label1.Left = (this.ClientSize.Width - Label1.Size.Width) / 2;

			Label1.Refresh();
			Label1.Visible = true;
			return base.ShowDialog();
		}
		public MessageForm()
		{
			InitializeComponent();
		}
	}
}
