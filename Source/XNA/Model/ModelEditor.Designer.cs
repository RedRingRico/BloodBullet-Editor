using Microsoft.Xna.Framework;
using System.Windows.Forms;
namespace BloodBulletEditor.Model
{
    partial class ModelEditor
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
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "ModelEditor";
            m_Closed = false;

            m_FileName = string.Empty;

            m_PerspectiveView = new PerspectiveViewControl( );
            m_PerspectiveView.Dock = System.Windows.Forms.DockStyle.Fill;
            m_PerspectiveView.Location = new System.Drawing.Point( 100, 100 );

            m_MainMenu = new MainMenu( );

            m_FileMenu = new MenuItem( "&File" );
            m_FileOpen = new MenuItem( "&Open",
                this.FileOpen, Shortcut.CtrlO );

            m_MainMenu.MenuItems.Add( m_FileMenu );
            m_FileMenu.MenuItems.Add( m_FileOpen );

            this.Menu = m_MainMenu;

            this.SuspendLayout( );

            this.Controls.Add( m_PerspectiveView );

            this.ResumeLayout( );

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( ModelEditor_FormClosing );
        }

        private void FileOpen( object p_Sender, System.EventArgs p_Args )
        {
            OpenFileDialog FileDialog = new OpenFileDialog( );

            FileDialog.Filter = "Blood Bullet file (*.blood)|*.blood";
            if( FileDialog.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
            {
                m_FileName = FileDialog.FileName;
            }

            FileDialog.Dispose( );
        }

        void ModelEditor_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            this.Hide( );
            m_Closed = true;

            if( e.CloseReason != System.Windows.Forms.CloseReason.ApplicationExitCall )
            {
                e.Cancel = true;
            }
        }

        public bool IsClosed( )
        {
            return m_Closed;
        }

        protected override void OnShown(System.EventArgs e)
        {
            base.OnShown(e);
            m_Closed = false;
        }

        protected override void OnClosed(System.EventArgs e)
        {
            this.Hide( );
            m_Closed = true;
        }

        private bool                    m_Closed;
        private PerspectiveViewControl  m_PerspectiveView;
        private MainMenu                m_MainMenu;
        private MenuItem                m_FileMenu;
        private MenuItem                m_FileOpen;
        private string                  m_FileName;

        #endregion
    }
}