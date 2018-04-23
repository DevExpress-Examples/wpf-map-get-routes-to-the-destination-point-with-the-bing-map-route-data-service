Imports DevExpress.Xpf.Map
Imports System
Imports System.Text
Imports System.Windows
Imports System.Windows.Media

Namespace CalculateRoutesFromMajorRoads

    Partial Public Class MainWindow
        Inherits Window


        Public Sub New()
            InitializeComponent()
        End Sub

        #Region "#calculateRoutesClick"
        Private latitude As Double
        Private longitude As Double
        Private destination As String
        Private options As New BingRouteOptions()

        Private Sub calculateRoutes_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If GetCalculateRoutesArguments() Then
                CalculateMajorRoutes()
            End If
        End Sub

        Private Function GetCalculateRoutesArguments() As Boolean
            latitude = If(String.IsNullOrEmpty(tbLatitude.Text), 0, Double.Parse(tbLatitude.Text))
            If (latitude > 90) OrElse (latitude < -90) Then
                MessageBox.Show("Latitude must be less than or equal to 90 and greater than or equal to - 90. Please, correct the input value.")
                Return False
            End If

            longitude = If(String.IsNullOrEmpty(tbLongitude.Text), 0, Double.Parse(tbLongitude.Text))
            If (longitude > 180) OrElse (longitude < -180) Then
                MessageBox.Show("Longitude must be less than or equal to 180 and greater than or equal to - 180. Please, correct the input value.")
                Return False
            End If

            destination = tbDestination.Text

            If cbMode.SelectedIndex = 0 Then
                options.Mode = BingTravelMode.Driving
            Else
                options.Mode = BingTravelMode.Walking
            End If

            If cbOptimize.SelectedIndex = 0 Then
                options.RouteOptimization = BingRouteOptimization.MinimizeTime
            Else
                options.RouteOptimization = BingRouteOptimization.MinimizeDistance
            End If

            Return True
        End Function

        Private Sub CalculateMajorRoutes()
            CalculateMajorRouteRequest(destination, New GeoPoint(latitude, longitude), options)
        End Sub

        Private Sub CalculateMajorRouteRequest(ByVal destination As String, ByVal location As GeoPoint, ByVal options As BingRouteOptions)
            Try
'                #Region "#CalculateMajorRoutes"
                routeProvider.RouteOptions = options
                routeProvider.CalculateRoutesFromMajorRoads(New RouteWaypoint(destination, location), 2.0)
'                #End Region ' #CalculateMajorRoutes
            Catch ex As Exception
                MessageBox.Show("An error occurs: " & ex.ToString())
            End Try
        End Sub
        #End Region ' #calculateRoutesClick

        Private Sub routeDataProvider_RouteCalculated(ByVal sender As Object, ByVal e As BingRouteCalculatedEventArgs)
            mapControl1.CenterPoint = New GeoPoint(latitude, longitude)
            mapControl1.ZoomLevel = 10
            Dim result As RouteCalculationResult = e.CalculationResult
            Dim resultList As New StringBuilder("")
            resultList.Append(String.Format("Status: {0}" & ControlChars.Lf, result.ResultCode))
            resultList.Append(String.Format("Fault reason: {0}" & ControlChars.Lf, result.FaultReason))

            If result.StartingPoints IsNot Nothing Then
                resultList.Append(String.Format("_________________________" & ControlChars.Lf))
                Dim i As Integer = 1
                For Each startingPoint As RouteWaypoint In result.StartingPoints
                    resultList.Append(String.Format("Starting point {0}: {1} ({2})" & ControlChars.Lf, i, startingPoint.Description, startingPoint.Location))
                Next startingPoint
                    i += 1
            End If

            If result.RouteResults IsNot Nothing Then
                Dim rnum As Integer = 1
                For Each routeResult As BingRouteResult In result.RouteResults
                    resultList.Append(String.Format("_________________________" & ControlChars.Lf))
                    resultList.Append(String.Format("Path {0}:" & ControlChars.Lf, rnum))
                    rnum += 1
                    resultList.Append(String.Format("Distance: {0}" & ControlChars.Lf, routeResult.Distance))
                    resultList.Append(String.Format("Time: {0}" & ControlChars.Lf, routeResult.Time))

                    Dim path As New MapPolyline()
                    path.Stroke = New SolidColorBrush(Colors.Red)
                    path.StrokeStyle = New StrokeStyle() With {.Thickness = 2}
                    For Each point As GeoPoint In routeResult.RoutePath
                        path.Points.Add(point)
                    Next point

                    If routeResult.Legs IsNot Nothing Then
                        Dim legNum As Integer = 1
                        For Each leg As BingRouteLeg In routeResult.Legs
                            resultList.Append(String.Format(ControlChars.Tab & "Leg {0}:" & ControlChars.Lf, legNum))
                            legNum += 1
                            resultList.Append(String.Format(ControlChars.Tab & "Start: {0}" & ControlChars.Lf, leg.StartPoint))
                            resultList.Append(String.Format(ControlChars.Tab & "End: {0}" & ControlChars.Lf, leg.EndPoint))
                            resultList.Append(String.Format(ControlChars.Tab & "Distance: {0}" & ControlChars.Lf, leg.Distance))
                            resultList.Append(String.Format(ControlChars.Tab & "Time: {0}" & ControlChars.Lf, leg.Time))
                            If leg.Itinerary IsNot Nothing Then
                                Dim itNum As Integer = 1
                                For Each itineraryItem As BingItineraryItem In leg.Itinerary
                                    resultList.Append(String.Format(ControlChars.Tab & ControlChars.Tab & "Itinerary {0}:" & ControlChars.Lf, itNum))
                                    itNum += 1
                                    resultList.Append(String.Format(ControlChars.Tab & ControlChars.Tab & "Maneuver: {0}" & ControlChars.Lf, itineraryItem.Maneuver))
                                    resultList.Append(String.Format(ControlChars.Tab & ControlChars.Tab & "Location: {0}" & ControlChars.Lf, itineraryItem.Location))
                                    resultList.Append(String.Format(ControlChars.Tab & ControlChars.Tab & "Instructions: {0}" & ControlChars.Lf, itineraryItem.ManeuverInstruction))
                                    Dim warnNum As Integer = 1
                                    For Each warning As BingItineraryItemWarning In itineraryItem.Warnings
                                        resultList.Append(String.Format(ControlChars.Tab & ControlChars.Tab & ControlChars.Tab & "Warning {0}:" & ControlChars.Lf, warnNum))
                                        warnNum += 1
                                        resultList.Append(String.Format(ControlChars.Tab & ControlChars.Tab & ControlChars.Tab & "Type: {0}" & ControlChars.Lf, warning.Type))
                                        resultList.Append(String.Format(ControlChars.Tab & ControlChars.Tab & ControlChars.Tab & "Text: {0}" & ControlChars.Lf, warning.Text))
                                    Next warning
                                Next itineraryItem
                            End If
                            If leg.StartPoint <> Nothing Then
                                Dim start As New MapDot()
                                start.Size = 10
                                start.Location = leg.StartPoint

                            End If
                            If leg.EndPoint <> Nothing Then
                                Dim [end] As New MapDot()
                                [end].Size = 15
                                [end].Location = leg.EndPoint

                            End If
                        Next leg
                    End If
                Next routeResult
            End If

            tbResults.Text = resultList.ToString()
        End Sub

    End Class
End Namespace

