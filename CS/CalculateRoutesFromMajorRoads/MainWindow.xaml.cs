using DevExpress.Xpf.Map;
using System;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CalculateRoutesFromMajorRoads {

    public partial class MainWindow : Window {
        

        public MainWindow() {
            InitializeComponent();
        }

        #region #calculateRoutesClick
        double latitude;
        double longitude;
        string destination;
        BingRouteOptions options = new BingRouteOptions();
        
        private void calculateRoutes_Click(object sender, RoutedEventArgs e) {
            if (GetCalculateRoutesArguments()) {
                CalculateMajorRoutes();
            }
        }

        private bool GetCalculateRoutesArguments() {
            latitude = String.IsNullOrEmpty(tbLatitude.Text) ? 0 : Double.Parse(tbLatitude.Text);
            if ((latitude > 90) || (latitude < -90)) {
                MessageBox.Show("Latitude must be less than or equal to 90 and greater than or equal to - 90. Please, correct the input value.");
                return false;
            }

            longitude = String.IsNullOrEmpty(tbLongitude.Text) ? 0 : Double.Parse(tbLongitude.Text);
            if ((longitude > 180) || (longitude < -180)) {
                MessageBox.Show("Longitude must be less than or equal to 180 and greater than or equal to - 180. Please, correct the input value.");
                return false;
            }
            
            destination = tbDestination.Text;

            if (cbMode.SelectedIndex == 0)
                options.Mode = BingTravelMode.Driving;
            else
                options.Mode = BingTravelMode.Walking;

            if (cbOptimize.SelectedIndex == 0)
                options.RouteOptimization = BingRouteOptimization.MinimizeTime;
            else
                options.RouteOptimization = BingRouteOptimization.MinimizeDistance;

            return true;
        }

        private void CalculateMajorRoutes() {
            CalculateMajorRouteRequest(destination, new GeoPoint(latitude, longitude), options);
        }

        private void CalculateMajorRouteRequest(string destination, GeoPoint location, BingRouteOptions options) {
            try {
                #region #CalculateMajorRoutes
                routeProvider.RouteOptions = options;
                routeProvider.CalculateRoutesFromMajorRoads(new RouteWaypoint(destination, location), 2.0);
                #endregion #CalculateMajorRoutes
            }
            catch (Exception ex) {
                MessageBox.Show("An error occurs: " + ex.ToString());
            }
        }
        #endregion #calculateRoutesClick

        private void routeDataProvider_RouteCalculated(object sender, BingRouteCalculatedEventArgs e) {
            mapControl1.CenterPoint = new GeoPoint(latitude, longitude);
            mapControl1.ZoomLevel = 10;
            RouteCalculationResult result = e.CalculationResult;
            StringBuilder resultList = new StringBuilder("");
            resultList.Append(String.Format("Status: {0}\n", result.ResultCode));
            resultList.Append(String.Format("Fault reason: {0}\n", result.FaultReason));

            if (result.StartingPoints != null) {
                resultList.Append(String.Format("_________________________\n"));
                int i = 1;
                foreach (RouteWaypoint startingPoint in result.StartingPoints)
                    resultList.Append(String.Format("Starting point {0}: {1} ({2})\n", i++,
                        startingPoint.Description, startingPoint.Location));
            }

            if (result.RouteResults != null) {
                int rnum = 1;
                foreach (BingRouteResult routeResult in result.RouteResults) {
                    resultList.Append(String.Format("_________________________\n"));
                    resultList.Append(String.Format("Path {0}:\n", rnum++));
                    resultList.Append(String.Format("Distance: {0}\n", routeResult.Distance));
                    resultList.Append(String.Format("Time: {0}\n", routeResult.Time));

                    MapPolyline path = new MapPolyline();
                    path.Stroke = new SolidColorBrush(Colors.Red);
                    path.StrokeStyle = new StrokeStyle() { Thickness = 2 };
                    foreach (GeoPoint point in routeResult.RoutePath)
                        path.Points.Add(point);

                    if (routeResult.Legs != null) {
                        int legNum = 1;
                        foreach (BingRouteLeg leg in routeResult.Legs) {
                            resultList.Append(String.Format("\tLeg {0}:\n", legNum++));
                            resultList.Append(String.Format("\tStart: {0}\n", leg.StartPoint));
                            resultList.Append(String.Format("\tEnd: {0}\n", leg.EndPoint));
                            resultList.Append(String.Format("\tDistance: {0}\n", leg.Distance));
                            resultList.Append(String.Format("\tTime: {0}\n", leg.Time));
                            if (leg.Itinerary != null) {
                                int itNum = 1;
                                foreach (BingItineraryItem itineraryItem in leg.Itinerary) {
                                    resultList.Append(String.Format("\t\tItinerary {0}:\n", itNum++));
                                    resultList.Append(String.Format("\t\tManeuver: {0}\n", itineraryItem.Maneuver));
                                    resultList.Append(String.Format("\t\tLocation: {0}\n", itineraryItem.Location));
                                    resultList.Append(String.Format("\t\tInstructions: {0}\n", itineraryItem.ManeuverInstruction));
                                    int warnNum = 1;
                                    foreach (BingItineraryItemWarning warning in itineraryItem.Warnings) {
                                        resultList.Append(String.Format("\t\t\tWarning {0}:\n", warnNum++));
                                        resultList.Append(String.Format("\t\t\tType: {0}\n", warning.Type));
                                        resultList.Append(String.Format("\t\t\tText: {0}\n", warning.Text));
                                    }
                                }
                            }
                            if (leg.StartPoint != null) {
                                MapDot start = new MapDot();
                                start.Size = 10;
                                start.Location = leg.StartPoint;

                            }
                            if (leg.EndPoint != null) {
                                MapDot end = new MapDot();
                                end.Size = 15;
                                end.Location = leg.EndPoint;

                            }
                        }
                    }
                }
            }

            tbResults.Text = resultList.ToString();
        }

    }
}

