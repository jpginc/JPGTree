﻿using System;
using ConsoleApp1.BuiltInActions;

namespace ConsoleApp1
{
    internal class NoteTreeViewChoice : ITreeViewChoice
    {
        public readonly Note Note;
        public string SortByText => Note.NoteName;
        public string Text => Note.NoteName;

        public NoteTreeViewChoice(Note note)
        {
            Note = note;
        }

        public bool OnTreeViewSelectCallback(JpgTreeView jpgTreeView)
        {
            MainWindow.Instance.SetInputText(Note.NoteContents);
            return true;
        }

        public bool OnAcceptCallback(UserActionResult choice)
        {
            JpgActionManager.PushActionContext(new NoteAction(Note));
            MainWindow.Instance.SetInputText(Note.NoteContents);
            return true;
        }

        public bool OnSaveCallback(UserActionResult choice)
        {
            Console.WriteLine("the save data is " + choice.MultiLineInput);
            Note.NoteContents = choice.MultiLineInput;
            NotesManager.Instance.Save();
            return true;
        }

    }
}