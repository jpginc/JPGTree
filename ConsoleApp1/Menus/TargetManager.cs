﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ConsoleApp1.BuiltInActions
{
    [DataContract]
    [KnownType(typeof(Target))]
    public class TargetManager : IManagerAndActionProvider
    {
        [IgnoreDataMember] public ProjectPersistence Settings;

        [IgnoreDataMember] public string ManageText => "Manage Targets";
        [IgnoreDataMember] public string CreateChoiceText => "Create Target";

        [IgnoreDataMember] public string DeleteChoiceText => "Delete Targets";

        [DataMember] public List<ICreatable> Creatables { get; set; } = new List<ICreatable>();

        public TargetManager()
        {
        }

        public void Save()
        {
            Settings.Save();
        }

        public void Delete(ICreatable creatable)
        {
            lock (Creatables)
            {
                Creatables.Remove(creatable);
            }
            Save();
        }

        public void New(UserActionResult obj)
        {
            var target = new Target();
            if (CreatableWizard.GetRequiredFields(target))
            {
                lock (Creatables)
                {
                    Creatables.Add(target);
                    Save();
                }

            }
        }

        public bool HasChildren()
        {
            return true;
        }

        public IEnumerable<ICreatable> GetChildren(ICreatable parent)
        {
            if (parent is Target)
            {
                return GetChildren((Target) parent);
            }
            return Enumerable.Empty<ICreatable>();
        }

        public InputType InputType => InputType.Single;
        public IEnumerable<ITreeViewChoice> GetActions()
        {
            return Creatables.Select(c => new AutoAction(c, this));
        }

        public ActionProviderResult HandleUserAction(UserActionResult res)
        {
            return ActionProviderResult.PassToTreeViewChoices;
        }

        public void AddPremade(Target target)
        {
            if (! Creatables.Any(p => ((Target) p).IpOrDomain.Equals(target.IpOrDomain)))
            {
                Creatables.Add(target);
            }
        }

        public IEnumerable<ICreatable> GetChildren(Target target)
        {
            return Settings.PortManager.GetChildren(target);
        }

        public Target GetTarget(string targetName)
        {
            return Creatables.FirstOrDefault(t => ((Target) t).IpOrDomain == targetName) as Target;
        }

        public Target GetTargetById(string uid)
        {
            return Creatables.FirstOrDefault(t => t.UniqueId.Equals(uid)) as Target;
        }
    }

    internal interface IManagerAndActionProvider : IActionProvider , IManager
    {
    }

    [DataContract]
    public class Target : BaseCreatable
    {
        [DataMember]
        [AutoSingleLineString]
        [Wizard]
        public string IpOrDomain { get; set; }
        [DataMember] public List<string> CommandsRun { get; set; } = new List<string>();
        [IgnoreDataMember] public override string ThisSummary => IpOrDomain;
        [IgnoreDataMember] public override string FullSummary => $"{IpOrDomain}{MyTagsAndChildTags}{MyPortsString}{MyChildNotes}";
        public string MyChildNotes => string.Join("\n", MyChildren.Select(c => c.SummaryForParent));

        public string MyTagsAndChildTags
        {
            get
            {
                var x = string.Join(", ", MyPorts.SelectMany(p => p.Tags)
                    .Select(t => t.ThisSummary)
                    .Distinct());
                return x.Equals("") ? "" : $"\nTags: {x}";
            }
        }


        [IgnoreDataMember] public override string SummaryForParent => FullSummary;

        [IgnoreDataMember]
        public IEnumerable<ICreatable> MyChildren => ChildrenReferences
            .Select(c => ProgramSettingsClass.Instance.GetCreatable(c))
            .Where(c => c != null);

        [IgnoreDataMember] public IEnumerable<Port> MyPorts => MyChildren.Where(c => c.GetType() == typeof(Port))
            .Select(c=> (Port)c);


        [IgnoreDataMember] public string MyPortsString => "\nPorts: " + string.Join(", ", MyChildren.Select(c => c.ThisSummary));


        [IgnoreDataMember] public override string EditChoiceText => IpOrDomain;
    }
}