﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ConsoleApp1.BuiltInActions;

namespace ConsoleApp1
{
    [DataContract]
    public class Note : ICreatable, IComparable<Note>
    {
        [IgnoreDataMember] public string EditChoiceText => NoteName;
        [DataMember, AutoSingleLineString, Wizard] public string NoteName { get; set; }
        [DataMember] public readonly string UniqueId = Guid.NewGuid().ToString("N");
        [DataMember, AutoMultiLineString, Wizard] public virtual string NoteContents { get; set; } = "";
        [DataMember] public DateTime CreateTime { get; set; } = DateTime.Now;
        [DataMember] public List<string> Tags { get; set; } = new List<string>();
        public virtual int CompareTo(Note obj)
        {
            return string.Compare(UniqueId, obj.UniqueId, StringComparison.Ordinal);
        }
    }

    class UserNote : Note
    {
    }
}