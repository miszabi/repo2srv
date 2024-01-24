using System;
namespace repo2srv
{
    public class Root
    {
        public IEnumerable<AppSetting> Apps { get; set; }
    }

    public class Action
    {
        public List<string> RemoveFiles { get; set; }        
    }

    public class AppSetting
    {
        public string Name { get; set; }
        public List<Action> Actions { get; set; }
        public string SourcePath { get; set; }
        public List<string> DestinationPaths { get; set; }
    }    
}
