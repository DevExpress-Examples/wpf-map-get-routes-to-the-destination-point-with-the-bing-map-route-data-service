Imports DevExpress.Xpf.Map
Imports System
Imports System.Text
Imports System.Windows
Imports System.Windows.Media

Namespace CalculateRoutesFromMajorRoads

    Public Partial Class MainWindow
        Inherits Window

        Public Sub New()
            Me.InitializeComponent()
        End Sub

'#Region "#calculateRoutesClick"
        Private latitude As Double

        Private longitude As Double

        Private destination As String

        Private options As BingRouteOptions = New BingRouteOptions()

        Private Sub calculateRoutes_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If GetCalculateRoutesArguments() Then
                CalculateMajorRoutes()
            End If
        End Sub

        Private Function GetCalculateRoutesArguments() As Boolean
            latitude = If(String.IsNullOrEmpty(Me.tbLatitude.Text), 0, Double.Parse(Me.tbLatitude.Text))
            If latitude > 90 OrElse latitude < -90 Then
                MessageBox.Show("Latitude must be less than or equal to 90 and greater than or equal to - 90. Please, correct the input value.")
                Return False
            End If

            longitude = If(String.IsNullOrEmpty(Me.tbLongitude.Text), 0, Double.Parse(Me.tbLongitude.Text))
            If longitude > 180 OrElse longitude < -180 Then
                MessageBox.Show("Longitude must be less than or equal to 180 and greater than or equal to - 180. Please, correct the input value.")
                Return False
            End If

            destination = Me.tbDestination.Text
            If Me.cbMode.SelectedIndex = 0 Then
                options.Mode = BingTravelMode.Driving
            Else
                options.Mode = BingTravelMode.Walking
            End If

            If Me.cbOptimize.SelectedIndex = 0 Then
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
'#Region "#CalculateMajorRoutes"
                Me.routeProvider.RouteOptions = options
'#End Region  ' #CalculateMajorRoutes
                Me.routeProvider.CalculateRoutesFromMajorRoads(New RouteWaypoint(destination, location), 2.0)
            Catch ex As Exception
                Call MessageBox.Show("An error occurs: " & ex.ToString())
            End Try
        End Sub

'#End Region  ' #calculateRoutesClick
        Private Sub routeDataProvider_RouteCalculated(ByVal sender As Object, ByVal e As BingRouteCalculatedEventArgs)
            Me.mapControl1.CenterPoint = New GeoPoint(latitude, longitude)
            Me.mapControl1.ZoomLevel = 10
            Dim result As RouteCalculationResult = e.CalculationResult
            Dim resultList As StringBuilder = New StringBuilder("")
            resultList.Append(String.Format("Status: {0}" & Microsoft.VisualBasic.Constants.vbLf, result.ResultCode))
            resultList.Append(String.Format("Fault reason: {0}" & Microsoft.VisualBasic.Constants.vbLf, result.FaultReason))
            If result.StartingPoints IsNot Nothing Then
                resultList.Append(String.Format("_________________________" & Microsoft.VisualBasic.Constants.vbLf))
                Dim i As Integer = 1
                For Each startingPoint As RouteWaypoint In result.StartingPoints
                    resultList.Append(String.Format("Starting point {0}: {1} ({2})" & Microsoft.VisualBasic.Constants.vbLf, Math.Min(Threading.Interlocked.Increment(i), i - 1), startingPoint.Description, startingPoint.Location))
                Next
            End If

            If result.RouteResults IsNot Nothing Then
                Dim rnum As Integer = 1
                For Each routeResult As BingRouteResult In result.RouteResults
                    resultList.Append(String.Format("_________________________" & Microsoft.VisualBasic.Constants.vbLf))
                    resultList.Append(String.Format("Path {0}:" & Microsoft.VisualBasic.Constants.vbLf, Math.Min(Threading.Interlocked.Increment(rnum), rnum - 1)))
                    resultList.Append(String.Format("Distance: {0}" & Microsoft.VisualBasic.Constants.vbLf, routeResult.Distance))
                    resultList.Append(String.Format("Time: {0}" & Microsoft.VisualBasic.Constants.vbLf, routeResult.Time))
                    Dim path As MapPolyline = New MapPolyline()
                    path.Stroke = New SolidColorBrush(Colors.Red)
                    path.StrokeStyle = New StrokeStyle() With {.Thickness = 2}
                    For Each point As GeoPoint In routeResult.RoutePath
                        path.Points.Add(point)
                    Next

                    If routeResult.Legs IsNot Nothing Then
                        Dim legNum As Integer = 1
                        For Each leg As BingRouteLeg In routeResult.Legs
                            resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & "Leg {0}:" & Microsoft.VisualBasic.Constants.vbLf, Math.Min(Threading.Interlocked.Increment(legNum), legNum - 1)))
                            resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & "Start: {0}" & Microsoft.VisualBasic.Constants.vbLf, leg.StartPoint))
                            resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & "End: {0}" & Microsoft.VisualBasic.Constants.vbLf, leg.EndPoint))
                            resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & "Distance: {0}" & Microsoft.VisualBasic.Constants.vbLf, leg.Distance))
                            resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & "Time: {0}" & Microsoft.VisualBasic.Constants.vbLf, leg.Time))
                            If leg.Itinerary IsNot Nothing Then
                                Dim itNum As Integer = 1
                                For Each itineraryItem As BingItineraryItem In leg.Itinerary
                                    resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & Microsoft.VisualBasic.Constants.vbTab & "Itinerary {0}:" & Microsoft.VisualBasic.Constants.vbLf, Math.Min(Threading.Interlocked.Increment(itNum), itNum - 1)))
                                    resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & Microsoft.VisualBasic.Constants.vbTab & "Maneuver: {0}" & Microsoft.VisualBasic.Constants.vbLf, itineraryItem.Maneuver))
                                    resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & Microsoft.VisualBasic.Constants.vbTab & "Location: {0}" & Microsoft.VisualBasic.Constants.vbLf, itineraryItem.Location))
                                    resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & Microsoft.VisualBasic.Constants.vbTab & "Instructions: {0}" & Microsoft.VisualBasic.Constants.vbLf, itineraryItem.ManeuverInstruction))
                                    Dim warnNum As Integer = 1
                                    For Each warning As BingItineraryItemWarning In itineraryItem.Warnings
                                        resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & Microsoft.VisualBasic.Constants.vbTab & Microsoft.VisualBasic.Constants.vbTab & "Warning {0}:" & Microsoft.VisualBasic.Constants.vbLf, Math.Min(Threading.Interlocked.Increment(warnNum), warnNum - 1)))
                                        resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & Microsoft.VisualBasic.Constants.vbTab & Microsoft.VisualBasic.Constants.vbTab & "Type: {0}" & Microsoft.VisualBasic.Constants.vbLf, warning.Type))
                                        resultList.Append(String.Format(Microsoft.VisualBasic.Constants.vbTab & Microsoft.VisualBasic.Constants.vbTab & Microsoft.VisualBasic.Constants.vbTab & "Text: {0}" & Microsoft.VisualBasic.Constants.vbLf, warning.Text))
                                    Next
                                Next
                            End If

                            If leg.StartPoint IsNot Nothing Then
                                Dim start As MapDot = New MapDot()
                                start.Size = 10
                                start.Location = leg.StartPoint
                            End If

                            If leg.EndPoint IsNot Nothing Then
                                Dim [end] As MapDot = New MapDot()
                                [end].Size = 15
                                [end].Location = leg.EndPoint
                            End If
                        Next
                    End If
                Next
            End If

            Me.tbResults.Text = resultList.ToString()
        End Sub
    End Class
End Namespace
