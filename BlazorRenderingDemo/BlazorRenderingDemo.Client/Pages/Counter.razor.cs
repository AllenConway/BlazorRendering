using Microsoft.AspNetCore.Components;

namespace BlazorRenderingDemo.Client.Pages
{
    public partial class Counter
    {
        // Demo talking point: [Parameter] lets a PARENT component pass a value in,
        // e.g. <Counter InitialCount="5" />. Because Counter is a routable @page,
        // no parent sets it here, so it stays at its default (0). To make it change,
        // wire it to the route: @page "/counter/{InitialCount:int?}".
        [Parameter]
        public int InitialCount { get; set; } = 0;

        private int currentCount = 0;
        private bool isDelayedCounterRunning = false;

        // Lifecycle tracking
        private int onInitializedCount = 0;
        private int onParametersSetCount = 0;
        private int onAfterRenderCount = 0;
        private string lastOnInitialized = "";
        private string lastOnParametersSet = "";
        private string lastOnAfterRender = "";

        protected override void OnInitialized()
        {
            base.OnInitialized();
            onInitializedCount++;
            lastOnInitialized = DateTime.Now.ToString("HH:mm:ss.fff");
            
            // Set initial count from parameter
            currentCount = InitialCount;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            onParametersSetCount++;
            lastOnParametersSet = DateTime.Now.ToString("HH:mm:ss.fff");
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            onAfterRenderCount++;
            lastOnAfterRender = DateTime.Now.ToString("HH:mm:ss.fff");

            // Need to trigger re-render to show updated count
            if (firstRender)
            {
                StateHasChanged();
            }
        }

        private void IncrementCount()
        {
            currentCount++;
            // StateHasChanged() is automatic in event handlers like @onclick
            // This will trigger OnParametersSet and OnAfterRender!
        }

        private async Task IncrementCountDelayed()
        {
            if (isDelayedCounterRunning) return;

            isDelayedCounterRunning = true;
            // Update UI to show spinner
            StateHasChanged(); 

            // Background task StateHasChanged() is NOT automatic here
            await Task.Delay(3000);
            currentCount++;

            // Without this line, the UI won't update
            StateHasChanged();

            isDelayedCounterRunning = false;
        }
    }
}
