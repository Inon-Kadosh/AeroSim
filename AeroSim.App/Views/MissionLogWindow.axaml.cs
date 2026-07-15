using System.Collections.ObjectModel;
using AeroSim.App.ViewModels;
using AeroSim.Core.Models;
using Avalonia.Controls;

namespace AeroSim.App.Views;

public partial class MissionLogWindow : Window
{
    public MissionLogWindow()
    {
        InitializeComponent();
    }

    public MissionLogWindow(
        ObservableCollection<MissionEvent> events)
        : this()
    {
        DataContext =
            new MissionLogViewModel(events);
    }
}