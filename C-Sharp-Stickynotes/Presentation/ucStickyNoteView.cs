﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using StickyNotesLibrary;

namespace C_Sharp_Stickynotes.Presentation
{
    public partial class ucStickyNoteView : UserControl
    {
        private StickyNotesLibrary.StickyNoteModel Item;

        // local event handlers
        public EventHandler DeleteNote;
        
        public ucStickyNoteView(StickyNotesLibrary.StickyNoteModel item)
        {
            Item = item;
            InitializeComponent();
        }


        // Gets the Children Controls of the given parent control paremeter.
        private List<Control> GetAllChildControls(Control parent)
        {
            List<Control> children = new List<Control>();
            children.AddRange(parent.Controls.Cast<Control>());
            foreach (Control child in parent.Controls)
            {
                children.AddRange(this.GetAllChildControls(child));
            }
            return children;
        }

        // populates each User Control with correct text inside the richTextBox and the correct corresponding colors.
        private void ucStickyNoteView_Load(object sender, EventArgs e)
        {
            Color c = Color.FromArgb(Item.NoteColor);
            List<Control> children = GetAllChildControls(panelNoteView);
            foreach (Control control in children)
            {
                switch (control)
                {
                    case RichTextBox:
                        control.Text = Item.NoteText;
                        control.BackColor = c;
                        break;
                    case Button:
                        control.BackColor = c;
                        break;
                    default:
                        break;
                }
            }
        }

        // Delete the sticky note from the sqlite local server from the user control in the frmStickyNoteList
        private void btnDeleteNote_Click(object sender, EventArgs e)
        {
            DialogResult dialogConfirmation = MessageBox.Show("Are you sure you want to delete this sticky note?", "Delete Confirmation", 
                                                               MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            
            switch (dialogConfirmation)
            {
                case DialogResult.Yes:
                    SQLiteStickyNoteAccess sqliteStickyNoteAccess = new SQLiteStickyNoteAccess();
                    sqliteStickyNoteAccess.DeleteStickyNote(Item.NoteID).Wait();
                    if (DeleteNote != null)
                        DeleteNote(this, e);
                    break;
                default:
                    break;
            }
        }

        private void panelNoteView_Click(object sender, EventArgs e)
        {
            var newNote = new Thread(() => Application.Run(new frmStickyNote(Item, true)));
            newNote.Start();
        }
    }
}
