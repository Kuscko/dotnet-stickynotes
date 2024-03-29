﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace StickyNotesLibrary
{
    public class SQLiteStickyNoteAccess : StickyNoteModel
    {
        public List<StickyNoteModel> GetStickyNotes()
        {
            // use 'using' to reliable close connections to the database.
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var note = conn.Query<StickyNoteModel>("SELECT * FROM StickyNotes");
                return note.ToList();
            }
        }

        // think whether save and update are necessary or simply one. How logic would look, update tomorrow TODO::
        public async Task SaveStickyNoteAsync(string text, int color)
        {
            string sql = "INSERT INTO StickyNotes " +
                         "(NoteText, NoteColor) " +
                         "VALUES (@noteText, @noteColor) ";
            // use 'using' to reliable close connections to the database.
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                await conn.ExecuteAsync(sql, new { noteText = text, noteColor = color });
            }
        }

        public async Task DeleteStickyNote(int noteID)
        {
            string sql = "DELETE FROM StickyNotes " +
                         "WHERE NoteID = @id";
            // use 'using' to reliable close connections to the database.
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                await conn.ExecuteAsync(sql, new { id = noteID });
            }
        }

        /// <summary>
        /// Updates the local SQLite database using the existing note's ID.
        /// </summary>
        /// <param name="text">The existing text comes from the "rtbNotes.Text".</param>
        /// <param name="color">The color is pulled from the parent BackColor of the sticky note then converted using .ToArgb().</param>
        /// <param name="noteID">Note ID is from the StickyNoteModel.NoteID passed when the sticky note is loaded from the SQLite local database.</param>
        /// <returns></returns>
        public async Task UpdateStickyNote(string text, int color, int noteID)
        {
            string sql = "UPDATE StickyNotes " +
                         "SET NoteText = @noteText, NoteColor = @noteColor " +
                         "WHERE NoteID = @id";
            // use 'using' to reliable close connections to the database.
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                await conn.ExecuteAsync(sql, new { noteText = text, noteColor = color, id = noteID });
            }
        }

        // gets the connectionstring labeled["Default"] from the App.config inside the executable 
        private static string LoadConnectionString(string id = "Default")
        {
            ConnectionStringSettings c = ConfigurationManager.ConnectionStrings[id];
            string fixedConnectionString = c.ConnectionString.Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory);
            return fixedConnectionString;
        }
    }
}
