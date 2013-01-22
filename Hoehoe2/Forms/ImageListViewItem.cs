// Hoehoe - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011- t.ashula (@t_ashula) <office@ashula.info>
//
// All rights reserved.
// This file is part of Hoehoe.
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

namespace Hoehoe
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class ImageListViewItem : ListViewItem
    {
        #region privates

        private readonly ImageDictionary _images;
        private readonly string _imageUrl;

        #endregion privates

        #region constructor

        public ImageListViewItem(string[] items, string imageKey)
            : base(items, imageKey)
        {
        }

        public ImageListViewItem(string[] items, ImageDictionary imageDictionary, string imageKey)
            : base(items, imageKey)
        {
            _images = imageDictionary;
            _imageUrl = imageKey;
            Image dummy = GetImage(false);
        }

        #endregion constructor

        #region delegates

        public delegate void ImageDownloadedEventHandler(object sender, EventArgs e);

        #endregion delegates

        #region events

        public event ImageDownloadedEventHandler ImageDownloaded;

        #endregion events

        #region properties

        public Image Image
        {
            get
            {
                return string.IsNullOrEmpty(_imageUrl) ? null : _images[_imageUrl];
            }
        }

        #endregion properties

        #region public methods

        public void RegetImage()
        {
            Image dummy = GetImage(true);
        }

        #endregion public methods

        #region private methods

        private Image GetImage(bool force)
        {
            return _images[_imageUrl, force, new Action<Image>(img =>
            {
                if (img == null)
                {
                    return;
                }

                if (ListView != null && ListView.Created && !ListView.IsDisposed)
                {
                    ListView.Invoke(new Action(() =>
                    {
                        if (Index < ListView.VirtualListSize)
                        {
                            ListView.RedrawItems(Index, Index, true);
                            OnImageDownloaded();
                        }
                    }));
                }
            })];
        }

        private void OnImageDownloaded()
        {
            if (ImageDownloaded != null)
            {
                ImageDownloaded(this, EventArgs.Empty);
            }
        }

        #endregion private methods
    }
}