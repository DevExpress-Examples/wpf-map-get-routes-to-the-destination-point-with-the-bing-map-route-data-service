Imports DevExpress.Xpf.Map
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls

Namespace CalculateRoutesFromMajorRoads
    Partial Public Class MainWindow
        Inherits Window

        Public location As New GeoPoint(40, -120)

        Public Sub New()
            InitializeComponent()
            Me.DataContext = location
        End Sub

        #Region "#RouteCalculated"
        Private Sub routeDataProvider_RouteCalculated(ByVal sender As Object, ByVal e As BingRouteCalculatedEventArgs)
            Dim result As RouteCalculationResult = e.CalculationResult

            Dim resultList As New StringBuilder()
            resultList.Append(String.Format("Status: {0}" & ControlChars.Lf, result.ResultCode))
            resultList.Append(String.Format("Fault reason: {0}" & ControlChars.Lf, result.FaultReason))
            resultList.Append(ProcessIntermediatePoints(result.IntermediatePoints))
            resultList.Append(ProcessRouteResults(result.RouteResults))

            tbResults.Text = resultList.ToString()
        End Sub
        #End Region ' #RouteCalculated

        #Region "#ProcessStartingPoints"
        Private Function ProcessIntermediatePoints(ByVal points As List(Of RouteWaypoint)) As String
            If points Is Nothing Then
                Return ""
            End If

            Dim sb As New StringBuilder("Intermediate Points:" & ControlChars.Lf)
            sb.Append(String.Format("_________________________" & ControlChars.Lf))
            For i As Integer = 0 To points.Count - 1
                    sb.Append(String.Format("Intermediate point {0}: {1} ({2})" & ControlChars.Lf, i+1, points(i).Description, points(i).Location))
            Next i
            Return sb.ToString()
        End Function
        #End Region ' #ProcessStartingPoints

        #Region "#ProcessRouteResults"
        Private Function ProcessRouteResults(ByVal results As List(Of BingRouteResult)) As String
            If results Is Nothing Then
                Return ""
            End If

            Dim sb As New StringBuilder("RouteResults:" & ControlChars.Lf)
            For i As Integer = 0 To results.Count - 1
                sb.Append(String.Format("_________________________" & ControlChars.Lf))
                sb.Append(String.Format("Path {0}:" & ControlChars.Lf, i+1))
                sb.Append(String.Format("Distance: {0}" & ControlChars.Lf, results(i).Distance))
                sb.Append(String.Format("Time: {0}" & ControlChars.Lf, results(i).Time))
                sb.Append(ProcessLegs(results(i).Legs))
            Next i
            Return sb.ToString()
        End Function
        #End Region ' #ProcessRouteResults

        #Region "#ProcessLegs"
        Private Function ProcessLegs(ByVal legs As List(Of BingRouteLeg)) As String
            If legs Is Nothing Then
                Return ""
            End If

            Dim sb As New StringBuilder("Legs:" & ControlChars.Lf)
            For i As Integer = 0 To legs.Count - 1
                sb.Append(String.Format(ControlChars.Tab & "Leg {0}:" & ControlChars.Lf, i+1))
                sb.Append(String.Format(ControlChars.Tab & "Start: {0}" & ControlChars.Lf, legs(i).StartPoint))
                sb.Append(String.Format(ControlChars.Tab & "End: {0}" & ControlChars.Lf, legs(i).EndPoint))
                sb.Append(String.Format(ControlChars.Tab & "Distance: {0}" & ControlChars.Lf, legs(i).Distance))
                sb.Append(String.Format(ControlChars.Tab & "Time: {0}" & ControlChars.Lf, legs(i).Time))
                sb.Append(ProcessItinerary(legs(i).Itinerary))
            Next i
            Return sb.ToString()
        End Function
        #End Region ' #ProcessLegs

        #Region "#ProcessItinerary"
        Private Function ProcessItinerary(ByVal items As List(Of BingItineraryItem)) As String
            If items Is Nothing Then
                Return ""
            End If

            Dim sb As New StringBuilder(ControlChars.Tab & "Internary Items:" & ControlChars.Lf)
            For i As Integer = 0 To items.Count - 1
                sb.Append(String.Format(ControlChars.Tab & ControlChars.Tab & "Itinerary {0}:" & ControlChars.Lf, i+1))
                sb.Append(String.Format(ControlChars.Tab & ControlChars.Tab & "Maneuver: {0}" & ControlChars.Lf, items(i).Maneuver))
                sb.Append(String.Format(ControlChars.Tab & ControlChars.Tab & "Location: {0}" & ControlChars.Lf, items(i).Location))
                sb.Append(String.Format(ControlChars.Tab & ControlChars.Tab & "Instructions: {0}" & ControlChars.Lf, items(i).ManeuverInstruction))
                sb.Append(ProcessWarnings(items(i).Warnings))
            Next i
            Return sb.ToString()
        End Function
        #End Region ' #ProcessItinerary

        #Region "#ProcessWarnings"
        Private Function ProcessWarnings(ByVal warnings As List(Of BingItineraryItemWarning)) As String
            If warnings Is Nothing Then
                Return ""
            End If

            Dim sb As New StringBuilder(ControlChars.Tab & ControlChars.Tab & "Warnings:" & ControlChars.Lf)
            For i As Integer = 0 To warnings.Count - 1
                sb.Append(String.Format(ControlChars.Tab & ControlChars.Tab & ControlChars.Tab & "Warning {0}:" & ControlChars.Lf, i + 1))
                sb.Append(String.Format(ControlChars.Tab & ControlChars.Tab & ControlChars.Tab & "Type: {0}" & ControlChars.Lf, warnings(i).Type))
                sb.Append(String.Format(ControlChars.Tab & ControlChars.Tab & ControlChars.Tab & "Text: {0}" & ControlChars.Lf, warnings(i).Text))

            Next i
            Return sb.ToString()
        End Function
        #End Region ' #ProcessWarnings

        #Region "#CalculateRoutesFromMajorRoads"
        Private Sub calculateRoutes_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            routeProvider.CalculateRoutesFromMajorRoads(New RouteWaypoint(tbDestination.Text, location))
        End Sub
        #End Region ' #CalculateRoutesFromMajorRoads

        Private Sub ValidationError(ByVal sender As Object, ByVal e As ValidationErrorEventArgs)
            If e.Action = ValidationErrorEventAction.Added Then
                MessageBox.Show(e.Error.ErrorContent.ToString())
            End If
        End Sub
    End Class

    Friend Class RangeDoubleValidationRule
        Inherits ValidationRule


        Private min_Renamed As Double = Double.MinValue

        Private max_Renamed As Double = Double.MaxValue
        Public Property Min() As Double
            Get
                Return min_Renamed
            End Get
            Set(ByVal value As Double)
                If (value <> min_Renamed) AndAlso (value <=max_Renamed) Then
                    min_Renamed = value
                End If
            End Set
        End Property
        Public Property Max() As Double
            Get
                Return max_Renamed
            End Get
            Set(ByVal value As Double)
                If (value <> max_Renamed) AndAlso (value >= min_Renamed) Then
                    max_Renamed = value
                End If
            End Set
        End Property

        Public Overrides Function Validate(ByVal value As Object, ByVal cultureInfo As System.Globalization.CultureInfo) As ValidationResult
            Dim d As Double = Nothing
            If Not Double.TryParse(TryCast(value, String), d) Then
                Return New ValidationResult(False, "Input value should be floating point number.")
            End If

            If (d < min_Renamed) OrElse (d > max_Renamed) Then
                Return New ValidationResult(False, String.Format("Input value should be larger than or equals to {0} and less than or equals to {1}", min_Renamed, max_Renamed))
            End If

            Return New ValidationResult(True, Nothing)
        End Function
    End Class

End Namespace