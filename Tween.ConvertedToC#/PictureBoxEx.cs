using System;
using System.Windows.Forms;

namespace Tween.TweenCustomControl
{
    public class PictureBoxEx : PictureBox
    {
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
        {
            try
            {
                base.OnPaint(pe);
            }
            catch (OutOfMemoryException ex)
            {
            }
        }
    }
}