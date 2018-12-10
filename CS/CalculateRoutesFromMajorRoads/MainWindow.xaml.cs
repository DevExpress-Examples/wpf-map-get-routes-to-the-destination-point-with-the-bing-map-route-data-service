using DevExpress.Xpf.Map;
using DevExpress.Xpf.Ribbon;
using System;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CalculateRoutesFromMajorRoads {
    public partial class MainWindow : DXRibbonWindow {
        public Point MouseDownPosition { get; set; }

        public MainWindow() {
            InitializeComponent();
        }

        private void OnMapControlMouseDown(object sender, MouseButtonEventArgs e) {
            MouseDownPosition = e.GetPosition(mapControl);
        }

        #region #CalculateRoutesFromMajorRoads
        private void OnMapControlMouseUp(object sender, MouseButtonEventArgs e) {
            Point mouseUpPosition = e.GetPosition(mapControl);
            if (IsScrolled(mouseUpPosition)) return;
            GeoPoint point = mapControl.ScreenPointToCoordPoint(mouseUpPosition) as GeoPoint;
            CalculateRoutesFromMajorRoads(point);
        }

        private bool IsScrolled(Point mouseUpPosition) {
            var positionDistance =
                  (MouseDownPosition.X - mouseUpPosition.X)
                * (MouseDownPosition.X - mouseUpPosition.X)
                + (MouseDownPosition.Y - mouseUpPosition.Y)
                * (MouseDownPosition.Y - mouseUpPosition.Y);
            return positionDistance >= 4;
        }

        private void CalculateRoutesFromMajorRoads(GeoPoint point) {
            targetLocationPushpin.Location = point;
            targetLocationPushpin.Visible = true;
            routeProvider.CalculateRoutesFromMajorRoads(new RouteWaypoint(String.Empty, point));
            bsiState.Content = "Started to calculate routes from major roads.";
        }
        #endregion #CalculateRoutesFromMajorRoads

        #region #RouteCalculated
        private void routeProvider_RouteCalculated(object sender, BingRouteCalculatedEventArgs e) {
            tbResults.Text = ParseRouteCalculationResult(e.CalculationResult);
            bsiState.Content = "Ready.";
        }

        string ParseRouteCalculationResult(RouteCalculationResult result) {
            StringBuilder sb = new StringBuilder();
            sb.Append("Status: ").Append(result.ResultCode).Append("\n");
            if (result.ResultCode != RequestResultCode.Success) {
                sb.Append("Fault reason: ").Append(result.FaultReason).Append("\n");
            } else {
                sb.Append("Intermediate Points:\n");
                for (int i = 0; i < result.IntermediatePoints.Count; i++) {
                    AppendIntermediatePointString(sb, i, result.IntermediatePoints[i]);
                }
                sb.Append("Route Results:\n");
                for (int i = 0; i < result.RouteResults.Count; i++) {
                    AppendRouteResultString(sb, i, result.RouteResults[i]);
                }
            }
            return sb.ToString();
        }
        #endregion #RouteCalculated

        #region #ProcessStartingPoints
        void AppendIntermediatePointString(StringBuilder sb, int index, RouteWaypoint point) {
            sb.Append(index + 1).Append("\tLocation: ").Append(point.Location)
                .Append("\n\tDescription: ").Append(point.Description)
                .Append("\n");
        }
        #endregion #ProcessStartingPoints

        #region #ProcessRouteResults
        void AppendRouteResultString(StringBuilder sb, int index, BingRouteResult routeResult) {
            sb.Append(index + 1).Append("\tDistance: ").Append(routeResult.Distance)
                    .Append("\n\tTime: ").Append(routeResult.Time)
                    .Append("\n\tLegs:\n");
            for (int i = 0; i < routeResult.Legs.Count; i++) {
                AppendLegString(sb, i, routeResult.Legs[i]);
            }
        }

        #endregion #ProcessRouteResults

        #region #ProcessLegs
        void AppendLegString(StringBuilder sb, int index, BingRouteLeg leg) {
            sb.Append("\t").Append(index + 1).Append("\t").Append("Start: ").Append(leg.StartPoint)
                .Append("\n\t\t").Append("End: ").Append(leg.EndPoint)
                .Append("\n\t\tDistance: ").Append(leg.Distance)
                .Append("\n\t\tTime: ").Append(leg.Time)
                .Append("\n\t\tInternary:")
                .Append("\n");
            for (int i = 0; i < leg.Itinerary.Count; i++) {
                AppendInternaryString(sb, i, leg.Itinerary[i]);
            }
        }
        #endregion #ProcessLegs

        #region #ProcessItinerary
        void AppendInternaryString(StringBuilder sb, int index, BingItineraryItem internary) {
            sb.Append("\t\t").Append(index + 1).Append("\t").Append("Maneuver: ").Append(internary.Maneuver)
                .Append("\n\t\t\tLocation: ").Append(internary.Location)
                .Append("\n\t\t\tInstructions: ").Append(internary.ManeuverInstruction)
                .Append("\n");
            if (internary.Warnings.Count > 0) {
                sb.Append("\t\t\tWarnings:\n");
                for (int i = 0; i < internary.Warnings.Count; i++) {
                    AppendWarningString(sb, i, internary.Warnings[i]);
                }
            }
        }
        #endregion #ProcessItinerary

        #region #ProcessWarnings
        void AppendWarningString(StringBuilder sb, int index, BingItineraryItemWarning warning) {
            sb.Append("\t\t\t").Append(index + 1).Append("\tType: ").Append(warning.Type)
                .Append("\n\t\t\t\tText: ").Append(warning.Text).Append("\n");
        }
        #endregion #ProcessWarnings

        private void routeProvider_LayerItemsGenerating(object sender, LayerItemsGeneratingEventArgs args) {
            mapControl.ZoomToFit(args.Items);
        }
    }
}