using DevExpress.Xpf.Map;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CalculateRoutesFromMajorRoads {
    public partial class MainWindow : Window {
        public GeoPoint location = new GeoPoint(40, -120);

        public MainWindow() {
            InitializeComponent();
            this.DataContext = location;
        }
        
        #region #RouteCalculated
        private void routeDataProvider_RouteCalculated(object sender, BingRouteCalculatedEventArgs e) {
            RouteCalculationResult result = e.CalculationResult;
            
            StringBuilder resultList = new StringBuilder();
            resultList.Append(String.Format("Status: {0}\n", result.ResultCode));
            resultList.Append(String.Format("Fault reason: {0}\n", result.FaultReason));
            resultList.Append(ProcessIntermediatePoints(result.IntermediatePoints));
            resultList.Append(ProcessRouteResults(result.RouteResults));
            
            tbResults.Text = resultList.ToString();
        }
        #endregion #RouteCalculated

        #region #ProcessStartingPoints
        string ProcessIntermediatePoints(List<RouteWaypoint> points) {
            if (points == null) return "";

            StringBuilder sb = new StringBuilder("Intermediate Points:\n"); 
            sb.Append(String.Format("_________________________\n"));
            for (int i = 0; i < points.Count; ++i)
                    sb.Append(String.Format(
                        "Intermediate point {0}: {1} ({2})\n", 
                        i+1,
                        points[i].Description, 
                        points[i].Location
                    ));
            return sb.ToString();
        }
        #endregion #ProcessStartingPoints

        #region #ProcessRouteResults
        string ProcessRouteResults(List<BingRouteResult> results) {
            if (results == null) return "";

            StringBuilder sb = new StringBuilder("RouteResults:\n");
            for (int i = 0; i < results.Count; i++) {
                sb.Append(String.Format("_________________________\n"));
                sb.Append(String.Format("Path {0}:\n", i+1));
                sb.Append(String.Format("Distance: {0}\n", results[i].Distance));
                sb.Append(String.Format("Time: {0}\n", results[i].Time));
                sb.Append(ProcessLegs(results[i].Legs));
            }
            return sb.ToString();
        }
        #endregion #ProcessRouteResults

        #region #ProcessLegs
        string ProcessLegs(List<BingRouteLeg> legs) {
            if (legs == null) return "";
            
            StringBuilder sb = new StringBuilder("Legs:\n");    
            for (int i = 0; i < legs.Count; i++) {
                sb.Append(String.Format("\tLeg {0}:\n", i+1));
                sb.Append(String.Format("\tStart: {0}\n", legs[i].StartPoint));
                sb.Append(String.Format("\tEnd: {0}\n", legs[i].EndPoint));
                sb.Append(String.Format("\tDistance: {0}\n", legs[i].Distance));
                sb.Append(String.Format("\tTime: {0}\n", legs[i].Time));
                sb.Append(ProcessItinerary(legs[i].Itinerary));
            }
            return sb.ToString();
        }
        #endregion #ProcessLegs

        #region #ProcessItinerary
        string ProcessItinerary(List<BingItineraryItem> items) {
            if (items == null) return "";
            
            StringBuilder sb = new StringBuilder("\tInternary Items:\n");
            for (int i = 0; i < items.Count; i++) {
                sb.Append(String.Format("\t\tItinerary {0}:\n", i+1));
                sb.Append(String.Format("\t\tManeuver: {0}\n", items[i].Maneuver));
                sb.Append(String.Format("\t\tLocation: {0}\n", items[i].Location));
                sb.Append(String.Format("\t\tInstructions: {0}\n", items[i].ManeuverInstruction));
                sb.Append(ProcessWarnings(items[i].Warnings));
            }
            return sb.ToString();
        }
        #endregion #ProcessItinerary

        #region #ProcessWarnings
        string ProcessWarnings(List<BingItineraryItemWarning> warnings) {
            if (warnings == null) return "";

            StringBuilder sb = new StringBuilder("\t\tWarnings:\n");
            for (int i = 0; i < warnings.Count; i++) {
                sb.Append(String.Format("\t\t\tWarning {0}:\n", i + 1));
                sb.Append(String.Format("\t\t\tType: {0}\n", warnings[i].Type));
                sb.Append(String.Format("\t\t\tText: {0}\n", warnings[i].Text));

            }
            return sb.ToString();
        }
        #endregion #ProcessWarnings

        #region #CalculateRoutesFromMajorRoads
        private void calculateRoutes_Click(object sender, RoutedEventArgs e) {
            routeProvider.CalculateRoutesFromMajorRoads(new RouteWaypoint(tbDestination.Text, location));
        }
        #endregion #CalculateRoutesFromMajorRoads

        private void ValidationError(object sender, ValidationErrorEventArgs e) {
            if (e.Action == ValidationErrorEventAction.Added)
                MessageBox.Show(e.Error.ErrorContent.ToString());
        }
    }

    class RangeDoubleValidationRule : ValidationRule {
        double min = Double.MinValue;
        double max = Double.MaxValue;
        public double Min {
            get { return min; }
            set {
                if ((value != min) && (value <=max))
                    min = value;
            }
        }
        public double Max {
            get { return max; }
            set {
                if ((value != max) && (value >= min))
                    max = value;
            }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) {
            double d;
            if (!Double.TryParse(value as string, out d))
                return new ValidationResult(false, "Input value should be floating point number.");

            if ((d < min) || (d > max))
                return new ValidationResult(
                    false, 
                    String.Format(
                        "Input value should be larger than or equals to {0} and less than or equals to {1}", 
                        min, 
                        max 
                    )
                );

            return new ValidationResult(true, null);
        }
    }

}