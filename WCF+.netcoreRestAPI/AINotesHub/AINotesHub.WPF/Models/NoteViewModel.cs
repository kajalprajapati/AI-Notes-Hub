using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using AINotesHub.Shared;

namespace AINotesHub.WPF.Models
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        public Note Model { get; }

        public NoteViewModel(Note note)
        {
            Model = note;
        }

        // Forward property changes for properties you expose
        // Expose properties for binding and notify WPF UI
        public string Title
        {
            get => Model.Title;
            set { if (Model.Title != value) { Model.Title = value; OnPropertyChanged(nameof(Title)); } }
        }

        public string Content
        {
            get => Model.Content;
            set { if (Model.Content != value) { Model.Content = value; OnPropertyChanged(nameof(Content)); } }
        }

        public string Category
        {
            get => Model.Category;
            set { if (Model.Category != value) { Model.Category = value; OnPropertyChanged(nameof(Category)); } }
        }

        public string CardBackground
        {
            get => Model.CardBackground;
            set { if (Model.CardBackground != value) { Model.CardBackground = value; OnPropertyChanged(nameof(CardBackground)); } }
        }

        public DateTime CreatedAt
        {
            get => Model.CreatedAt;
            set { if (Model.CreatedAt != value) { Model.CreatedAt = value; 
                    OnPropertyChanged(nameof(CreatedAt)); } }
        }

        public DateTime UpdatedAt
        {
            get => (DateTime)Model.UpdatedAt;
            set
            {
                if (Model.UpdatedAt != value)
                {
                    Model.UpdatedAt = value;
                    OnPropertyChanged(nameof(UpdatedAt));
                }
            }
        }

        // Add more if you expose more properties (Title, Content, etc.)

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}











