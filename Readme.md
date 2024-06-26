<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128571517/21.1.5%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E4250)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* **[MainWindow.xaml](./CS/CalculateRoutesFromMajorRoads/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/CalculateRoutesFromMajorRoads/MainWindow.xaml))**
* [MainWindow.xaml.cs](./CS/CalculateRoutesFromMajorRoads/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/CalculateRoutesFromMajorRoads/MainWindow.xaml.vb))
<!-- default file list end -->
# How to get routes to the destination point using the Bing Map Route Data Service  


<p>This example  demonstrates how to calculate routes to the destination point from major roads using the <a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapBingRouteDataProvider_CalculateRoutesFromMajorRoadstopic"><u>BingRouteDataProvider.CalculateRoutesFromMajorRoads</u></a> method.</p><p>Before route calculation, specify destination point coordinates (<a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapGeoPoint_Latitudetopic"><u>GeoPoint.Latitude</u></a> and <a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapGeoPoint_Longitudetopic"><u>GeoPoint.Longitude</u></a>). In addition, you can specify the optional parameters: the destination name (<a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapRouteWaypoint_Descriptiontopic"><u>RouteWaypoint.Description</u></a>), driving or walking route travel mode using the <a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapBingRouteOptions_Modetopic"><u>BingRouteOptions.Mode</u></a> property and route optimization options to calculate the optimal route either by time or by distance via the <a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapBingRouteOptions_RouteOptimizationtopic"><u>BingRouteOptions.RouteOptimization</u></a> property.</p><p>To start the application, click the <strong>Calculate Routes From major Roads</strong> button , it handles the<strong> calculateRoutes_Click </strong>event.  All parameters are passed to the <strong>CalculateMajorRoutes</strong> method, and you can see the results in the textblock element below and calculated routes on a map. </p><p>The requested results contain the total distance of a route, route leg, itinerary item ( <a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapBingRouteResult_Distancetopic"><u>BingRouteResult.Distance</u></a>, <a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapBingRouteLeg_Distancetopic"><u>BingRouteLeg.Distance</u></a>, <a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapBingItineraryItem_Distancetopic"><u>BingItineraryItem.Distance</u></a>), the time required to follow the calculated route ( <a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapBingRouteResult_Timetopic"><u>BingRouteResult.Time</u></a>) and pass the rout leg and itinerary item (<a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapBingRouteLeg_Timetopic"><u>BingRouteLeg.Time</u></a>, <a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapBingItineraryItem_Timetopic"><u>BingItineraryItem.Time</u></a>). You can also see the maneuver instructions  associated with the itinerary item (<a href="http://documentation.devexpress.com/#WPF/DevExpressXpfMapBingItineraryItem_ManeuverInstructiontopic"><u>BingItineraryItem.ManeuverInstruction</u></a>) and other parameters.</p><p>Note that if you run this sample as is, you will get a warning message informing that the specified Bing Maps key is invalid. To learn more how to register a Bing Maps account and create a key for it,  please refer to the <a href="http://documentation.devexpress.com/#WPF/CustomDocument10974"><u>How to: Get a Bing Maps Key</u></a> tutorial.</p><br />


<br/>


<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=wpf-map-get-routes-to-the-destination-point-with-the-bing-map-route-data-service&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=wpf-map-get-routes-to-the-destination-point-with-the-bing-map-route-data-service&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
