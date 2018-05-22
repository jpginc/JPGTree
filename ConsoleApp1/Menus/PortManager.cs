﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ConsoleApp1.BuiltInActions
{
    [DataContract]
    public class PortManager : Manager, IManagerAndActionProvider 
    {
        private Project _project;
        [IgnoreDataMember] public override string ManageText => "Manage Ports";
        [IgnoreDataMember] public override string CreateChoiceText => "New TcpPort";
        [IgnoreDataMember] public override string DeleteChoiceText => "Delete Ports";
        public override void New(UserActionResult obj)
        {
            var port = new Port();
            if (CreatableWizard.GetRequiredFields(port))
            {
                Creatables.Add(port);
                Save();
            }
        }

        public InputType InputType => InputType.Multi;
        public IEnumerable<ITreeViewChoice> GetActions()
        {
            return Creatables.Where(c => ((IDoneable) c).ScanItemStatus != ScanItemState.Done)
                .Select(c => new AutoAction(c, this));
        }

        public ActionProviderResult HandleUserAction(UserActionResult res)
        {
            if (res.Result == UserActionResult.ResultType.Accept
                && res.UserChoices.Count() > 1)
            {
                var x = new SelectCommandToRunMenu(Settings.Project);
                x.PrepopulatePorts(res.UserChoices.Select(c => ((AutoAction)c).Creatable as Port));
                JpgActionManager.PushActionContext(x);
                return ActionProviderResult.ProcessingFinished;
            }
            return ActionProviderResult.PassToTreeViewChoices;
        }

        public void AddPremade(Port port)
        {
            var existing = Creatables.FirstOrDefault(p =>
            {
                var c = (Port)p;
                return c.Target.Equals(port.Target) && c.PortNumber.Equals(port.PortNumber);
            });
            if (existing == null)
            {
                Creatables.Add(port);
            }
            else
            {
                ((Port) existing).Notes.AddRange(port.Notes);
            }
        }

        public IEnumerable<ICreatable> GetChildren(Target target)
        {
            return Creatables.Where(c => ((Port) c).Target.Equals(target.IpOrDomain));
        }

        public Port GetPort(string portNumber)
        {
            return (Port) Creatables.FirstOrDefault(c => ((Port) c).PortNumber.Equals(portNumber));
        }
    }
}