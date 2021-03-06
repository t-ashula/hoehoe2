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
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// タスクサービス機能付きプログレスバー
    /// </summary>
    /// <remarks>
    /// 重要：BackGroundWorkerコンポーネントが実際のタスクサービスを行うため、DoWorkでコントロールを触ることはできない。
    /// また、Twitterへの通信を必要とする場合は引数にTwitterInstanceを含めそれを使用すること。
    /// 1.Class生成
    /// 2.コンストラクタの引数としてサービス登録(Dowork RunWorkerCompletedも同様 使用しない場合Nothing)
    /// 3.Instance.Argumentへ,あるいはコンストラクタ引数へ引数セット
    /// 4.Instance.InfoMessage、またはコンストラクタ引数へ表示メッセージ設定
    /// 5.Instance.ShowDialog()により表示
    /// 6.必要な場合はInstance.Result(=Servicerのe.Result)を参照し戻り値を得る
    /// 7.Dispose タスクサービスが正常終了した場合は自分自身をCloseするので最後にDisposeすること。
    /// </remarks>
    public partial class FormInfo
    {
        #region privates

        private string _formMessage;
        private readonly BackgroundWorkerServicer _backGroundWorkerServicer;

        #endregion

        #region constructor

        public FormInfo(Form owner, string message, DoWorkEventHandler doWork, RunWorkerCompletedEventHandler runWorkerCompleted = null, object argument = null)
        {
            InitializeComponent();
            Owner = owner;
            InfoMessage = message;
            _backGroundWorkerServicer = new BackgroundWorkerServicer();
            _backGroundWorkerServicer.DoWork += doWork;
            if (runWorkerCompleted != null)
            {
                _backGroundWorkerServicer.RunWorkerCompleted += runWorkerCompleted;
            }

            Argument = argument;
        }

        #endregion

        #region properties

        /// <summary>
        /// ダイアログに表示されるユーザー向けメッセージを設定あるいは取得する
        /// </summary>
        public string InfoMessage
        {
            get { return _formMessage; }
            set { LabelInformation.Text = _formMessage = value; }
        }

        /// <summary>
        /// Servicerへ渡すパラメータ
        /// </summary>
        public object Argument { get; set; }

        /// <summary>
        /// Servicerのe.Result を取得します
        /// </summary>
        public object Result
        {
            get { return _backGroundWorkerServicer.Result; }
        }

        #endregion

        #region event handler

        private void LabelInformation_TextChanged(object sender, EventArgs e)
        {
            LabelInformation.Refresh();
        }

        private void FormInfo_Shown(object sender, EventArgs e)
        {
            _backGroundWorkerServicer.RunWorkerAsync(Argument);
            while (_backGroundWorkerServicer.IsBusy)
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }

            TopMost = false; // MessageBoxが裏に隠れる問題に対応
            Close();
        }

        private void FormInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            // フォームを閉じたあとに親フォームが最前面にならない問題に対応
            if (Owner != null && Owner.Created)
            {
                Owner.TopMost = !Owner.TopMost;
                Owner.TopMost = !Owner.TopMost;
            }
        }

        #endregion

        #region inner class

        private class BackgroundWorkerServicer : BackgroundWorker
        {
            public object Result { get; private set; }

            protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
            {
                Result = e.Result;
                base.OnRunWorkerCompleted(e);
            }
        }

        #endregion
    }
}