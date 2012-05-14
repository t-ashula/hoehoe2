// Tween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
// All rights reserved.
//
// This file is part of Tween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tween
{
    public class ImageListViewItem : ListViewItem
    {
        //Public Property Image As Image = Nothing
        public event ImageDownloadedEventHandler ImageDownloaded;

        public delegate void ImageDownloadedEventHandler(object sender, EventArgs e);

        private ImageDictionary _imageDict = null;
        private string _imageUrl;

        public ImageListViewItem(string[] items, string imageKey)
            : base(items, imageKey)
        {
        }

        public ImageListViewItem(string[] items, ImageDictionary imageDictionary, string imageKey)
            : base(items, imageKey)
        {
            this._imageDict = imageDictionary;
            this._imageUrl = imageKey;
            Image dummy = this.GetImage(false);
        }

        private Image GetImage(bool force)
        {
            return this._imageDict[this._imageUrl, force, getImg =>
            {
                if (getImg == null)
                {
                    return;
                }
                if (this.ListView != null && this.ListView.Created && !this.ListView.IsDisposed)
                {
                    this.ListView.Invoke(new Action(() =>
                    {
                        if (this.Index < this.ListView.VirtualListSize)
                        {
                            this.ListView.RedrawItems(this.Index, this.Index, true); OnImageDownloaded();
                        }
                    }));
                }
            }];
        }

        private void OnImageDownloaded()
        {
            if (ImageDownloaded != null)
            {
                ImageDownloaded(this, EventArgs.Empty);
            }
        }

        public Image Image
        {
            get { return String.IsNullOrEmpty(this._imageUrl) ? null : this._imageDict[this._imageUrl]; }
        }

        public void RegetImage()
        {
            Image dummy = GetImage(true);
        }
    }
}