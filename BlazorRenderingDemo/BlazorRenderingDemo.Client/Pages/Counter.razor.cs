using Microsoft.AspNetCore.Components;

namespace BlazorRenderingDemo.Client.Pages
{
    public partial class Counter : IDisposable
    {
        // [Parameter] lets a PARENT component pass a value in,
        // e.g. <Counter InitialCount="5" />. Because Counter is a routable @page,
        // no parent sets it here, so it stays at its default (0). To make it change,
        // wire it to the route: @page "/counter/{InitialCount:int?}".
        [Parameter]
        public int InitialCount { get; set; } = 0;

        private int currentCount = 0;

        // Example only: If prerendering had loaded expensive data that the interactive instance
        // needed again, .NET 10’s [PersistentState] could carry that state across the
        // prerender-to-interactive boundary.
        // [PersistentState]
        // public int PersistedCount { get; set; }

        private bool isDelayedCounterRunning = false;

        // Drives the live countdown shown inside the delayed button
        private System.Timers.Timer? countdownTimer;
        private int secondsRemaining = 0;

        // Lifecycle tracking
        private int onInitializedCount = 0;
        private int onParametersSetCount = 0;
        private int onAfterRenderCount = 0;
        private string lastOnInitialized = "";
        private string lastOnParametersSet = "";
        private string lastOnAfterRender = "";

        // Bound to the "Suppress renders" checkbox, read by ShouldRender()
        private bool suppressRenders;

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

        // Runs AFTER the DOM has been updated. Unlike OnInitialized/OnParametersSet,
        // Blazor does NOT auto-render after this hook - doing so would loop forever.
        // So state changed here is invisible until something else renders, which is why
        // this hook is meant for JS interop / DOM work rather than state changes.
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            onAfterRenderCount++;
            lastOnAfterRender = DateTime.Now.ToString("HH:mm:ss.fff");

            // Flush that first increment so the badge is not blank on load. The
            // firstRender guard is mandatory: StateHasChanged() renders, which calls
            // OnAfterRender again. Unguarded, that is an infinite render loop.
            // Cost of this one safe extra render: OnAfterRender runs 2x on load.
            if (firstRender)
            {
                StateHasChanged();
            }
        }

        private void IncrementCount()
        {
            currentCount++;
            // StateHasChanged() is automatic in event handlers like @onclick, so this
            // re-renders and triggers OnAfterRender. OnParametersSet does NOT re-run:
            // no parameters changed, only internal state.
        }

        // IN BAND: Blazor awaits the Task returned by this handler, so it calls
        // StateHasChanged() for us twice - once before the first await (spinner
        // appears) and once when the Task completes (new count appears).
        // That is why there are no StateHasChanged() calls in this method.
        private async Task IncrementCountDelayed()
        {
            if (isDelayedCounterRunning) return;

            isDelayedCounterRunning = true;
            secondsRemaining = 3;

            StartCountdownTimer();

            try
            {
                await Task.Delay(3000);
                currentCount++;
            }
            finally
            {
                StopCountdownTimer();
                isDelayedCounterRunning = false;
            }
        }

        // OUT OF BAND: the Elapsed event is raised by the timer, not by Blazor.
        // Blazor has no idea state changed, so nothing re-renders unless we ask.
        private void StartCountdownTimer()
        {
            countdownTimer = new System.Timers.Timer(1000) { AutoReset = true };

            countdownTimer.Elapsed += async (_, _) =>
            {
                secondsRemaining--;

                // InvokeAsync is required (not just StateHasChanged) because Elapsed
                // fires on a thread pool thread. Rendering must happen on the
                // renderer's dispatcher, and InvokeAsync marshals us onto it.
                // Swap in a bare StateHasChanged() to see:
                //   Blazor Server -> InvalidOperationException (wrong thread)
                //   WebAssembly   -> works today only because WASM is single threaded
                // Remove the line entirely and the countdown freezes at 3.
                await InvokeAsync(StateHasChanged);
            };

            countdownTimer.Start();
        }

        private void StopCountdownTimer()
        {
            countdownTimer?.Stop();
            countdownTimer?.Dispose();
            countdownTimer = null;
        }

        // A running timer would keep this component alive after navigating away.
        public void Dispose() => StopCountdownTimer();

        // ShouldRender() is the last gate before the render tree is built: return false
        // and the render is skipped entirely, even though StateHasChanged() was called.
        // Real use is perf (skipping expensive diffs). Here the checkbox drives it so you
        // can watch the UI freeze while the counter and countdown keep changing underneath.
        // Note: the FIRST render always happens - this gate only applies to re-renders.
        protected override bool ShouldRender() => !suppressRenders;
    }
}
