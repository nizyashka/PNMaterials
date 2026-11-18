using System;
using System.Collections.Generic;
using System.Text;

namespace PNMaterialsClient.Services;

public partial class LoadingService : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBusy))]
    private int _activeOperations;

    public bool IsBusy => ActiveOperations > 0;

    public void StartOperation() => ActiveOperations++;
    public void EndOperation() => ActiveOperations--;
}
