using System.Collections.ObjectModel;
using AeroSim.Core.Models;

namespace AeroSim.App.ViewModels;

public sealed class MissionLogViewModel : ViewModelBase
{
    public MissionLogViewModel(
        ObservableCollection<MissionEvent> events)
    {
        Events = events;
    }

    public ObservableCollection<MissionEvent> Events { get; }
}