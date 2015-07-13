using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using MHGameWork.TheWizards.Profiling;

namespace MHGameWork.TheWizards.Diagnostics.Profiling
{
    /// <summary>
    /// Responsible for providing a pluggable diagnostics component for engine debugging
    /// </summary>
    public class ProfilerComponent : IDiagnosticsComponent
    {
        private ProfilerDisplay display;

        private Dictionary<ProfilingPoint, ProfilingNode> nodes = new Dictionary<ProfilingPoint, ProfilingNode>();
        private ProfilingPoint rootPoint;

        public ProfilerComponent()
        {
            display = new ProfilerDisplay();
            // add some buttons
            addDisplayButtons();

        }

        private void addDisplayButtons()
        {
            display.ViewModel.Buttons.Clear();

            display.ViewModel.Buttons.Add(new ProfilerDisplayModel.ProfilerCommand("Start", o => StartMeasurement()));
            display.ViewModel.Buttons.Add(new ProfilerDisplayModel.ProfilerCommand("End", o => EndMeasurement()));
            var snapshotCommand = new ProfilerDisplayModel.ProfilerCommand("Snapshot", delegate
            {
                try { TakeSnapshot(); }
                catch (Exception e) { Console.WriteLine(e); }
            });
            display.ViewModel.Buttons.Add(snapshotCommand);
            display.ViewModel.Buttons.Add(new ProfilerDisplayModel.ProfilerCommand("Enable", o => EnableProfiling()));
            display.ViewModel.Buttons.Add(new ProfilerDisplayModel.ProfilerCommand("Disable", o => DisableProfiling()));
            display.ViewModel.Buttons.Add(new ProfilerDisplayModel.ProfilerCommand("Select",
                                                                                   delegate
                                                                                   {
                                                                                       if (display.ViewModel.SelectedItem == null)
                                                                                           return;
                                                                                       SetMeasurementPoint(display.ViewModel.SelectedItem.ProfilingPoint as ProfilingPoint);
                                                                                   }));
            display.ViewModel.Buttons.Add(new ProfilerDisplayModel.ProfilerCommand("Reset", o => ResetMeasurementPoint()));
        }

        public ProfilingPoint MeasurementPoint { get; private set; }

        /// <summary>
        /// Runs in a different thread!
        /// </summary>
        /// <param name="rootPoint"></param>
        public void Update(ProfilingPoint rootPoint)
        {
            if (Thread.CurrentThread != display.Dispatcher.Thread)
            {
                display.Dispatcher.Invoke(new Action(() => Update(rootPoint)));
                return;
            }


            resetAll();
            updateRecursive(rootPoint);
            if (display.ViewModel.BaseLevel[0] != nodes[rootPoint])
                display.ViewModel.SetRoot(nodes[rootPoint]);


        }

        private void resetAll()
        {
            foreach (var node in nodes.Values)
                node.Duration = 0;
        }

        private void updateRecursive(ProfilingPoint p)
        {
            var node = getOrCreateNode(p);
            node.Children.Clear();

            node.Duration = (float)Math.Round(p.TotalTime * 1000, 1);
            node.Percentage = (float)Math.Round(p.TotalTime / MeasurementPoint.TotalTime * 100, 1);
            node.Name = p.Name;
            node.ProfilingPoint = p;



            foreach (var child in p.NonRecursiveChildren)
            {
                var cNode = getOrCreateNode(child);
                cNode.Duration += p.TotalTime;

                if (!node.Children.Contains(cNode))
                    node.Children.Add(cNode);
                updateRecursive(child);
            }
        }

        private ProfilingNode getOrCreateNode(ProfilingPoint rootPoint)
        {
            if (nodes.ContainsKey(rootPoint)) return nodes[rootPoint];
            var node = new ProfilingNode();
            node.Name = rootPoint.Name;
            nodes.Add(rootPoint, node);
            return node;
        }

        public Control GetView()
        {
            return display;
        }

        public void ShowResultsFrom(ProfilingPoint root)
        {
            Update(root);
        }

        public void SetMeasurementPoint(ProfilingPoint p)
        {
            MeasurementPoint = p;
        }

        public void StartMeasurement()
        {
            Profiler.ResetAll();
        }

        public void EndMeasurement()
        {
            ShowResultsFrom(MeasurementPoint);
        }

        private bool inSnapshot;
        private bool waitingForEnd = false;
        private ProfilingPoint snapshotPoint;
        public void TakeSnapshot()
        {
            if (inSnapshot) throw new InvalidOperationException();
            inSnapshot = true;
            waitingForEnd = true;
            snapshotPoint = MeasurementPoint;
            snapshotPoint.Ended += onSnapshotEnded;
        }
        private void onSnapshotEnded()
        {
            if (waitingForEnd)
            {
                waitingForEnd = false;
                Profiler.ResetAll();
                return;
            }
            snapshotPoint.Ended -= onSnapshotEnded;
            ShowResultsFrom(snapshotPoint);
            inSnapshot = false;
        }

        public void DisableProfiling()
        {
            Profiler.SetProfilingEnabled(false);
        }

        public void EnableProfiling()
        {
            Profiler.SetProfilingEnabled(true);
        }

        public void SetRootPoint(ProfilingPoint p1)
        {
            rootPoint = p1;
            SetMeasurementPoint(p1);
        }

        public void ResetMeasurementPoint()
        {
            SetMeasurementPoint(rootPoint);
        }
    }
}
