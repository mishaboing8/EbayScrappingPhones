using System;

using SQLite;
using FirstXamarin.models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FirstXamarin
{
    public class NotesDB
    {
        readonly SQLiteAsyncConnection db;

        public NotesDB(string connectionString)
        {
            db = new SQLiteAsyncConnection(connectionString);

            db.CreateTableAsync<Notes>().Wait();
        }

        public Task<List<Notes>> GetNotesAsync()
        {
            return db.Table<Notes>().ToListAsync();
        }

        public Task<Notes> GetNotesAsync(int id)
        {
            return db.Table<Notes>()
                .Where(i => i.ID == id)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveNoteAsync(Notes note)
        {
            if (note.ID != 0)
                return db.UpdateAsync(note);

            return db.InsertAsync(note);
        }

        public Task<int> DeleteNoteAsync(Notes note)
        {
            return db.DeleteAsync(note);
        }
    }
}

